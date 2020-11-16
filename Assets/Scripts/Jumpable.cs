using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jumpable : MonoBehaviour {
    [SerializeField] private Rigidbody2D forceTarget;
    private Collider2D collidedObject;
    private void Start() {
    }

    private void Update() {
        if (collidedObject == null) {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space)) {
            Vector3 direction = transform.position - collidedObject.transform.position;
            forceTarget.AddForce(direction * 10, ForceMode2D.Impulse);
            Debug.Log("Added force");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.collider.tag == "Planet") {
            collidedObject = collision.collider;
            Debug.Log("Collided");
        }
    }

    private void OnCollisionExit2D(Collision2D collision) {
        if (collision.collider.tag == "Planet") {
            collidedObject = null;
            Debug.Log("Uncollided");
        }
    }
}
