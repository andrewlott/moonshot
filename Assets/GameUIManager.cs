using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class GameUIManager : MonoBehaviour {
    [Header("Main Game")]
    [SerializeField] private GameObject gameUI;
    [SerializeField] private Animator startEndAnimator;
    [SerializeField] private Text clockText;
    [SerializeField] private List<GameObject> playerUI;
    [SerializeField] private List<Image> playerColors;
    [SerializeField] private List<Text> playerStars;

    [Header("End Game")]
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private List<GameObject> gameOverPlayerUI;
    [SerializeField] private List<GameObject> gameOverPlayerPlacesUI;
    [SerializeField] private List<Image> gameOverPlayerColors;
    [SerializeField] private List<Text> gameOverPlayerStars;

    private bool gameOver = false;

    void Start() {
        gameUI.SetActive(true);
        gameOverUI.SetActive(false);
        startEndAnimator.SetTrigger("gameStart");

        foreach(GameObject g in playerUI) {
            g.SetActive(false);
        }
        for (int i = 0; i < GameManager.instance.players.Count; i++) {
            GameObject player = GameManager.instance.players[i];
            playerUI[i].SetActive(true);
            playerColors[i].gameObject.SetActive(true);
            playerColors[i].gameObject.transform.parent.gameObject.SetActive(true);
            playerColors[i].color = player.GetComponent<Colorable>().color;
            Bank b = player.GetComponent<Banker>().bank;
            int amount = 0;
            if (b != null) {
                amount = b.tokens;
            }
            playerStars[i].text = amount.ToString();
        }
    }

    private void FixedUpdate() {
        if (gameOver) {
            return;
        }
        int timeRemaining = Mathf.Max(0, GameManager.instance.gameDurationSeconds - Mathf.RoundToInt(Time.time - GameManager.instance.startTime));
        clockText.text = string.Format("TIME {0}:{1}", timeRemaining / 60, (timeRemaining % 60).ToString("d2"));

        for (int i = 0; i < GameManager.instance.players.Count; i++) {
            GameObject player = GameManager.instance.players[i];
            Bank b = player.GetComponent<Banker>().bank;
            int amount = 0;
            if (b != null) {
                amount = b.tokens;
            }
            string amountString = amount.ToString();
            if (playerStars[i].text != amountString) {
                playerStars[i].text = amountString;
            }
        }

        if (timeRemaining == 0 && !gameOver) {
            gameOver = true;
            startEndAnimator.SetTrigger("gameEnd");
            StartCoroutine(GameOverAfterSeconds(2));
        }
    }

    private void GameOver() {
        gameUI.SetActive(false);
        gameOverUI.SetActive(true);

        foreach (GameObject g in gameOverPlayerUI) {
            g.SetActive(false);
        }
        foreach (GameObject g in gameOverPlayerPlacesUI) {
            g.SetActive(false);
        }

        List<GameObject> sortedPlayers = new List<GameObject>(GameManager.instance.players);
        sortedPlayers = sortedPlayers.OrderBy(p => -p.GetComponent<Banker>().bank.tokens).ToList();

        for (int i = 0; i < sortedPlayers.Count; i++) {
            GameObject player = sortedPlayers[i];

            gameOverPlayerUI[i].SetActive(true);
            gameOverPlayerPlacesUI[i].SetActive(true);
            gameOverPlayerColors[i].gameObject.SetActive(true);
            gameOverPlayerColors[i].gameObject.transform.parent.gameObject.SetActive(true);
            gameOverPlayerColors[i].color = player.GetComponent<Colorable>().color;
            Bank b = player.GetComponent<Banker>().bank;
            int amount = 0;
            if (b != null) {
                amount = b.tokens;
            }
            gameOverPlayerStars[i].text = amount.ToString();
        }
    }

    IEnumerator GameOverAfterSeconds(float seconds) {
        yield return new WaitForSeconds(seconds);
        GameOver();
    }

    public void BackPressed() {
        GameManager.instance.ClearAll();
        SceneManager.LoadScene("LobbyScene", LoadSceneMode.Single);
    }

    public void RematchPressed() {
        GameManager.instance.ClearAllNonPlayers();
        SceneManager.LoadScene("MainScene", LoadSceneMode.Single);
        GameManager.instance.StartNewGame();
    }
}
