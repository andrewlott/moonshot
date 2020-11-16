using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Mathematics;

public class GameManager : MonoBehaviour {
    [SerializeField] private GameObject planetPrefab;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private int numPlanets = 3;
    [SerializeField] private float minDistanceBetweenPlanets = 5.0f;

    private List<GameObject> planets;
    private GameObject player;

    private void Start() {
        planets = GenerateRandomPlanets(numPlanets);
        player = GeneratePlayer();
    }

    private GameObject GeneratePlayer() {
        GameObject randomPlanet = planets[Mathf.FloorToInt(planets.Count * UnityEngine.Random.value)];
        CircleCollider2D cc = randomPlanet.GetComponent<CircleCollider2D>();

        float randomAngle = Mathf.Deg2Rad * 360.0f * UnityEngine.Random.value;

        Vector3 pos = new Vector3(
            cc.radius * Mathf.Cos(randomAngle),
            cc.radius * Mathf.Sin(randomAngle),
            0.0f
        );
        GameObject player = Instantiate(playerPrefab, pos, Quaternion.identity);

        return player;
    }

    private List<GameObject> GenerateRandomPlanets(int numPlanets) {
        List<GameObject> planets = new List<GameObject>();
        for (int i = 0; i < numPlanets; i++) {
            planets.Add(GenerateRandomPlanet(planets));
        }
        return planets;
    }

    private GameObject GenerateRandomPlanet(List<GameObject> otherPlanets) {
        // make sure planets are formed in bounds
        // make a clone with disabled renderer for each one
        Vector3 basePosition = Camera.main.ViewportToWorldPoint(Vector3.zero);
        Vector3 viewSize = Camera.main.ViewportToWorldPoint(Vector3.one) - basePosition;
            
        Vector3 randomPosition = new Vector3(
            basePosition.x + UnityEngine.Random.value * viewSize.x,
            basePosition.y + UnityEngine.Random.value * viewSize.y,
            0.0f
        );
        while (!IsValidPlanetPosition(randomPosition, otherPlanets)) {
            randomPosition = new Vector3(
                basePosition.x + UnityEngine.Random.value * viewSize.x,
                basePosition.y + UnityEngine.Random.value * viewSize.y,
                0.0f
            );
        }

        GameObject planet = Instantiate(planetPrefab, randomPosition, Quaternion.identity);
        //planet.transform.localScale = Vector3.one * (0.5f + UnityEngine.Random.value * 1.0f);
        MakeWrappingPlanets(planet);
        return planet;
    }

    private bool IsValidPlanetPosition(Vector3 position, List<GameObject> otherPlanets) {
        foreach(GameObject planet in otherPlanets) {
            if (Vector3.Distance(planet.transform.position, position) < minDistanceBetweenPlanets) {
                return false;
            }
        }
        return true;
    }

    private void MakeWrappingPlanets(GameObject planet) {
        Vector3 viewSize = Camera.main.ViewportToWorldPoint(Vector3.one) - Camera.main.ViewportToWorldPoint(Vector3.zero);
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
        foreach (Vector3 pos in positions) {
            GameObject g = Instantiate(planetPrefab, pos, Quaternion.identity);
            g.GetComponent<SpriteRenderer>().enabled = false;
        }
    }
}
