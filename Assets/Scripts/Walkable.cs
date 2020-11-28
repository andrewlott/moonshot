﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walkable : MonoBehaviour {
    [SerializeField] public bool isEnabled;
    [SerializeField] public float walkAmount = 25.0f;

    private KeyCode leftKey;
    private KeyCode rightKey;
    private Collider2D collidedObject;
    private Rigidbody2D forceBody;
    

    private void Start() {
        Player p = GetComponent<Player>();
        if (p != null) {
            leftKey = LeftKeyFromPlayerId(p.playerId);
            rightKey = RightKeyFromPlayerId(p.playerId);
        }
        forceBody = GetComponent<Rigidbody2D>();
    }

    private void Update() {
        if (collidedObject == null || !isEnabled) {
            return;
        }

        Vector3 normalDirection = (collidedObject.transform.position - transform.position).normalized;
        if (Input.GetKey(leftKey)) {
            Vector3 leftDirection = new Vector3(normalDirection.y, -normalDirection.x, 0);
            forceBody.AddForce(leftDirection * walkAmount, ForceMode2D.Force); // Automatically applies once per second?
        }

        if (Input.GetKey(rightKey)) {
            Vector3 rightDirection = new Vector3(-normalDirection.y, normalDirection.x, 0);
            forceBody.AddForce(rightDirection * walkAmount, ForceMode2D.Force); // Automatically applies once per second?
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.collider.tag == "Planet") {
            collidedObject = collision.collider;
        }
    }

    private void OnCollisionExit2D(Collision2D collision) {
        if (collision.collider.tag == "Planet") {
            collidedObject = null;
        }
    }

    private KeyCode LeftKeyFromPlayerId(int playerId) {
        KeyCode key = KeyCode.LeftArrow;
        //switch(playerId) {
        //    case 1:
        //        key = KeyCode.Space;
        //        break;
        //    case 2:
        //        key = KeyCode.A;
        //        break;
        //    case 3:
        //        key = KeyCode.KeypadEnter;
        //        break;
        //}
        return key;
    }

    private KeyCode RightKeyFromPlayerId(int playerId) {
        KeyCode key = KeyCode.RightArrow;
        //switch(playerId) {
        //    case 1:
        //        key = KeyCode.Space;
        //        break;
        //    case 2:
        //        key = KeyCode.A;
        //        break;
        //    case 3:
        //        key = KeyCode.KeypadEnter;
        //        break;
        //}
        return key;
    }
}