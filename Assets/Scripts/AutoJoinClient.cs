using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AutoJoinClient : MonoBehaviour {
    [SerializeField] private NetworkManager networkManager;
    [SerializeField] string serverAddress = "moonshot.andrewlott.com";
    [SerializeField] string localAddress = "localhost";

    private void Start() {
        if (!Application.isBatchMode) {
            Debug.Log("Client started");
        } else {
            // Headless server
            Debug.Log("Server started");
        }
    }

    public void JoinServer() {
        Debug.Log("Joining");
        networkManager.networkAddress = serverAddress;
        networkManager.StartClient();
    }

    public void HostLocal() {
        Debug.Log("Hosting");
        networkManager.networkAddress = localAddress;
        networkManager.StartHost();
    }

    public void JoinLocal() {
        Debug.Log("Joining local");
        networkManager.networkAddress = localAddress;
        networkManager.StartClient();
    }
}
