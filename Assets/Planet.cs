using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour {
    [SerializeField] private PointEffector2D gravityPointEffector;
    [SerializeField] public float minGravity = -10.0f;
    [SerializeField] public float maxGravity = -200.0f;

    public void SetGravity(float amount) {
        gravityPointEffector.forceMagnitude = amount;
    }

    public float GetPlanetRadius() {
        return GetCollider(false).radius * transform.localScale.x;
    }

    public float GetGravityDistance() {
        return GetCollider(true).radius * transform.localScale.x;
    }

    private CircleCollider2D GetCollider(bool isEffector) {
        foreach(CircleCollider2D c in GetComponents<CircleCollider2D>()) {
            if (c.usedByEffector == isEffector) {
                return c;
            }
        }
        return null;
    }
}
