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
}
