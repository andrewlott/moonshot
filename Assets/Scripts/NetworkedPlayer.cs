using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkedPlayer : NetworkBehaviour {
    public override void OnStartServer() {
        base.OnStartServer();

        GameObject gameManagerObject = GameObject.FindGameObjectWithTag("GameManager");
        if (gameManagerObject == null) {
            return;
        }

        GameManager gm = gameManagerObject.GetComponent<GameManager>();
        if (!gm.players.Contains(gameObject)) {
            gm.players.Add(gameObject);
        }
        transform.position = gm.RandomPointOnPlanet(gm.planets[Random.Range(0, gm.planets.Count)]);
    }

    public override void OnStartClient() {
        base.OnStartClient();

        if (!hasAuthority) {
            Jumpable j = GetComponent<Jumpable>();
            j.shouldJump = false;
            j.shouldPush = false;

            Walkable w = GetComponent<Walkable>();
            w.isEnabled = false;
        }
    }
}
