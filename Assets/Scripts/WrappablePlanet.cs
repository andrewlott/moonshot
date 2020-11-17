using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WrappablePlanet : MonoBehaviour {
    [SerializeField]
    private bool isEnabled;
    private Camera mainCamera;
    // How far out of bounds before teleporting
    private static float threshold = 0.25f;
    private static Vector3 viewSize;
    private List<GameObject> wrappedPlanets;

    private void Start() {
        mainCamera = Camera.main;
        viewSize = mainCamera.ViewportToWorldPoint(Vector3.one) - Camera.main.ViewportToWorldPoint(Vector3.zero);
        MakeWrappingPlanets(gameObject);
    }

    private void Update() {
        if (!isEnabled) {
            return;
        }

        Wrap();
    }

    private void Wrap() {
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

        // Wrapped planets
        List<Vector3> positions = GetWrapPlanetPositions(gameObject);
        for (int i = 0; i < wrappedPlanets.Count; i++) {
            wrappedPlanets[i].transform.position = positions[i];
        }
    }

    private List<Vector3> GetWrapPlanetPositions(GameObject planet) {
        Vector3 mainPosition = planet.transform.position;
        List<Vector3> positions = new List<Vector3>();
        for (int i = 0; i < 3; i++) {
            for (int j = 0; j < 3; j++) {
                if (i == j) {
                    // Skip main planet
                    continue;
                }
                positions.Add(new Vector3(
                    mainPosition.x - viewSize.x + (i * viewSize.x),
                    mainPosition.y - viewSize.y + (j * viewSize.y),
                    0.0f
                ));
            }
        }
        return positions;
    }

    private void MakeWrappingPlanets(GameObject planet) {
        Vector3 mainPosition = planet.transform.position;
        List<Vector3> positions = GetWrapPlanetPositions(planet);
        wrappedPlanets = new List<GameObject>();
        foreach (Vector3 pos in positions) {
            GameObject g = Instantiate(planet, pos, Quaternion.identity);
            Destroy(g.GetComponent<WrappablePlanet>());
            g.GetComponent<SpriteRenderer>().enabled = false;
            wrappedPlanets.Add(g);
        }
    }
}
