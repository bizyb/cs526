﻿using UnityEngine;
using UnityEngine.UI;


// TODO: score refers to distance travel; final text should say distance travelled
// instead of 'Score' and Highest Score should change to Longest Journey
public class GameManager : MonoBehaviour
{

    public delegate void GameDelegate();
    public static event GameDelegate OnGameStarted;
    public static event GameDelegate OnGameOverConfirmed;

    public static GameManager Instance;

    public GameObject startPage;
    public GameObject gameOverPage;
    public GameObject countdownPage;
    public Text scoreText;

    enum PageState
    {
        None,
        Start,
        Countdown,
        GameOver
    }

    int score = 0;
    bool gameOver;

    public bool GameOver { get { return gameOver; } }
    public int Score { get { return score; }}

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void OnEnable()
    {
        Tap.OnPlayerDied += OnPlayerDied;
        Tap.OnPlayerScored += OnPlayerScored;
        CountdownText.OnCountdownFinished += OnCountdownFinished;
    }

    void OnDisable()
    {
        Tap.OnPlayerDied -= OnPlayerDied;
        Tap.OnPlayerScored -= OnPlayerScored;
        CountdownText.OnCountdownFinished -= OnCountdownFinished;
    }

    void OnCountdownFinished()
    {
        SetPageState(PageState.None);
        OnGameStarted();
        score = 0;
        gameOver = false;
    }

    void OnPlayerScored()
    {
        score++;
        scoreText.text = score.ToString();
    }

    void OnPlayerDied()
    {
        gameOver = true;
        int savedScore = PlayerPrefs.GetInt("HighScore");
        if (score > savedScore)
        {
            PlayerPrefs.SetInt("HighScore", score);
        }
        SetPageState(PageState.GameOver);
    }

    void SetPageState(PageState state)
    {
        switch (state)
        {
            case PageState.None:
                startPage.SetActive(false);
                gameOverPage.SetActive(false);
                countdownPage.SetActive(false);
                break;
            case PageState.Start:
                startPage.SetActive(true);
                gameOverPage.SetActive(false);
                countdownPage.SetActive(false);
                break;
            case PageState.Countdown:
                startPage.SetActive(false);
                gameOverPage.SetActive(false);
                countdownPage.SetActive(true);
                break;
            case PageState.GameOver:
                startPage.SetActive(false);
                gameOverPage.SetActive(true);
                countdownPage.SetActive(false);
                break;
        }
    }

    public void ConfirmGameOver()
    {

        SetPageState(PageState.Start);
        scoreText.text = "0";
        OnGameOverConfirmed();
    }

    public void StartGame()
    {
        SetPageState(PageState.Countdown);
    }

}