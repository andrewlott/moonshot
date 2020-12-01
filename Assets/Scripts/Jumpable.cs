using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jumpable : MonoBehaviour {
    [SerializeField] private Rigidbody2D forceTarget;
    [SerializeField] public bool shouldJump;
    [SerializeField] public bool shouldPush;
    [SerializeField] public float jumpAmountMin = 10.0f;
    [SerializeField] public float jumpAmountMax = 50.0f;
    [SerializeField] public static float jumpAmount = 20.0f;

    private KeyCode jumpKey;
    private KeyCode attackKey;
    private Collider2D collidedObject;
    private Animator animator;

    private void Start() {
        Player p = GetComponent<Player>();
        if (p != null) {
            jumpKey = JumpKeyFromPlayerId(p.playerId);
            attackKey = AttackKeyFromPlayerId(p.playerId);
        }

        animator = GetComponent<Animator>();
    }

    private void Update() {
        if (collidedObject == null) {
            if (Input.GetKeyDown(attackKey)) {
                Vector3 direction = new Vector3(-1 + UnityEngine.Random.value * 2, -1 + UnityEngine.Random.value * 2, 0.0f);
                //forceTarget.velocity = Vector2.zero; // Cancel out walking force just in case, not sure it does anything
                forceTarget.AddForce(direction * jumpAmount * 0.25f, ForceMode2D.Impulse);

                animator.SetTrigger("hurt");
            }
            return;
        }

        if (Input.GetKeyDown(jumpKey)) {
            if (shouldJump) {
                Vector3 direction = (transform.position - collidedObject.transform.position).normalized;
                forceTarget.velocity = Vector2.zero; // Cancel out walking force just in case, not sure it does anything
                forceTarget.AddForce(direction * jumpAmount, ForceMode2D.Impulse);

                animator.SetTrigger("jump");
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

    public static KeyCode JumpKeyFromPlayerId(int playerId) {
        KeyCode key = KeyCode.UpArrow;
        switch (playerId) {
            case 1:
                key = KeyCode.UpArrow;
                break;
            case 2:
                key = KeyCode.W;
                break;
            case 3:
                key = KeyCode.Space;
                break;
        }
        return key;
    }

    public static KeyCode AttackKeyFromPlayerId(int playerId) {
        KeyCode key = KeyCode.DownArrow;
        switch (playerId) {
            case 1:
                key = KeyCode.DownArrow;
                break;
            case 2:
                key = KeyCode.S;
                break;
            case 3:
                key = KeyCode.V;
                break;
        }
        return key;
    }
}
