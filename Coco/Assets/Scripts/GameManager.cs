using UnityEngine;
using UnityEngine.UI;
//using System;
using System.Collections;
using UnityEngine.SceneManagement;



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
    public GameObject gameOverSuccessPage;
    public GameObject countdownPage;
    public GameObject bird;
    public GameObject parallaxObjects;
    public GameObject healthBar;

    public GameObject currentScore;
    public GameObject backgroundOne;
    //public GameObject backgroundTwo;
    //public GameObject backgroundThree;
    //public GameObject backgroundFour;
    //public GameObject backgroundFive;
    public GameObject groundOne;
    //public GameObject groundTwo;
    //public GameObject groundThree;
    //public GameObject groundFour;
    //public GameObject groundFive;
    //public GameObject messageContainer;
    //public Text message;
    public int startTime;
    public Text scoreText;
    private bool[] alerted = new bool[5];
    private bool[] bgChanged = new bool[5];



    // scale the game time by this much for debugging purpose
    public readonly float scaleFactor = 0.97f;
    public readonly float gameDuration = 1800f; // 30 minutes
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
    //public GameObject debrisPrefab;
    //public GameObject explosionPrefab;
    //public GameObject healthPickupPrefab;

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
    //public Text scoreText;
    //public Text finalScoreText;
    int score = 0;
    public int asteroidPoints = 50;
    public int debrisPoints = 10;

    // Game Over //
    [Header("Game Over")]
    //public Image gameOverScreen;
    //public Text gameOverText;

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
        Debug.Log("Entering OnEnable");
        StartCoroutine("SpawnTimer");
        startTime = 0; //(Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        //anim = GetComponent<Animator>();
        Tap.OnPlayerDied += OnPlayerDied;
        Tap.OnPlayerScored += OnPlayerScored;
        TouchController.OnPlayerScored += OnPlayerScored;
        CountdownText.OnCountdownFinished += OnCountdownFinished;



        // update distance travelled, i.e. the score, every 1 second

        //SetPageState(PageState.Start);
        //ResetObjects();
        //InvokeRepeating("OnPlayerScored", 1f, 1f); // remve after debuggin and enable the one in OnCountDownFinished
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
        //InvokeRepeating("OnPlayerScored", 1f, 1f); //this is the right place, not OnEnable
        Debug.Log("Exiting OnCountdownFinished");


        //startTime = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

    }
   


    /*
     * Scoring is based on time travelled. For every second spent travelling, 
     * increment the distance by MILES_PER_SEC. In addition, alert the player 
     * at certain points in the game about an approaching completion of the 
     * current leg of the race. Changes are made according to the given schedule.
     * Alerts should be displayed before the end of the current leg and removed 
     * right before the background change.
     * 
     * The schedule (timesteps):
     * Leg1: 0-750 (Ghana-Algeria)
     * Leg2: 750-1200 (Algeria-Spain)
     * Leg3: 1200-1500 (Spain-France)
     * Leg4: 1500-1700 (France-England)
     * Leg5: 1700-1800 (England to the castle)
     * 
     * Alerts:
     *  Alert1: 0-50: Leg 1: Ghana to Algeria
     *  Alert2: 700-750: Approaching Leg2: Algeria to Spain
     *  Alert3: 1150-1200: Approaching Leg 3: Spain to France
     *  Alert4: 1450-1500: Approaching Leg 4: France to England
     *  Alert5: 1650-1700: Approaching Leg 5: Destination
     *  
    */
    void OnPlayerScored()
    {
        Debug.Log("Entering OnPlayerScored");

        //Debug.Log("game over: " + gameOver);

        //string str = UnityEngine.StackTraceUtility.ExtractStackTrace();
        //Debug.Log(str);

        if (gameOver) { return; }
        //int timeNow = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        //int elapsed = timeNow - startTime;

        score += MILES_PER_SEC;
        scoreText.text = score.ToString();

        // Alert the player 
        //if (elapsed >= 0 && elapsed < 50 && !alertSent) {
        //    AlertPlayer(Alerts.Alert1); 
        //}
        //else if (elapsed >= 700 && elapsed < 750 && !alertSent) {
        //    AlertPlayer(Alerts.Alert2); 
        //}
        //else if (elapsed >= 1150 && elapsed < 1200 && !alertSent) {
        //    AlertPlayer(Alerts.Alert3);
        //}
        //else if (elapsed >= 1450 && elapsed < 1500 && !alertSent) {
        //    AlertPlayer(Alerts.Alert4);
        //}
        //else if (elapsed >= 1650 && !alertSent) {
        //    AlertPlayer(Alerts.Alert5);
        //}

        //if (elapsed >= 0 && elapsed < 750 && !backgroundChanged) {
        //    ChangeBackground(Background.Leg1);
        //}
        //else if (elapsed >= 750 && elapsed < 1200 && !backgroundChanged)
        //{
        //    ChangeBackground(Background.Leg2);
        //}
        //else if (elapsed >= 1200 && elapsed < 1500 && !backgroundChanged)
        //{
        //    ChangeBackground(Background.Leg3);
        //}
        //else if (elapsed >= 1500 && elapsed < 1700 && !backgroundChanged)
        //{
        //    ChangeBackground(Background.Leg4);
        //}
        //else if (elapsed >= 1700 && !backgroundChanged)
        //{
        //    ChangeBackground(Background.Leg5);
        //}

        // use boolean flags to avoid having to make multiple calls to the 
        // functions
        //if (elapsed >= 0 && elapsed < 5 && !alerted[0])
        //{
        //    AlertPlayer(Alerts.Alert1);

        //}
        //else if (elapsed >= 5 && elapsed < 10 && !alerted[1])
        //{
        //    AlertPlayer(Alerts.Alert2);

        //}
        //else if (elapsed >= 10 && elapsed < 15 && !alerted[2])
        //{
        //    AlertPlayer(Alerts.Alert3);
           
        //}
        //else if (elapsed >= 15 && elapsed < 20 && !alerted[3])
        //{
        //    AlertPlayer(Alerts.Alert4);

        //}
        //else if (elapsed >= 20 && !alerted[4])
        //{
        //    AlertPlayer(Alerts.Alert5);
           
        //}

        //if (elapsed >= 0 && elapsed < 5 && !bgChanged[0])
        //{
        //    ChangeBackground(Background.Leg1);

        //}
        //else if (elapsed >= 5 && elapsed < 10 && !bgChanged[1])
        //{
        //    ChangeBackground(Background.Leg2);
           
        //}
        //else if (elapsed >= 10 && elapsed < 20 && !bgChanged[2])
        //{
        //    ChangeBackground(Background.Leg3);
           
        //}
        //else if (elapsed >= 20 && elapsed < 25 && !bgChanged[3])
        //{
        //    ChangeBackground(Background.Leg4);
           
        //}
        //else if (elapsed >= 25 && !bgChanged[4])
        //{
            //ChangeBackground(Background.Leg5);
           
        //}

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
        //StartCoroutine("DelayedTransition");
        //SetPageState(PageState.GameOver);
        //CancelInvoke();
        ShowDeathScreen();
        Debug.Log("Exiting OnPlayerDied");

    }


    //void AlertPlayer(Alerts alert) {
    //    //TODO: play sound 

    //    String s = "";
    //    switch (alert) {
    //        case Alerts.Alert1:
    //            s = "Leg 1: Ghana to Algeria";
    //            alerted[0] = true;
    //            break;
    //        case Alerts.Alert2:
    //            s = "Approaching Leg2: Algeria to Spain";
    //            alerted[1] = true;
    //            break;
    //        case Alerts.Alert3:
    //            s = "Approaching Leg 3: Spain to France";
    //            alerted[2] = true;
    //            break;
    //        case Alerts.Alert4:
    //            s = "Approaching Leg 4: France to England";
    //            alerted[3] = true;
    //            break;
    //        case Alerts.Alert5:
    //            s = "Approaching Leg 5: Destination";
    //            alerted[4] = true;
    //            break;
    //    }
    //    messageContainer.SetActive(true);
    //    message.text = s;

    //}

    void ChangeBackground(Background leg) {

        switch (leg) {
            case Background.Leg1:
                backgroundOne.SetActive(true);
                //backgroundTwo.SetActive(false);
                //backgroundThree.SetActive(false);
                //backgroundFour.SetActive(false);
                //backgroundFive.SetActive(false);

                groundOne.SetActive(true);
                //groundTwo.SetActive(false);
                //groundThree.SetActive(false);
                //groundFour.SetActive(false);
                //groundFive.SetActive(false);

                bgChanged[0] = true;
                break;
            //case Background.Leg2:
                //backgroundOne.SetActive(false);
                //backgroundTwo.SetActive(true);
                //backgroundThree.SetActive(false);
                //backgroundFour.SetActive(false);
                //backgroundFive.SetActive(false);

                //groundOne.SetActive(false);
                //groundTwo.SetActive(true);
                //groundThree.SetActive(false);
                //groundFour.SetActive(false);
                //groundFive.SetActive(false);

                //bgChanged[1] = true;
                //break;
            //case Background.Leg3:
            //    backgroundOne.SetActive(false);
            //    backgroundTwo.SetActive(false);
            //    backgroundThree.SetActive(true);
            //    backgroundFour.SetActive(false);
            //    backgroundFive.SetActive(false);

            //    groundOne.SetActive(false);
            //    groundTwo.SetActive(false);
            //    groundThree.SetActive(true);
            //    groundFour.SetActive(false);
            //    groundFive.SetActive(false);

            //    bgChanged[2] = true;
            //    break;
            //case Background.Leg4:
            //    backgroundOne.SetActive(false);
            //    backgroundTwo.SetActive(false);
            //    backgroundThree.SetActive(false);
            //    backgroundFour.SetActive(true);
            //    backgroundFive.SetActive(false);

            //    groundOne.SetActive(false);
            //    groundTwo.SetActive(false);
            //    groundThree.SetActive(false);
            //    groundFour.SetActive(true);
            //    groundFive.SetActive(false);

            //    bgChanged[3] = true;
            //    break;
            //case Background.Leg5:
                //backgroundOne.SetActive(false);
                //backgroundTwo.SetActive(false);
                //backgroundThree.SetActive(false);
                //backgroundFour.SetActive(false);
                //backgroundFive.SetActive(true);

                //groundOne.SetActive(false);
                //groundTwo.SetActive(false);
                //groundThree.SetActive(false);
                //groundFour.SetActive(false);
                //groundFive.SetActive(true);

                //bgChanged[4] = true;
                //break;
        }
        //if (gameOver) { return; }
        //String msg = "";
        //if (leg == Background.Leg2) { msg = "Leg2: Algeria to Spain"; }
        //else if (leg == Background.Leg3) { msg = "Leg3: Spain to France"; }
        //else if (leg == Background.Leg4) { msg = "Leg 4: France to England"; }
        //else if (leg == Background.Leg5) { msg = "Leg 5: Destination"; }
        //messageContainer.SetActive(true);
        //message.text = msg;

        //Debug.Log("game over state: " + gameOver);
        //Debug.Log("setting message container to active....");

    }

    //IEnumerator OnGameOverSuccess()
    //{
    //    Debug.Log("Entering OnGameOverSuccess");
    //    yield return new WaitForSeconds(3);
       

    //}

    /*
     * Clear the screen before displaying game over text.
    */
    IEnumerator DelayedTransition()
    {

        yield return new WaitForSeconds(0);
        SetPageState(PageState.GameOver);

        //if (finalLeg) {
        //    // wait about 26 seconds to end the game; that's how long it takes
        //    // for the castle to center

        //    Debug.Log("returned from yielding...about to set success!!!");
        //    SetPageState(PageState.GameOverSuccess);
        //}
        //else {
        //    SetPageState(PageState.GameOver);
        //}
        healthBar.SetActive(false);
        currentScore.SetActive(false);
        //messageContainer.SetActive(false);
        //message.text = "";

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
        SetPageState(PageState.Start);
        scoreText.text = "0";
        ResetObjects();
       
;
        OnGameOverConfirmed();

    }

    public void StartGame()
    {
        Debug.Log("Entering StartGame...");
        //messageContainer.SetActive(false);
        healthBar.SetActive(true);
        currentScore.SetActive(true);
        //messageContainer.SetActive(true);
        ChangeBackground(Background.Leg1);
        SetPageState(PageState.Countdown);
        Debug.Log("Exiting StartGame...");
    }

    void ResetObjects()
    {
        Debug.Log("Entering ResetObject...");
        CancelInvoke();
        // 
        for (int i = 0; i < bgChanged.Length; i++) { bgChanged[i] = false; }
        for (int i = 0; i < alerted.Length; i++) { alerted[i] = false; }

        score = 0;
        finalLeg = false;


        ChangeBackground(Background.Leg1);

        healthBar.SetActive(false);
        currentScore.SetActive(false);
        //messageContainer.SetActive(false);

        Debug.Log("set all of them to false");

        //bird.transform.position = new Vector2(0, 0);
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

        // Call the setup function on the asteroid with the desired force and torque.
        //ast.GetComponent<AsteroidController>().Setup(-pos.normalized * 1000.0f, Random.insideUnitSphere * Random.Range(500.0f, 1500.0f));

        // Assign the health values to the asteroid.
        //ast.GetComponent<AsteroidController>().health = asteroidHealth;
        //ast.GetComponent<AsteroidController>().maxHealth = asteroidHealth;
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
    public void ShowDeathScreen()
    {
        //if (hasLost == false)
            //hasLost = true;

        // Enable the game over screen game object.
        gameOverScreen.gameObject.SetActive(true);

        // Start the Fade coroutine so that the death screen will fade in.
        //StartCoroutine("FadeDeathScreen");

        // Set spawning to false so that no more asteroids get spawned.
        //spawning = false;
    }

    IEnumerator FadeDeathScreen()
    {
        // Wait for half a second for a little bit more dynamic effect.
        yield return new WaitForSeconds(0.0f);

        // Set the text to the final score text plus the user's score.
        //finalScoreText.text = "Final Score\n" + score.ToString();

        // Create temporary colors to be able to apply a fade to the image and text.
        Color imageColor = gameOverScreen.color;
        Color textColor = gameOverText.color;
        //Color finalScoreTextColor = finalScoreText.color;

        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime * 3f)
        {
            // Lerp the alpha of the temp colors from 0 to 0.75 by the amount of t. 
            imageColor.a = Mathf.Lerp(0.0f, 0.75f, t);
            textColor.a = Mathf.Lerp(0.0f, 1.0f, t);
            //finalScoreTextColor.a = Mathf.Lerp(0.0f, 1.0f, t);

            // Apply the temp color to the image and text.
            gameOverScreen.color = imageColor;
            gameOverText.color = textColor;

            //finalScoreText.color = finalScoreTextColor;

            // Wait for next frame.
            yield return null;
        }

        // Apply a finalized amount to the alpha channels.
        imageColor.a = 0.75f;
        textColor.a = 1.0f;

        // Apply the final color values to the image and text.
        gameOverScreen.color = imageColor;
        gameOverText.color = textColor;
    }
    //void Start()
    //{
    //    // Start the spawning timers.
    //    StartCoroutine("SpawnTimer");
    //    //StartCoroutine("SpawnHealthTimer");

    //    // Update the score text to reflect the current score on start.
    //    UpdateScoreText();
    //}


}