using UnityEngine;
using UnityEngine.UI;
//using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{

    public delegate void GameDelegate();
    public static event GameDelegate OnGameStarted;
    public static event GameDelegate OnGameOverConfirmed;

    public static GameManager Instance;

    public GameObject startPage;
    public GameObject gameOverPage;
    public GameObject gameOverSuccessPage;
    public GameObject countdownPage;
    public GameObject bird;
    public GameObject healthBar;
    PlayerHealth health;
    public GameObject joystickPage;
    public GameObject currentScore;
    public GameObject background;
    public GameObject ground;
    public int startTime;
    public Text scoreText;


    // scale the game time by this much for debugging purpose
    public readonly float scaleFactor = 0.97f;
    public readonly float gameDuration = 1800f; // 30 minutes
    readonly float maxHealth = 100f;
  
    [System.Serializable]
    public struct YSpawnRange
    {
        public float minY;
        public float maxY;
    }

    [System.Serializable]
    public struct XSpawnRange
    {
        public float minX;
        public float maxX;
    }
    public YSpawnRange ySpawnRange;
    public XSpawnRange xSpawnRange;

    public Image gameOverScreen;
    public Text gameOverText;



    // Prefabs //
    [Header("Prefabs")]
    public GameObject obstaclePrefab;
    public int startingObstacles = 5;

    // Spawning Info //
    bool spawning = true;
    [Header("Spawning")]
    public float spawnTimeMin = 1.0f;
    public float spawnTimeMax = 3.0f;

    // Score //
    int score = 0;


    enum PageState
    {
        None,
        Start,
        Countdown,
        GameOver,
        GameOverSuccess
    }


    //int score = 0;
    bool gameOver;
    bool finalLeg;

    public bool GameOver { get { return gameOver; } }
    public int Score { get { return score; }}

    public bool FinalLeg { get { return finalLeg; } set { finalLeg = value; }}

    LinkedList<GameObject> obstacles;

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
    private void Start()
    {
        // TODO: store the obstacles in a hash table; 
        health = PlayerHealth.Instance;
        obstacles = new LinkedList<GameObject>();
    }
    void OnEnable()
    {
   
        StartCoroutine("SpawnTimer");
        startTime = 0; //(Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        Tap.OnPlayerDied += OnPlayerDied;
        TouchController.OnPlayerScored += OnPlayerScored;
        CountdownText.OnCountdownFinished += OnCountdownFinished;




    }

    void OnDisable()
    {

        Tap.OnPlayerDied -= OnPlayerDied;
        TouchController.OnPlayerScored -= OnPlayerScored;
        CountdownText.OnCountdownFinished -= OnCountdownFinished;

    }

    void OnCountdownFinished()
    {
       
        //Debug.Log("Entering OnCountdownFinished");
        SetPageState(PageState.None);
        OnGameStarted();
        score = 0;
        gameOver = false;
        startTime = 0; //(Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        health.UpdateHealth(maxHealth, null);
        healthBar.SetActive(true);
        //Debug.Log("Exiting OnCountdownFinished");


        //startTime = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

    }
   

    void OnPlayerScored(string optional)
    {
        //Debug.Log("Entering OnPlayerScored");

        if (gameOver) { return; }
        //int timeNow = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        //int elapsed = timeNow - startTime;

        score++;
        scoreText.text = score.ToString();
        //Debug.Log("Exiting OnPlayerScored");

    }


    void OnPlayerDied(string optional)
    {
        //Debug.Log("Entering OnPlayerDied");
        gameOver = true;
        int savedScore = PlayerPrefs.GetInt("HighScore");
        if (score > savedScore)
        {
            PlayerPrefs.SetInt("HighScore", score);
        }
      
        joystickPage.SetActive(false);
        healthBar.SetActive(false);
        gameOverScreen.gameObject.SetActive(true);
        //Debug.Log("Exiting OnPlayerDied");

    }

    void SetBackground() {

        background.SetActive(true);
        ground.SetActive(true);
    }

    void SetPageState(PageState state)
    {
        switch (state)
        {
            case PageState.None:
                startPage.SetActive(false);
                gameOverPage.SetActive(false);
                countdownPage.SetActive(false);
                gameOverSuccessPage.SetActive(false);
                break;
            case PageState.Start:
                startPage.SetActive(true);
                gameOverPage.SetActive(false);
                countdownPage.SetActive(false);
                gameOverSuccessPage.SetActive(false);
                break;
            case PageState.Countdown:
                startPage.SetActive(false);
                gameOverPage.SetActive(false);
                countdownPage.SetActive(true);
                gameOverSuccessPage.SetActive(false);
                break;
            case PageState.GameOver:
                //Debug.Log("Setting page state to GAME OVER");
                startPage.SetActive(false);
                gameOverPage.SetActive(true);
                countdownPage.SetActive(false);
                gameOverSuccessPage.SetActive(false);
                break;
            case PageState.GameOverSuccess:
                startPage.SetActive(false);
                gameOverPage.SetActive(false);
                countdownPage.SetActive(false);
                gameOverSuccessPage.SetActive(true);
                break;
        }
    }

    public void ConfirmGameOver()
    {
        //Debug.Log("confirming game over state: replay button clicked");
        ResetObjects();
        SetPageState(PageState.Start);
        scoreText.text = "0";
        ResetObjects();
        OnGameOverConfirmed();

    }

    public void StartGame()
    {
        //Debug.Log("Entering StartGame...");
        joystickPage.SetActive(true);
        SetBackground();
        SetPageState(PageState.Countdown);
        //Debug.Log("Exiting StartGame...");
    }

    void ResetObjects()
    {
      
        //Debug.Log("Entering ResetObject...");

        foreach (GameObject obstacle in obstacles)
        {
            if (obstacle != null) {
                Destroy(obstacle);
            }
        }

        //Debug.Log("Exiting ResetObject...");

    }

    void SpawnObstacle()
    {
        if (gameOver) { return; }
        Vector2 dir = Vector2.zero;
        dir.x = Random.Range(xSpawnRange.minX, xSpawnRange.maxX);
        dir.y = Random.Range(ySpawnRange.minY, ySpawnRange.maxY);
        Vector3 pos = Vector3.zero;

        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
            pos = new Vector3(Mathf.Sign(dir.x) * Camera.main.orthographicSize * Camera.main.aspect * 1.3f, dir.y * Camera.main.orthographicSize * 1.2f, 0);
        else
            pos = new Vector3(dir.x * Camera.main.orthographicSize * Camera.main.aspect * 1.3f, Mathf.Sign(dir.y) * Camera.main.orthographicSize * 1.2f, 0);


        GameObject obst = Instantiate(obstaclePrefab, pos, Quaternion.Euler(0, 0, 0)) as GameObject;
        obstacles.AddLast(obst);

    }

    IEnumerator SpawnTimer()
    {
        // Wait for a bit before the initial spawn.
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < startingObstacles; i++)
            SpawnObstacle();

        // While spawning is true...
        while (spawning)
        {
            // Wait for a range of seconds determined my the min and max variables.
            yield return new WaitForSeconds(Random.Range(spawnTimeMin, spawnTimeMax));

            // Spawn an asteroid.
            SpawnObstacle();
        }
    }
}