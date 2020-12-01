using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Mathematics;
using Mirror;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour {
    [Header("Prefabs")]
    [SerializeField] private GameObject planetPrefab;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject tokenPrefab;

    [Header("Game Variables")]
    [SerializeField] private int numPlanets = 3;
    [SerializeField] private int numPlayers = 1;
    [SerializeField] private float minDistanceBetweenPlanets = 5.0f;
    [SerializeField] private float minTokenDistance = 5.0f;
    [SerializeField] private int numTokensPerPlanet = 4;
    [SerializeField] public int minPlayersToStart = 2;
    [SerializeField] public int maxLocalPlayers = 3;

    [Header("State")]
    public SyncList<GameObject> planets = new SyncList<GameObject>();
    public SyncList<GameObject> players = new SyncList<GameObject>();
    public SyncList<GameObject> tokens = new SyncList<GameObject>();

    private bool gameStarted;

    public static GameManager instance;

    private void Awake() {
        instance = this;
        //planets = GenerateRandomPlanets(numPlanets);
        //players = GeneratePlayers(numPlayers);
    }

    public SyncList<GameObject> GeneratePlayers() {
        planets.Shuffle(); // not great
        // TODO: If it's the bank of another player, then skip
        players = new SyncList<GameObject>();
        for (int i = 0; i < numPlayers; i++) {
            GameObject player = GeneratePlayer(planets[i % numPlanets]);
            player.GetComponent<Player>().playerId = i + 1;
        }
        return players;
    }

    public GameObject GeneratePlayer(GameObject nearPlanet) {
        Vector3 pos = RandomPointOnPlanet(nearPlanet);
        GameObject player = Instantiate(playerPrefab, pos, Quaternion.identity, gameObject.transform);
        // Add to players
        players.Add(player);
        return player;
    }

    public Vector3 RandomPointOnPlanet(GameObject planet) {
        CircleCollider2D cc = planet.GetComponent<CircleCollider2D>();
        float dist = cc.radius * planet.transform.localScale.x;
        Vector3 pos = RandomPositionFromPoint(planet.transform.position, dist, dist);
        return pos;
    }

    public Vector3 RandomPositionFromPoint(Vector3 center, float minDistance, float maxDistance) {
        float randomAngle = Mathf.Deg2Rad * 360.0f * UnityEngine.Random.value;
        float randomDistance = minDistance + (maxDistance - minDistance) * UnityEngine.Random.value;
        Vector3 pos = center + new Vector3(
            randomDistance * Mathf.Cos(randomAngle),
            randomDistance * Mathf.Sin(randomAngle),
            0.0f
        );
        return ClampPositionInScreenBounds(pos);
    }

    public SyncList<GameObject> GenerateRandomPlanets() {
        planets = new SyncList<GameObject>();
        for (int i = 0; i < numPlanets; i++) {
            planets.Add(GenerateRandomPlanet(planets));
        }
        return planets;
    }

    private GameObject GenerateRandomPlanet(SyncList<GameObject> otherPlanets) {
        // make sure planets are formed in bounds
        // make a clone with disabled renderer for each one
        Vector3 basePosition = Camera.main.ViewportToWorldPoint(Vector3.zero);
        Vector3 viewSize = Utils.ViewSize();

        int sign = 0;
        if (otherPlanets.Count > 0) {
            sign = otherPlanets.Count % 2 == 0 ? -1 : 1;
        }
        float posX = viewSize.x / 2.0f + sign * viewSize.x / (numPlanets);
        //float posX = (1 + otherPlanets.Count) * viewSize.x / (numPlanets + 1);
        float posY = UnityEngine.Random.Range(1, 3) * viewSize.y / 3;
        Vector3 randomPosition = new Vector3(
            basePosition.x + posX,
            basePosition.y + posY,
            0.0f
        );
        int attempts = 0;
        int maxAttempts = 1000;
        while (!IsValidPlanetPosition(randomPosition, otherPlanets) && attempts < maxAttempts) {
            posY = UnityEngine.Random.Range(1, 3) * viewSize.y / 3;
            randomPosition = new Vector3(
                basePosition.x + posX,
                basePosition.y + posY,
                0.0f
            );
            attempts++;
        }

        if (attempts >= maxAttempts) {
            Debug.Log("Unable to generate non-colliding planet");
        }

        GameObject planet = Instantiate(planetPrefab, randomPosition, Quaternion.identity, gameObject.transform);

        //planet.transform.localScale = Vector3.one * (0.5f + UnityEngine.Random.value * 1.0f);
        return planet;
    }

    private bool IsValidPlanetPosition(Vector3 position, SyncList<GameObject> otherPlanets) {
        if (otherPlanets.Count == 0) {
            return true;
        }
        float epsilon = 0.5f;
        return IsValidPosition(position, otherPlanets, 2 * planets[0].GetComponent<Planet>().GetGravityDistance() + epsilon);
    }

    public SyncList<GameObject> GenerateTokens() {
        tokens = new SyncList<GameObject>();
        foreach(GameObject planet in planets) {
            for (int i = 0; i < numTokensPerPlanet; i++) {
                Vector3 position = RandomPositionFromPoint(
                    planet.transform.position,
                    planet.GetComponent<Planet>().GetPlanetRadius(),
                    planet.GetComponent<Planet>().GetGravityDistance()
                );
                int attempts = 0;
                int maxAttempts = 10000;
                while (!IsValidPosition(position, planets, planet.GetComponent<Planet>().GetPlanetRadius()) && attempts < maxAttempts) {
                    position = RandomPositionFromPoint(
                        planet.transform.position,
                        planet.GetComponent<Planet>().GetPlanetRadius(),
                        planet.GetComponent<Planet>().GetGravityDistance()
                    );
                    attempts++;
                }
                if (attempts >= maxAttempts) {
                    Debug.Log("Unable to generate non-colliding token");
                }
                tokens.Add(GenerateToken(position));
            }
        }
        return tokens;
    }

    private bool IsValidPosition(Vector3 position, SyncList<GameObject> otherObjects, float minDistance) {
        foreach (GameObject g in otherObjects) {
            if (Vector3.Distance(g.transform.position, position) < minDistance) {
                return false;
            }
        }
        return true;
    }

    public GameObject GenerateToken(Vector3 position) {
        GameObject token = Instantiate(tokenPrefab, position, Quaternion.identity, gameObject.transform);
        return token;
    }

    private Vector3 ClampPositionInScreenBounds(Vector3 mainPosition) {
        Vector3 basePosition = Camera.main.ViewportToWorldPoint(Vector3.zero);
        for (int i = 0; i < 3; i++) {
            for (int j = 0; j < 3; j++) {
                Vector3 newPosition = new Vector3(
                    mainPosition.x - Utils.ViewSize().x + (i * Utils.ViewSize().x),
                    mainPosition.y - Utils.ViewSize().y + (j * Utils.ViewSize().y),
                    0.0f
                );

                if (basePosition.x < newPosition.x && basePosition.x + Utils.ViewSize().x > newPosition.x &&
                    basePosition.y < newPosition.y && basePosition.y + Utils.ViewSize().y > newPosition.y
                ) {
                    return newPosition;
                }
            }
        }

        Debug.Log("Failed to clamp position");
        return mainPosition;
    }

    public bool CanStartGame() {
        return !gameStarted && players.Count >= minPlayersToStart;
    }

    public void StartNewGame() {
        GenerateRandomPlanets();
        GenerateTokens();

        planets.Shuffle();
        for (int i = 0; i < players.Count; i++) {
            GameObject player = players[i];
            player.transform.position = RandomPointOnPlanet(planets[i]);
            player.GetComponent<Banker>().SetBank(planets[i].GetComponent<Bank>());
        }

        gameStarted = true;
    }

    public void SetLocalPlayerColor(int playerIndex, int colorIndex) {
        Colorable c = players[playerIndex].GetComponent<Colorable>();
        c.color = c.colorChoices[colorIndex];
    }

    public Player GetPlayerForPlayerId(int id) {
        foreach (GameObject g in GameManager.instance.players) {
            Player player = g.GetComponent<Player>();
            if (player.playerId == id) {
                return player;
            }
        }
        return null;
    }

    public void SetGravityScale(float gravityScale) {
        foreach(GameObject g in planets) {
            Planet p = g.GetComponent<Planet>();
            float amount = p.minGravity + (p.maxGravity - p.minGravity) * gravityScale;
            p.SetGravity(amount);
        }
    }

    public void SetJumpScale(float jumpScale) {
        Jumpable jumper = players[0].GetComponent<Jumpable>();
        Jumpable.jumpAmount = jumper.jumpAmountMin + (jumper.jumpAmountMax - jumper.jumpAmountMin) * jumpScale;
    }

    public void SetWalkScale(float walkScale) {
        Walkable walker = players[0].GetComponent<Walkable>();
        Walkable.walkAmount = walker.walkAmountMin + (walker.walkAmountMax - walker.walkAmountMin) * walkScale;
    }

    public void GoBackToLobby() {
        foreach(GameObject g in planets) {
            Destroy(g);
        }
        foreach(GameObject g in players) {
            Destroy(g);
        }
        foreach(GameObject g in tokens) {
            Destroy(g);
        }
        SceneManager.LoadScene("LobbyScene", LoadSceneMode.Single);
    }
}