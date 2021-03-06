﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    [SerializeField] public int playerId = 1;
    private Rigidbody2D rb;
    private float speed = 15.0f;
    private bool collided = false;
    private Animator animator;
    void Awake() {
        rb = this.GetComponent<Rigidbody2D>();
        animator = this.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update() {
        GameObject nearestPlanet = NearestPlanet();

        if (collided) {
            RotateWithPlanet(nearestPlanet);
        }

        Vector3 vectorToTarget = nearestPlanet.transform.position - transform.position;
        float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
        float planetRadius = nearestPlanet.GetComponent<CircleCollider2D>().radius * nearestPlanet.transform.localScale.x;
        float dist =  Mathf.Abs(Vector3.Distance(nearestPlanet.transform.position, transform.position)) - planetRadius;
        if (dist > 2 * planetRadius) {
            return;
        }
        float distProportion = Mathf.Max(1, Mathf.Min(0, planetRadius / dist));
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(Quaternion.identity, q, distProportion);
        rb.SetRotation(Quaternion.Slerp(transform.rotation, q, Time.deltaTime * speed));
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.collider.tag == "Planet") {
            animator.SetTrigger("land");
            collided = true;
        } else if (collision.collider.tag == "Player") {
            animator.SetTrigger("hurt");
        }
        //rb.isKinematic = true;
    }

    private void OnCollisionExit2D(Collision2D collision) {
        if (collision.collider.tag == "Planet") {
            collided = false;
        }
        //rb.isKinematic = false;
    }

    private float AngleInPlane(Transform from, Transform to, Vector3 planeNormal) {
        Vector3 dir = to.position - from.position;

        Vector3 p1 = Project(dir, planeNormal);
        Vector3 p2 = Project(from.forward, planeNormal);

        return Vector3.Angle(p1, p2);
    }

    private Vector3 Project(Vector3 v, Vector3 onto) {
        return v - (Vector3.Dot(v, onto) / Vector3.Dot(onto, onto)) * onto;
    }

    private GameObject NearestPlanet() {
        GameObject [] planets = GameObject.FindGameObjectsWithTag("Planet");
        GameObject nearestPlanet = null;
        foreach (GameObject planet in planets) {
            if (nearestPlanet == null ||
                Vector3.Distance(this.transform.position, planet.transform.position) < Vector3.Distance(this.transform.position, nearestPlanet.transform.position)) {
                nearestPlanet = planet;
            }
        }
        return nearestPlanet;
    }

    private void RotateWithPlanet(GameObject planet) {
        Rotator r = planet.GetComponent<Rotator>();
        transform.RotateAround(planet.transform.position, Vector3.forward, r.rotationSpeed * Time.deltaTime);
    }
}
