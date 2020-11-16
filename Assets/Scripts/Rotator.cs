using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour {
    public float rotationSpeed = 50.0f; // degrees per second
    public Vector3 rotationAmount;
    private Rigidbody2D rb;
    // Start is called before the first frame update
    void Start() {
        rb = GetComponent<Rigidbody2D>();
        rotationAmount = new Vector3(0.0f, 0.0f, rotationSpeed);
        rb.SetRotation(rotationSpeed);
    }

    // Update is called once per frame
    void Update() {
        //rb.SetRotation(rotationSpeed);
        transform.Rotate(rotationAmount * Time.deltaTime);
    }
}
