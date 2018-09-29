using UnityEngine;
using UnityEngine.UI;
using System;



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
    public int startTime;
    public Text scoreText;


    // Coco travels at 3 miles per second for 
    // 25 minutes to cover 4500 miles, the distance between Ghana and the UK
    public int MILES_PER_SEC = 2; 

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

            //OnCountdownFinished();

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
        startTime = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;



        // update distance travelled, i.e. the score, every 1 second
        InvokeRepeating("OnPlayerScored", 1f, 1f);
    }

    void OnPlayerScored()
    {
       
        score += MILES_PER_SEC;
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