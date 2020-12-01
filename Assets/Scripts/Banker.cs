using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Banker : MonoBehaviour {
    [SerializeField] private bool isEnabled;
    private Bank bank;

    private void OnCollisionEnter2D(Collision2D collision) {
        Bank collisionBank = collision.collider.GetComponent<Bank>();
        if (collisionBank == null || !collisionBank.isEnabled) {
            return;
        }

        if (collisionBank.banker != null && collisionBank.banker != this) {
            return;
        }

        if (bank == null) {
            bank = collisionBank;
            bank.banker = this;

            Colorable c1 = GetComponent<Colorable>();
            Colorable c2 = bank.GetComponent<Colorable>();
            if (c1 != null && c2 != null) {
                c2.color = c1.color;
            }
        } else if (bank != collisionBank) { // maybe need a better comparison method here
            return;
        }

        TokenCollector tc = GetComponent<TokenCollector>();
        if (tc == null || tc.tokens < 1) {
            return;
        }

        bank.tokens += tc.tokens;
        Debug.Log(string.Format("Deposited: {0} (Total: {1})", tc.tokens, bank.tokens));
        tc.tokens = 0;
    }
}
