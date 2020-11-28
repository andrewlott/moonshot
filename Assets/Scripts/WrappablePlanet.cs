using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class WrappablePlanet : MonoBehaviour {
    [SerializeField]
    private bool isEnabled;
    private Camera mainCamera;
    // How far out of bounds before teleporting
    private static float threshold = 0.0f;
    public List<GameObject> wrappedPlanets;

    private void Start() {
        mainCamera = Camera.main;
        if (isEnabled) {
            MakeWrappingPlanets(gameObject);
        }
    }

    private void Update() {
        if (!isEnabled) {
            return;
        }

        Wrap();
    }

    private void Wrap() {
        Vector3 viewPos = mainCamera.WorldToViewportPoint(transform.position);
        // X
        if (viewPos.x < 0.0f - threshold) {
            transform.position = new Vector3(transform.position.x + Utils.ViewSize().x, transform.position.y, transform.position.z);
        } else if (viewPos.x > 1.0f + threshold) {
            transform.position = new Vector3(transform.position.x - Utils.ViewSize().x, transform.position.y, transform.position.z);
        }
        // Y
        if (viewPos.y < 0.0f - threshold) {
            transform.position = new Vector3(transform.position.x, transform.position.y + Utils.ViewSize().y, transform.position.z);
        } else if (viewPos.y > 1.0f + threshold) {
            transform.position = new Vector3(transform.position.x, transform.position.y - Utils.ViewSize().y, transform.position.z);
        }

        // Wrapped planets
        List<Vector3> positions = GetWrapPlanetPositions(gameObject);
        for (int i = 0; i < wrappedPlanets.Count; i++) {
            wrappedPlanets[i].transform.position = positions[i];
        }
    }

    private List<Vector3> GetWrapPlanetPositions(GameObject planet) {
        Vector3 mainPosition = planet.transform.position;
        List<Vector3> positions = new List<Vector3>();
        for (int i = 0; i < 3; i++) {
            for (int j = 0; j < 3; j++) {
                if (i == j) {
                    // Skip main planet
                    continue;
                }
                positions.Add(new Vector3(
                    mainPosition.x - Utils.ViewSize().x + (i * Utils.ViewSize().x),
                    mainPosition.y - Utils.ViewSize().y + (j * Utils.ViewSize().y),
                    0.0f
                ));
            }
        }
        return positions;
    }

    private void MakeWrappingPlanets(GameObject planet) {
        GameManager gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        Vector3 mainPosition = planet.transform.position;
        List<Vector3> positions = GetWrapPlanetPositions(planet);
        wrappedPlanets = new List<GameObject>();
        foreach (Vector3 pos in positions) {
            GameObject g = new GameObject();
            g.transform.SetParent(gm.transform);
            g.transform.position = pos;
            g.transform.localScale = planet.transform.localScale;

            foreach (Rigidbody2D rb in planet.GetComponents<Rigidbody2D>()) {
                g.AddComponent<Rigidbody2D>(rb);
            }
            foreach (CircleCollider2D c in planet.GetComponents<CircleCollider2D>()) {
                g.AddComponent<CircleCollider2D>(c);
            }
            foreach (PointEffector2D pe in planet.GetComponents<PointEffector2D>()) {
                g.AddComponent<PointEffector2D>(pe);
            }
            foreach (Rotator r in planet.GetComponents<Rotator>()) {
                g.AddComponent<Rotator>(r);
            }
            //foreach(NetworkIdentity ni in planet.GetComponents<NetworkIdentity>()) {
            //    g.AddComponent<NetworkIdentity>(ni);
            //}
            //foreach(NetworkTransform nt in planet.GetComponents<NetworkTransform>()) {
            //    g.AddComponent<NetworkTransform>(nt);
            //}
            //Destroy(g.GetComponent<WrappablePlanet>());
            //Destroy(g.GetComponent<NetworkIdentity>());
            //Destroy(g.GetComponent<NetworkTransform>());
            //g.GetComponent<SpriteRenderer>().enabled = false;
            wrappedPlanets.Add(g);
        }
    }

    private void OnDestroy() {
        foreach(GameObject wp in wrappedPlanets) {
            Destroy(wp);
        }
    }
}
