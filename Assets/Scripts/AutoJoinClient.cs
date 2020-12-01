using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

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

    public void CreateLocal() {
        Debug.Log("Localing");
        PlayerPrefs.SetInt("isLocal", 1);
        SceneManager.LoadScene("LobbyScene", LoadSceneMode.Single);
    }

    public void JoinServer() {
        CreateLocal();
        return;
        Debug.Log("Joining");
        PlayerPrefs.SetInt("isLocal", 0);
        networkManager.networkAddress = serverAddress;
        networkManager.StartClient();
    }

    public void HostLocal() {
        Debug.Log("Hosting");
        PlayerPrefs.SetInt("isLocal", 0);
        networkManager.networkAddress = localAddress;
        networkManager.StartHost();
    }

    public void JoinLocal() {
        Debug.Log("Joining local");
        PlayerPrefs.SetInt("isLocal", 0);
        networkManager.networkAddress = localAddress;
        networkManager.StartClient();
    }
}
