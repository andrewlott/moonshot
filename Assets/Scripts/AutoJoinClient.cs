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

    public void CreateLocal(float delaySeconds = 0.0f) {
        Debug.Log("Localing");
        PlayerPrefs.SetInt("isLocal", 1);
        StartCoroutine(LoadSceneAfterSeconds(delaySeconds));
    }

    public void JoinServer(float delaySeconds = 0.0f) {
        Debug.Log("Joining");
        PlayerPrefs.SetInt("isLocal", 0);
        networkManager.networkAddress = serverAddress;
        StartCoroutine(StartClientAfterSeconds(delaySeconds));
    }

    public void HostLocal(float delaySeconds = 0.0f) {
        Debug.Log("Hosting");
        PlayerPrefs.SetInt("isLocal", 0);
        networkManager.networkAddress = localAddress;
        StartCoroutine(StartHostAfterSeconds(delaySeconds));
    }

    public void JoinLocal(float delaySeconds = 0.0f) {
        Debug.Log("Joining local");
        PlayerPrefs.SetInt("isLocal", 0);
        networkManager.networkAddress = localAddress;
        StartCoroutine(StartClientAfterSeconds(delaySeconds));
    }

    IEnumerator LoadSceneAfterSeconds(float seconds) {
        yield return new WaitForSeconds(seconds);
        SceneManager.LoadScene("LobbyScene", LoadSceneMode.Single);
    }

    IEnumerator StartClientAfterSeconds(float seconds) {
        yield return new WaitForSeconds(seconds);
        networkManager.StartClient();
    }

    IEnumerator StartHostAfterSeconds(float seconds) {
        yield return new WaitForSeconds(seconds);
        networkManager.StartHost();
    }
}
