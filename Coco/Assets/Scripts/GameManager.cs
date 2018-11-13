using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{

    public delegate void GameDelegate();
    public static event GameDelegate OnGameStarted;
    public static event GameDelegate OnGameOverConfirmed;

    public static GameManager Instance;

    public GameObject startPage;
    public GameObject gameOverPage;
    public GameObject gameOverSuccessPage;
    public GameObject howtoPlayPage;
    public GameObject aboutPage;
    public GameObject creditsPage;
    public GameObject countdownPage;
    public GameObject bird;
    public GameObject healthBar;
    PlayerHealth health;
    public GameObject joystickPage;
    public GameObject currentScore;
    public GameObject background;
    public GameObject ground;
    public Text scoreText;
    public GameObject plusPrefab;
    public GameObject heartPrefab;
    public Text gameScore;
    public Rigidbody2D birdRigidBody;



    AudioController audioController;

   
    readonly float maxHealth = 100f;
    readonly int obstacleBonus = 1;
    readonly float healthBonusOnCoin = 25f;
    float timeSince = 0;

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
    public GameObject eagle;
    public GameObject goose;
    public GameObject dragonOne;
    public GameObject dragonTwo;
    public GameObject jellyfish;
    public GameObject helicopter;
    public GameObject rewardPrefab;
    int startingObstacles = 1;

    // Spawning Info //
    bool spawning = true;
    [Header("Spawning")]
    public float spawnTimeMin;
    public float spawnTimeMax;
    public float rewardSpawnTimeMin;
    public float rewardSpawnTimeMax;

    // Score //
    int score = 0;
    readonly float difficultyInterval = 30f;

    enum PageState
    {
        None,
        Start,
        Countdown,
        GameOver,
        GameOverSuccess
    }


    bool gameOver;
    bool finalLeg;
    bool gameStarted;
    int heliCount = 0;
    //bool stopAllAudio;
    public bool GameStarted { get { return gameStarted; } }
    public bool GameOver { get { return gameOver; } }
    //public bool StopAllAudio { get { return stopAllAudio; }}
    public int HeliCount { get { return heliCount; } set { heliCount = value; } }
    public int Score { get { return score; } }

    public bool FinalLeg { get { return finalLeg; } set { finalLeg = value; } }

    LinkedList<GameObject> obstacles;
    LinkedList<GameObject> rewards;

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
        rewards = new LinkedList<GameObject>();
        audioController = AudioController.Instance;
    }
    void OnEnable()
    {
        SetPageState(PageState.Start);
        bird.SetActive(false);
        joystickPage.SetActive(false);
        healthBar.SetActive(false);
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
    
        SetPageState(PageState.None);
        OnGameStarted();
        score = 0;
        gameOver = false;
        gameStarted = true;
        health.UpdateHealth(maxHealth, null);
        joystickPage.SetActive(true);
        healthBar.SetActive(true);
        StartCoroutine("ObstacleSpawnTimer");
        StartCoroutine("RewardSpawnTimer");
        StartCoroutine("Difficulty");
        heliCount = 0;
        startingObstacles = 1;
    }


    void OnPlayerScored(string optional, Vector3 pos)
    {
      
        if (gameOver) { return; }
        if (optional == "Coin")
        {
            SpawnRewardAnim(heartPrefab, pos);
            audioController.AudioOnReward();
            health.UpdateHealth(healthBonusOnCoin, null);
        }
        else
        {
            score += obstacleBonus;
            SpawnRewardAnim(plusPrefab, pos);
            audioController.AudioOnScore();
        }
        scoreText.text = score.ToString();

       
    }


    void OnPlayerDied(string optional, Vector3 pos)
    {

        gameStarted = false;
        gameOver = true;
        int savedScore = PlayerPrefs.GetInt("HighScore");
        if (score > savedScore)
        {
            PlayerPrefs.SetInt("HighScore", score);
        }

        joystickPage.SetActive(false);
        healthBar.SetActive(false);
        gameOverScreen.gameObject.SetActive(true);
        gameScore.text = score.ToString();

    }

    void SpawnRewardAnim(GameObject prefab, Vector3 pos) {

        Instantiate(prefab, pos, Quaternion.Euler(0, 0, 0));

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
       
        ResetObjects();
        SetPageState(PageState.Start);
        scoreText.text = "0";
        ResetObjects();
        OnGameOverConfirmed();
        bird.SetActive(false);

    }

    public void StartGame()
    {
       
        bird.SetActive(true);
        birdRigidBody.simulated = false;
        joystickPage.SetActive(true);
        SetBackground();
        SetPageState(PageState.Countdown);
    }

    void ResetObjects()
    {
      
       
        foreach (GameObject obstacle in obstacles)
        {
            if (obstacle != null) {
                Destroy(obstacle);
            }
        }

        foreach (GameObject reward in rewards)
        {
            if (reward != null)
            {
                Destroy(reward);
            }
        }

       

    }

    void SpawnObstacle()
    {
        //Debug.Log("Spawning obstcles: " + startingObstacles);
        //Debug.Log("=====================================");
        if (gameOver) { return; }

        // Each level has at most three different types of obstacles
        // Randomly select which one to spawn
       
        GameObject[] prefabs = { eagle, goose, dragonOne, dragonTwo, jellyfish, helicopter };
        GameObject obstaclePrefab = prefabs.RandomItem();

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
        if (obstaclePrefab == helicopter) {
            heliCount++;
            audioController.AudioOnHelicopter();
        }

    }

    void SpawnReward()
    {
        if (gameOver) { return; }
        Vector2 dir = Vector2.zero;
        dir.x = Random.Range(xSpawnRange.minX, xSpawnRange.maxX);
        dir.y = Random.Range(ySpawnRange.minY, ySpawnRange.maxY);
        Vector3 pos = new Vector3(dir.x, dir.y, 0);


        GameObject reward = Instantiate(rewardPrefab, pos, Quaternion.Euler(0, 0, 0)) as GameObject;
        rewards.AddLast(reward);

    }

    IEnumerator ObstacleSpawnTimer()
    {

        // While spawning is true...
        // Spawn obstacles when the game starts
        SpawnObstacle();

        // Spawn obstacles at a set interval, where the number of starting obstacles
        // increases every x seconds, where x is defined 
        while (true)
        {
            // Wait for a range of seconds determined my the min and max variables.
            yield return new WaitForSeconds(Random.Range(spawnTimeMin, spawnTimeMax));

            for (int i = 0; i < startingObstacles; i++)
            {
                SpawnObstacle();
            }
        }
    }

    IEnumerator RewardSpawnTimer()
    {

        SpawnReward();
        while (spawning)
        {
            // Wait for a range of seconds determined my the min and max variables.
            yield return new WaitForSeconds(Random.Range(rewardSpawnTimeMin, rewardSpawnTimeMax));
            SpawnReward();
        }
    }

    IEnumerator Difficulty() {

        while (true) {
            yield return new WaitForSeconds(difficultyInterval);
            startingObstacles++;
            //timeSince += difficultyInterval;
            //Debug.Log("obstacle count: " + startingObstacles);
            //Debug.Log("Time since beginning: " + timeSince);
            //Debug.Log("=====================================");


        }

    }

    // TODO: there's gotta be a better way to do this
    public void CloseHowTo() {
        howtoPlayPage.SetActive(false);
    }
    public void CloseAbout()
    {
       
        aboutPage.SetActive(false);
    }
    public void CloseCredits()
    {
        creditsPage.SetActive(false);
    }
    public void OpenHowTo() {
        howtoPlayPage.SetActive(true);
    }
    public void OpenAbout()
    {
        aboutPage.SetActive(true);
    }
    public void OpenCredits()
    {
        creditsPage.SetActive(true);
    }
}
public static class ArrayExtensions
{
    // This is an extension method. RandomItem() will now exist on all arrays.
    public static T RandomItem<T>(this T[] array)
    {
        return array[Random.Range(0, array.Length)];
    }
}