using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkedPlayer : NetworkBehaviour {
    public override void OnStartServer() {
        base.OnStartServer();

        GameManager gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
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
        }
    }
}
