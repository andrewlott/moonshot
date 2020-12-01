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
    [SerializeField] private TMP_Text matchIdText;
    [SerializeField] private Button startButton;
    private void Start() {
        instance = this;
        lobbyCanvas.enabled = true;
        roomCanvas.enabled = false;
    }

    private void Update() {
        if (GameManager.instance == null) {
            return;
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
        LobbyPlayer.localPlayer.StartGame();
    }

    public void BackButtonPressed() {
        // TODO
    }
}
