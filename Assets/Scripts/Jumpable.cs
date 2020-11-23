using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jumpable : MonoBehaviour {
    [SerializeField] private Rigidbody2D forceTarget;
    [SerializeField] public bool shouldJump;
    [SerializeField] public bool shouldPush;
    private KeyCode jumpKey;
    private Collider2D collidedObject;

    private void Start() {
        Player p = GetComponent<Player>();
        if (p != null) {
            jumpKey = JumpKeyFromPlayerId(p.playerId);
        }
    }

    private void Update() {
        if (collidedObject == null) {
            return;
        }

        if (Input.GetKeyDown(jumpKey)) {
            if (shouldJump) {
                Vector3 direction = transform.position - collidedObject.transform.position;
                forceTarget.AddForce(direction * 10, ForceMode2D.Impulse);
            }

            if (shouldPush) {
                Vector3 direction = transform.position - collidedObject.transform.position;
                collidedObject.GetComponent<Rigidbody2D>().AddForce(-direction * 100000, ForceMode2D.Impulse);
            }
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

    private KeyCode JumpKeyFromPlayerId(int playerId) {
        KeyCode key = KeyCode.Space;
        switch(playerId) {
            case 1:
                key = KeyCode.Space;
                break;
            case 2:
                key = KeyCode.A;
                break;
            case 3:
                key = KeyCode.KeypadEnter;
                break;
        }
        return key;
    }
}
