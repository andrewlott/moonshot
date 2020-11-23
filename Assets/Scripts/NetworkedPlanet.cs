using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkedPlanet : NetworkBehaviour {
    public override void OnStartServer() {
        base.OnStartServer();

        //Rigidbody2D rb = GetComponent<Rigidbody2D>();
        //rb.simulated = true;
        //foreach (GameObject wp in GetComponent<WrappablePlanet>().wrappedPlanets) {
        //    wp.GetComponent<Rigidbody2D>().simulated = true;
        //}
    }
}
