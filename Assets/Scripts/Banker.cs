using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Banker : MonoBehaviour {
    [SerializeField] public bool isEnabled;
    public Bank bank;

    private void OnCollisionStay2D(Collision2D collision) {
        if (!isEnabled) {
            return;
        }

        Bank collisionBank = collision.collider.GetComponent<Bank>();
        if (collisionBank == null || !collisionBank.isEnabled) {
            return;
        }

        if (collisionBank.banker != null && collisionBank.banker != this) {
            return;
        }

        if (bank == null) {
            SetBank(collisionBank);
        } else if (bank != collisionBank) { // maybe need a better comparison method here
            return;
        }

        TokenCollector tc = GetComponent<TokenCollector>();
        if (tc == null || tc.tokens < 1) {
            return;
        }

        if (tc.tokens == bank.tokens) {
            return;
        }

        Debug.Log(string.Format("Deposited: {0} (Total: {1})", tc.tokens - bank.tokens, tc.tokens));
        bank.tokens = tc.tokens;
    }

    public void SetBank(Bank _bank) {
        bank = _bank;
        bank.banker = this;

        Colorable c1 = GetComponent<Colorable>();
        Colorable c2 = bank.GetComponent<Colorable>();
        if (c1 != null && c2 != null) {
            c2.color = c1.color;
        }
    }
}
