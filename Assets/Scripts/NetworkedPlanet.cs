using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkedPlanet : NetworkBehaviour {
    public override void OnStartServer() {
        base.OnStartServer();

        GameObject gameManagerObject = GameObject.FindGameObjectWithTag("GameManager");
        if (gameManagerObject == null) {
            return;
        }

        GameManager gm = gameManagerObject.GetComponent<GameManager>();
        if (!gm.planets.Contains(gameObject)) {
            gm.planets.Add(gameObject);
        }
    }

}
