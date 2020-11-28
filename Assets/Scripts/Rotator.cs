using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour {
    public float rotationSpeedMin = 10.0f; // degrees per second
    public float rotationSpeedMax = 60.0f; // degrees per second
    public float rotationSpeed;
    public Vector3 rotationAmount;
    private Rigidbody2D rb;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        if (rotationSpeed == 0.0f) {
            rotationSpeed = rotationSpeedMin + (rotationSpeedMax - rotationSpeedMin) * UnityEngine.Random.value;
            if (UnityEngine.Random.value < 0.5f) {
                rotationSpeed *= -1;
            }
        }
        rotationAmount = new Vector3(0.0f, 0.0f, rotationSpeed);
        rb.SetRotation(rotationSpeed);
    }

    private void Update() {
        //rb.SetRotation(rotationSpeed);
        transform.Rotate(rotationAmount * Time.deltaTime);
    }
}
