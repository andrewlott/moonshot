using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Mathematics;

public class GameManager : MonoBehaviour {
    [SerializeField] private GameObject planetPrefab;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private int numPlanets = 3;
    [SerializeField] private int numPlayers = 2;
    [SerializeField] private float minDistanceBetweenPlanets = 5.0f;

    private List<GameObject> planets;
    private List<GameObject> players;

    private void Start() {
        planets = GenerateRandomPlanets(numPlanets);
        players = GeneratePlayers(numPlayers);
    }

    private List<GameObject> GeneratePlayers(int numPlayers) {
        planets.Shuffle(); // not great
        List<GameObject> players = new List<GameObject>();
        for (int i = 0; i < numPlayers; i++) {
            GameObject player = GeneratePlayer(planets[i % numPlanets]);
            player.GetComponent<Player>().playerId = i + 1;
            players.Add(player);
        }
        return players;
    }

    private GameObject GeneratePlayer(GameObject nearPlanet) {
        CircleCollider2D cc = nearPlanet.GetComponent<CircleCollider2D>();
        float randomAngle = Mathf.Deg2Rad * 360.0f * UnityEngine.Random.value;

        Vector3 pos = nearPlanet.transform.position + new Vector3(
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
}
