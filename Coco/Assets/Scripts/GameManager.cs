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
    public GameObject parallaxObjects;
    public GameObject healthBar;
    PlayerHealth health;
    public GameObject joystickPage;
    public GameObject currentScore;
    public GameObject backgroundOne;
   
    public GameObject groundOne;
    public int startTime;
    public Text scoreText;
    private bool[] alerted = new bool[5];
    private bool[] bgChanged = new bool[5];



    // scale the game time by this much for debugging purpose
    public readonly float scaleFactor = 0.97f;
    public readonly float gameDuration = 1800f; // 30 minutes
    readonly float maxHealth = 100f;
    //private Animator anim;


    // Coco travels at 3 miles per second for 
    // 25 minutes to cover 4500 miles, the distance between Ghana and the UK
    private readonly int MILES_PER_SEC = 2;


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
    public GameObject astroidPrefab;

    // Spawning Info //
    bool spawning = true;
    [Header("Spawning")]
    public float spawnTimeMin = 1.0f;
    public float spawnTimeMax = 3.0f;
    public int startingAsteroids = 2;
    public float healthSpawnTimeMin = 15.0f;
    public float healthSpawnTimeMax = 20.0f;

    // Score //
    [Header("Score")]
    int score = 0;
    public int asteroidPoints = 50;
    public int debrisPoints = 10;

    // Game Over //
    [Header("Game Over")]

    public int asteroidHealth = 100;
    public int debrisHealth = 20;

    bool hasLost = false;

    enum PageState
    {
        None,
        Start,
        Countdown,
        GameOver,
        GameOverSuccess
    }

    enum Background {
        Leg1,
        Leg2,
        Leg3,
        Leg4,
        Leg5
    }
    enum Alerts {
        Alert1,
        Alert2,
        Alert3,
        Alert4,
        Alert5
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
        Debug.Log("Entering OnEnable");
        StartCoroutine("SpawnTimer");
        startTime = 0; //(Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        //anim = GetComponent<Animator>();
        Tap.OnPlayerDied += OnPlayerDied;
        Tap.OnPlayerScored += OnPlayerScored;
        TouchController.OnPlayerScored += OnPlayerScored;
        CountdownText.OnCountdownFinished += OnCountdownFinished;
        Debug.Log("Exiting OnEnable");




    }

    void OnDisable()
    {
        Debug.Log("Entering OnDisable");
        Tap.OnPlayerDied -= OnPlayerDied;
        Tap.OnPlayerScored -= OnPlayerScored;
        TouchController.OnPlayerScored -= OnPlayerScored;
        CountdownText.OnCountdownFinished -= OnCountdownFinished;
        Debug.Log("Exiting OnDisable");

    }

    void OnCountdownFinished()
    {
       
        Debug.Log("Entering OnCountdownFinished");
        SetPageState(PageState.None);
        OnGameStarted();
        score = 0;
        gameOver = false;
        startTime = 0; //(Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        health.UpdateHealth(maxHealth, null);
        healthBar.SetActive(true);
        Debug.Log("Exiting OnCountdownFinished");


        //startTime = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

    }
   

    void OnPlayerScored()
    {
        Debug.Log("Entering OnPlayerScored");

        if (gameOver) { return; }
        //int timeNow = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        //int elapsed = timeNow - startTime;

        score += MILES_PER_SEC;
        scoreText.text = score.ToString();
        Debug.Log("Exiting OnPlayerScored");

    }


    void OnPlayerDied()
    {
        Debug.Log("Entering OnPlayerDied");
        //if (!backgroundFive.activeInHierarchy) { gameOver = true; }
        gameOver = true;
        int savedScore = PlayerPrefs.GetInt("HighScore");
        if (score > savedScore)
        {
            PlayerPrefs.SetInt("HighScore", score);
        }
      
        joystickPage.SetActive(false);
        healthBar.SetActive(false);
        gameOverScreen.gameObject.SetActive(true);
        Debug.Log("Exiting OnPlayerDied");

    }

    void ChangeBackground(Background leg) {

        switch (leg) {
            case Background.Leg1:
                backgroundOne.SetActive(true);
                groundOne.SetActive(true);
                bgChanged[0] = true;
                break; 
        }

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
                Debug.Log("Setting page state to GAME OVER");
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
        Debug.Log("confirming game over state: replay button clicked");
        ResetObjects();
        SetPageState(PageState.Start);
        scoreText.text = "0";
        ResetObjects();
        // TODO: set background to default

        OnGameOverConfirmed();

    }

    public void StartGame()
    {
        Debug.Log("Entering StartGame...");
        //messageContainer.SetActive(false);
        //healthBar.SetActive(true);
        //currentScore.SetActive(true);
        //messageContainer.SetActive(true);
        joystickPage.SetActive(true);
        ChangeBackground(Background.Leg1);
        SetPageState(PageState.Countdown);
        Debug.Log("Exiting StartGame...");
    }

    void ResetObjects()
    {
      
        Debug.Log("Entering ResetObject...");

        foreach (GameObject obstacle in obstacles)
        {
            if (obstacle != null) {
                Destroy(obstacle);
            }
        }

        Debug.Log("Exiting ResetObject...");

    }

    void SpawnAsteroid()
    {
        if (gameOver) { return; }
        // Get a random point within spawn range
        Vector2 dir = Vector2.zero;
        dir.x = Random.Range(xSpawnRange.minX, xSpawnRange.maxX);
        dir.y = Random.Range(ySpawnRange.minY, ySpawnRange.maxY);

        // Create a Vector3 varaible to store the spawn position.
        Vector3 pos = Vector3.zero;

        // If the X value of the spawn direction is greater than the Y, then spawn the asteroid to the left or right of the screen, determined by the value of dir.
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
            pos = new Vector3(Mathf.Sign(dir.x) * Camera.main.orthographicSize * Camera.main.aspect * 1.3f, dir.y * Camera.main.orthographicSize * 1.2f, 0);
        // Else the Y value is greater than X, so spawn the asteroid up or down, determined by the value of dir.
        else
            pos = new Vector3(dir.x * Camera.main.orthographicSize * Camera.main.aspect * 1.3f, Mathf.Sign(dir.y) * Camera.main.orthographicSize * 1.2f, 0);

        // Create the asteroid game object at the position( determined above ), and at a random rotation.
        GameObject ast = Instantiate(astroidPrefab, pos, Quaternion.Euler(0, 0, 0)) as GameObject;
        obstacles.AddLast(ast);

    }


    public void ModifyScore(string scoreType = "")
    {
        if (hasLost == true)
            return;

        int scoreMod = 5;

        if (scoreType == "Asteroid")
            scoreMod = asteroidPoints;
        else if (scoreType == "Debris")
            scoreMod = debrisPoints;

        // Increase the score by the appropriate amount.
        score += scoreMod;

        // Update the score text to reflect the current score.
        UpdateScoreText();
    }

    IEnumerator SpawnTimer()
    {
        // Wait for a bit before the initial spawn.
        yield return new WaitForSeconds(0.5f);

        // For as many times as the startingAsteroids variable dictates, spawn an asteroid.
        for (int i = 0; i < startingAsteroids; i++)
            SpawnAsteroid();

        // While spawning is true...
        while (spawning)
        {
            // Wait for a range of seconds determined my the min and max variables.
            yield return new WaitForSeconds(Random.Range(spawnTimeMin, spawnTimeMax));

            // Spawn an asteroid.
            SpawnAsteroid();
        }
    }
    void UpdateScoreText()
    {
        // Set the visual score amount to reflect the current score value.
        scoreText.text = score.ToString();
    }
  


}