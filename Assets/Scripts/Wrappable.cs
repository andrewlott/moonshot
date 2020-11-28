using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wrappable : MonoBehaviour {
    [SerializeField]
    private bool isEnabled;
    private Camera mainCamera;
    // How far out of bounds before teleporting
    private static float threshold = 0.0f;
    private static Vector3 viewSize;

    private void Start() {
        mainCamera = Camera.main;
        viewSize = mainCamera.ViewportToWorldPoint(Vector3.one) - Camera.main.ViewportToWorldPoint(Vector3.zero);
    }

    private void Update() {
        if (!isEnabled) {
            return;
        }
        Vector3 viewPos = mainCamera.WorldToViewportPoint(transform.position);
        // X
        if (viewPos.x < 0.0f - threshold) {
            transform.position = new Vector3(transform.position.x + viewSize.x, transform.position.y, transform.position.z);
        } else if (viewPos.x > 1.0f + threshold) {
            transform.position = new Vector3(transform.position.x - viewSize.x, transform.position.y, transform.position.z);
        }
        // Y
        if (viewPos.y < 0.0f - threshold) {
            transform.position = new Vector3(transform.position.x, transform.position.y + viewSize.y, transform.position.z);
        } else if (viewPos.y > 1.0f + threshold) {
            transform.position = new Vector3(transform.position.x, transform.position.y - viewSize.y, transform.position.z);
        }
    }
}
