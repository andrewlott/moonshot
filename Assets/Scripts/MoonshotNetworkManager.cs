using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MoonshotNetworkManager : NetworkManager {
    GameManager gameManager;

    public override void OnStartServer() {
        base.OnStartServer();
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

        gameManager.GenerateRandomPlanets();
        foreach (GameObject planet in gameManager.planets) {
            NetworkServer.Spawn(planet);
            foreach (GameObject wp in planet.GetComponent<WrappablePlanet>().wrappedPlanets) {
                //NetworkServer.Spawn(wp, conn);
            }
        }

        gameManager.GenerateTokens();
        foreach (GameObject token in gameManager.tokens) {
            NetworkServer.Spawn(token);
        }
    }

    public override void OnStopServer() {
        foreach (GameObject planet in gameManager.planets) {
            NetworkServer.Destroy(planet);
            foreach (GameObject wp in planet.GetComponent<WrappablePlanet>().wrappedPlanets) {
                //NetworkServer.Destroy(wp);
            }
            Destroy(planet); // also destroys the wp
        }
        foreach (GameObject token in gameManager.tokens) {
            NetworkServer.Destroy(token);
            Destroy(token);
        }
        foreach (GameObject player in gameManager.players) {
            NetworkServer.Destroy(player);
            Destroy(player);
        }

        base.OnStopServer();
    }
}
