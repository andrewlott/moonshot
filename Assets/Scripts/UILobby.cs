using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;
using UnityEngine.SceneManagement;

public class UILobby : MonoBehaviour {
    public static UILobby instance;
    [Header("Host/Join")]
    [SerializeField] private TMP_InputField matchIdInput;
    [SerializeField] private Button joinButton;
    [SerializeField] private Button hostButton;
    [SerializeField] private Canvas lobbyCanvas;
    [SerializeField] private Canvas roomCanvas;

    [Header("Room")]
    [SerializeField] public GameObject planetPrefab;
    [SerializeField] public GameObject roomPlayerPrefab;
    [SerializeField] public GameObject gameManagerPrefab;
    [SerializeField] private GameObject matchIdView;
    [SerializeField] private TMP_Text matchIdText;
    [SerializeField] private Button startButton;

    private bool isLocal;

    private void Start() {
        instance = this;
        isLocal = PlayerPrefs.GetInt("isLocal") == 1;

        if (isLocal) {
            lobbyCanvas.enabled = false;
            roomCanvas.enabled = true;
            matchIdView.SetActive(false);
            startButton.gameObject.SetActive(true);
            CreateLocalLobby();
        } else {
            lobbyCanvas.enabled = true;
            roomCanvas.enabled = false;
        }
    }

    private void Update() {
        if (GameManager.instance == null) {
            return;
        }

        if (isLocal) {
            for (int i = 1; i < GameManager.instance.maxLocalPlayers; i++) {
                if (!Input.GetKeyDown(Jumpable.JumpKeyFromPlayerId(i))) {
                    continue;
                }

                bool playerExists = false;
                foreach(GameObject g in GameManager.instance.players) {
                    Player player = g.GetComponent<Player>();
                    if (player.playerId == i) {
                        playerExists = true;
                    }
                }

                if (!playerExists) {
                    GameObject newRoomPlayer = Instantiate(roomPlayerPrefab, GameManager.instance.transform);
                    GameManager.instance.players.Add(newRoomPlayer);
                    newRoomPlayer.GetComponent<Player>().playerId = i;
                }
            }
        }

        bool canStart = GameManager.instance.CanStartGame();
        if (canStart && !startButton.enabled) {
            startButton.enabled = true;
        } else if (!canStart && startButton.enabled) {
            startButton.enabled = false;
        }
    }

    public void Host() {
        matchIdInput.interactable = false;
        joinButton.interactable = false;
        hostButton.interactable = false;

        LobbyPlayer.localPlayer.HostGame();
    }

    public void HostSuccess(bool success, string matchId) {
        if (success) {
            lobbyCanvas.enabled = false;
            roomCanvas.enabled = true;
            matchIdText.text = string.Format("Match ID: {0}", matchId);
            startButton.gameObject.SetActive(true);

            //GameObject gm = Instantiate(gameManagerPrefab);
            //gm.GetComponent<NetworkMatchChecker>().matchId = matchId.ToGuid();
            GameManager.instance.GetComponent<NetworkMatchChecker>().matchId = matchId.ToGuid();

            SpawnPlanet(LobbyPlayer.localPlayer, matchId);
            SpawnRoomPlayer(LobbyPlayer.localPlayer, matchId);
        } else {
            matchIdInput.interactable = true;
            joinButton.interactable = true;
            hostButton.interactable = true;
        }
    }

    public void Join() {
        matchIdInput.interactable = false;
        joinButton.interactable = false;
        hostButton.interactable = false;

        LobbyPlayer.localPlayer.JoinGame(matchIdInput.text.ToUpper());
    }

    public void JoinSuccess(bool success, string matchId) {
        if (success) {
            lobbyCanvas.enabled = false;
            roomCanvas.enabled = true;
            matchIdText.text = string.Format("Match ID: {0}", matchId);
            startButton.gameObject.SetActive(false);

            //GameObject gm = Instantiate(gameManagerPrefab);
            //gm.GetComponent<NetworkMatchChecker>().matchId = matchId.ToGuid();
            GameManager.instance.GetComponent<NetworkMatchChecker>().matchId = matchId.ToGuid();

            SpawnRoomPlayer(LobbyPlayer.localPlayer, matchId);
        } else {
            matchIdInput.interactable = true;
            joinButton.interactable = true;
            hostButton.interactable = true;
        }
    }

    public void SpawnPlanet(LobbyPlayer lobbyPlayer, string matchId) {
        //GameObject planet = Instantiate(planetPrefab);
        //planet.GetComponent<WrappablePlanet>().isEnabled = false;
        //Destroy(planet.GetComponent<Bank>());

        //planet.GetComponent<NetworkMatchChecker>().matchId = matchId.ToGuid();

        lobbyPlayer.SpawnPlanet(matchId);
    }

    public void SpawnRoomPlayer(LobbyPlayer lobbyPlayer, string matchId) {
        //GameObject newRoomPlayer = Instantiate(roomPlayerPrefab);

        //newRoomPlayer.GetComponent<NetworkMatchChecker>().matchId = matchId.ToGuid();

        lobbyPlayer.SpawnPlayer(matchId);
    }

    public void StartGame() {
        lobbyCanvas.enabled = false;
        roomCanvas.enabled = false;
        if (isLocal) {
            StartLocalGame();
        } else {
            LobbyPlayer.localPlayer.StartGame();
        }
    }

    private void CreateLocalLobby() {
        GameObject planet = Instantiate(planetPrefab, GameManager.instance.transform);
        GameManager.instance.planets.Add(planet);
        planet.GetComponent<WrappablePlanet>().isEnabled = false;
        planet.GetComponent<Bank>().isEnabled = false;

        GameObject newRoomPlayer = Instantiate(roomPlayerPrefab, GameManager.instance.transform);
        GameManager.instance.players.Add(newRoomPlayer);
    }

    private void StartLocalGame() {
        foreach (GameObject planet in GameManager.instance.planets) {
            NetworkServer.Destroy(planet);
            Destroy(planet);
        }
        SceneManager.LoadScene("MainScene", LoadSceneMode.Single);
        GameManager.instance.StartNewGame();
    }

    public void BackButtonPressed() {
        // TODO
    }
}
