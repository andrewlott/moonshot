using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefreshableToken : MonoBehaviour {
    [SerializeField] public static float collectionCooldown = 10.0f; //seconds
    [SerializeField] public static Color color = Color.yellow;

    private float lastCollectedTime = -collectionCooldown;
    private Colorable colorable;

    private void Start() {
        colorable = GetComponent<Colorable>();
    }

    private void Update() {
        if (colorable) {
            Color targetColor;
            if (!IsCollectable()) {
                // lerp color
                targetColor = Color.Lerp(Color.clear, color, (Time.time - lastCollectedTime) / collectionCooldown);
            } else {
                targetColor = color;
            }

            if (colorable.color != targetColor) {
                colorable.color = targetColor;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision) {
        if (!IsCollectable()) {
            return;
        }
        TokenCollector tokenCollector = collision.GetComponent<Collider2D>().GetComponent<TokenCollector>();
        if (tokenCollector == null) {
            return;
        }

        tokenCollector.tokens++;
        Debug.Log(string.Format("Collected: {0}", tokenCollector.tokens));
        lastCollectedTime = Time.time;
    }

    private bool IsCollectable() {
        return Time.time - lastCollectedTime > collectionCooldown;
    }
}
