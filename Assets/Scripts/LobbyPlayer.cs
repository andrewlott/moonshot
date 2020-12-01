using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class LobbyPlayer : NetworkBehaviour {
    public static LobbyPlayer localPlayer;
    [SyncVar]
    public string matchId;
    [SyncVar]
    public int playerIndex;
    private NetworkMatchChecker matchChecker;

    private void Start() {
        matchChecker = GetComponent<NetworkMatchChecker>();

        if (isLocalPlayer) {
            localPlayer = this;
        }
    }

    /* Host game */

    public void HostGame() {
        string _matchId = MatchMaker.GetRandomMatchId();
        CmdHostGame(_matchId);
    }

    [Command]
    private void CmdHostGame(string _matchId) {
        matchId = _matchId;
        if (MatchMaker.instance.HostGame(matchId, gameObject, out playerIndex)) {
            Debug.Log("Game hosted successfully");
            matchChecker.matchId = matchId.ToGuid();
            TargetHostGame(true, matchId, playerIndex);
        } else {
            Debug.Log("Game hosted failed");
            TargetHostGame(false, matchId, playerIndex);
        }
    }

    [TargetRpc]
    private void TargetHostGame(bool success, string _matchId, int _playerIndex) {
        UILobby.instance.HostSuccess(success, _matchId);
    }

    /* Join game */

    public void JoinGame(string _matchId) {
        CmdJoinGame(_matchId);
    }

    [Command]
    private void CmdJoinGame(string _matchId) {
        matchId = _matchId;
        if (MatchMaker.instance.JoinGame(matchId, gameObject, out playerIndex)) {
            Debug.Log("Game hosted successfully");
            matchChecker.matchId = matchId.ToGuid();
            TargetJoinGame(true, matchId, playerIndex);
        } else {
            Debug.Log("Game hosted failed");
            TargetJoinGame(false, matchId, playerIndex);
        }
    }

    [TargetRpc]
    private void TargetJoinGame(bool success, string _matchId, int _playerIndex) {
        UILobby.instance.JoinSuccess(success, _matchId);
    }

    public void SpawnPlanet(string _matchId) {
        CmdSpawnPlanet(_matchId);
    }

    [Command]
    private void CmdSpawnPlanet(string _matchId) {
        GameObject planet = Instantiate(UILobby.instance.planetPrefab, GameManager.instance.transform);
        planet.GetComponent<WrappablePlanet>().isEnabled = false;
        planet.GetComponent<Bank>().isEnabled = false;

        planet.GetComponent<NetworkMatchChecker>().matchId = matchId.ToGuid();
        NetworkServer.Spawn(planet, connectionToClient);
    }

    public void SpawnPlayer(string _matchId) {
        CmdSpawnPlayer(_matchId);
    }

    [Command]
    private void CmdSpawnPlayer(string _matchId) {
        GameObject newRoomPlayer = Instantiate(UILobby.instance.roomPlayerPrefab, GameManager.instance.transform);

        newRoomPlayer.GetComponent<NetworkMatchChecker>().matchId = matchId.ToGuid();

        NetworkServer.Spawn(newRoomPlayer, connectionToClient);
    }

    /* Start game */

    public void StartGame() {
        CmdStartGame();
    }

    [Command]
    private void CmdStartGame() {
        foreach (GameObject planet in GameManager.instance.planets) {
            NetworkServer.Destroy(planet);
            Destroy(planet);
        }
        GameManager.instance.StartNewGame();
        foreach (GameObject planet in GameManager.instance.planets) {
            planet.GetComponent<NetworkMatchChecker>().matchId = matchId.ToGuid();
            NetworkServer.Spawn(planet);
        }
        foreach (GameObject token in GameManager.instance.tokens) {
            token.GetComponent<NetworkMatchChecker>().matchId = matchId.ToGuid();
            NetworkServer.Spawn(token);
        }
        MatchMaker.instance.StartGame(matchId);
        Debug.Log("Game starting");
    }

    public void StartGamePlayer() {
        TargetStartGame();
    }

    [TargetRpc]
    private void TargetStartGame() {
        Debug.Log(string.Format("Game {0} starting", matchId));
        SceneManager.LoadScene("MainScene", LoadSceneMode.Single);
    }
}
