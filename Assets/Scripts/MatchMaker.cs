using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

[System.Serializable]
public class SyncListGameObject : SyncList<GameObject> { }
[System.Serializable]
public class SyncListMatch : SyncList<Match> { }

[System.Serializable]
public class Match {
    public string matchId;
    public SyncListGameObject players = new SyncListGameObject();

    public Match() { } // required for serializable
    public Match(string _matchId, GameObject hostPlayer) {
        matchId = _matchId;
        players.Add(hostPlayer);
    }
}

public class MatchMaker : NetworkBehaviour {
    public static MatchMaker instance;
    public SyncListMatch matches = new SyncListMatch();
    public SyncListString matchIds = new SyncListString();

    private static int idLength = 5;
    private static int maxPlayers = 4;

    private void Start() {
        instance = this;
    }

    public static string GetRandomMatchId() {
        string matchId = string.Empty;

        for (int i = 0; i < idLength; i++) {
            int rand = Random.Range(0, 36);
            if (rand < 26) {
                matchId += (char)(rand + ((int)'A'));
            } else {
                matchId += (rand - 26).ToString();
            }
        }
        Debug.Log(string.Format("Random matchID: {0}", matchId));
        return matchId;
    }

    private Match GetMatch(string matchId) {
        foreach(Match match in matches) { 
            if (match.matchId.ToLower() == matchId.ToLower()) {
                return match;
            }
        }
        return null;
    }

    public bool HostGame(string matchId, GameObject hostPlayer, out int playerIndex) {
        if (matchIds.Contains(matchId)) {
            Debug.Log("Match ID already in use");
            playerIndex = -1;
            return false;
        }
        matchIds.Add(matchId);
        Match match = new Match(matchId, hostPlayer);
        matches.Add(match);
        playerIndex = match.players.Count - 1;
        Debug.Log("Match ID works");
        return true;
    }

    public bool JoinGame(string matchId, GameObject joinPlayer, out int playerIndex) {
        if (!matchIds.Contains(matchId)) {
            Debug.Log("Match ID not found");
            playerIndex = -1;
            return false;
        }

        Match match = GetMatch(matchId);
        if (match.players.Count >= maxPlayers) {
            Debug.Log("Max players in match");
            playerIndex = -1;
            return false;
        }
        match.players.Add(joinPlayer);
        playerIndex = match.players.Count - 1;
        Debug.Log("Match joined");
        return true;
    }

    public void StartGame(string matchId) {
        Match match = GetMatch(matchId);
        foreach(GameObject playerGameObject in match.players) {
            LobbyPlayer lobbyPlayer = playerGameObject.GetComponent<LobbyPlayer>();
            lobbyPlayer.StartGamePlayer();
        }
    }
}
