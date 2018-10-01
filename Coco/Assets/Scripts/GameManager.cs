using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;



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
    public GameObject backgroundTwo;
    public GameObject backgroundThree;
    public GameObject backgroundFour;
    public GameObject backgroundFive;
    public GameObject groundOne;
    public GameObject groundTwo;
    public GameObject groundThree;
    public GameObject groundFour;
    public GameObject groundFive;
    public GameObject messageContainer;
    public Text message;
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
        startTime = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        //anim = GetComponent<Animator>();
        Tap.OnPlayerDied += OnPlayerDied;
        Tap.OnPlayerScored += OnPlayerScored;
        CountdownText.OnCountdownFinished += OnCountdownFinished;



        // update distance travelled, i.e. the score, every 1 second
        InvokeRepeating("OnPlayerScored", 1f, 1f);
        ChangeBackground(Background.Leg1);


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
        healthBar.SetActive(true);
        currentScore.SetActive(true);
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
       
        if (gameOver) { return; }
        int timeNow = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        int elapsed = timeNow - startTime;

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
        if (elapsed >= 0 && elapsed < 5 && !alerted[0])
        {
            AlertPlayer(Alerts.Alert1);

        }
        else if (elapsed >= 5 && elapsed < 10 && !alerted[1])
        {
            AlertPlayer(Alerts.Alert2);

        }
        else if (elapsed >= 10 && elapsed < 15 && !alerted[2])
        {
            AlertPlayer(Alerts.Alert3);
           
        }
        else if (elapsed >= 15 && elapsed < 20 && !alerted[3])
        {
            AlertPlayer(Alerts.Alert4);

        }
        else if (elapsed >= 20 && !alerted[4])
        {
            AlertPlayer(Alerts.Alert5);
           
        }

        if (elapsed >= 0 && elapsed < 5 && !bgChanged[0])
        {
            ChangeBackground(Background.Leg1);

        }
        else if (elapsed >= 5 && elapsed < 10 && !bgChanged[1])
        {
            ChangeBackground(Background.Leg2);
           
        }
        else if (elapsed >= 10 && elapsed < 20 && !bgChanged[2])
        {
            ChangeBackground(Background.Leg3);
           
        }
        else if (elapsed >= 20 && elapsed < 25 && !bgChanged[3])
        {
            ChangeBackground(Background.Leg4);
           
        }
        else if (elapsed >= 25 && !bgChanged[4])
        {
            ChangeBackground(Background.Leg5);
           
        }

    }


    void OnPlayerDied()
    {
        if (!backgroundFive.activeInHierarchy) { gameOver = true; }
        int savedScore = PlayerPrefs.GetInt("HighScore");
        if (score > savedScore)
        {
            PlayerPrefs.SetInt("HighScore", score);
        }
        StartCoroutine("DelayedTransition");

    }

    void AlertPlayer(Alerts alert) {
        //TODO: play sound 

        String s = "";
        switch (alert) {
            case Alerts.Alert1:
                s = "Leg 1: Ghana to Algeria";
                alerted[0] = true;
                break;
            case Alerts.Alert2:
                s = "Approaching Leg2: Algeria to Spain";
                alerted[1] = true;
                break;
            case Alerts.Alert3:
                s = "Approaching Leg 3: Spain to France";
                alerted[2] = true;
                break;
            case Alerts.Alert4:
                s = "Approaching Leg 4: France to England";
                alerted[3] = true;
                break;
            case Alerts.Alert5:
                s = "Approaching Leg 5: Destination";
                alerted[4] = true;
                break;
        }
        messageContainer.SetActive(true);
        message.text = s;

    }

    void ChangeBackground(Background leg) {

        switch (leg) {
            case Background.Leg1:
                backgroundOne.SetActive(true);
                backgroundTwo.SetActive(false);
                backgroundThree.SetActive(false);
                backgroundFour.SetActive(false);
                backgroundFive.SetActive(false);

                groundOne.SetActive(true);
                groundTwo.SetActive(false);
                groundThree.SetActive(false);
                groundFour.SetActive(false);
                groundFive.SetActive(false);

                bgChanged[0] = true;
                break;
            case Background.Leg2:
                backgroundOne.SetActive(false);
                backgroundTwo.SetActive(true);
                backgroundThree.SetActive(false);
                backgroundFour.SetActive(false);
                backgroundFive.SetActive(false);

                groundOne.SetActive(false);
                groundTwo.SetActive(true);
                groundThree.SetActive(false);
                groundFour.SetActive(false);
                groundFive.SetActive(false);

                bgChanged[1] = true;
                break;
            case Background.Leg3:
                backgroundOne.SetActive(false);
                backgroundTwo.SetActive(false);
                backgroundThree.SetActive(true);
                backgroundFour.SetActive(false);
                backgroundFive.SetActive(false);

                groundOne.SetActive(false);
                groundTwo.SetActive(false);
                groundThree.SetActive(true);
                groundFour.SetActive(false);
                groundFive.SetActive(false);

                bgChanged[2] = true;
                break;
            case Background.Leg4:
                backgroundOne.SetActive(false);
                backgroundTwo.SetActive(false);
                backgroundThree.SetActive(false);
                backgroundFour.SetActive(true);
                backgroundFive.SetActive(false);

                groundOne.SetActive(false);
                groundTwo.SetActive(false);
                groundThree.SetActive(false);
                groundFour.SetActive(true);
                groundFive.SetActive(false);

                bgChanged[3] = true;
                break;
            case Background.Leg5:
                backgroundOne.SetActive(false);
                backgroundTwo.SetActive(false);
                backgroundThree.SetActive(false);
                backgroundFour.SetActive(false);
                backgroundFive.SetActive(true);

                groundOne.SetActive(false);
                groundTwo.SetActive(false);
                groundThree.SetActive(false);
                groundFour.SetActive(false);
                groundFive.SetActive(true);

                bgChanged[4] = true;
                break;
        }
       
        String msg = "";
        if (leg == Background.Leg2) { msg = "Leg2: Algeria to Spain"; }
        else if (leg == Background.Leg3) { msg = "Leg3: Spain to France"; }
        else if (leg == Background.Leg4) { msg = "Leg 4: France to England"; }
        else if (leg == Background.Leg5) { msg = "Leg 5: Destination"; }
        messageContainer.SetActive(true);
        message.text = msg;

    }

    /*
     * Clear the screen before displaying game over text.
    */
    IEnumerator DelayedTransition()
    {

        yield return new WaitForSeconds(1);
        healthBar.SetActive(false);
        currentScore.SetActive(false);
        messageContainer.SetActive(false);
        message.text = "";

        if (backgroundTwo.activeInHierarchy) {
            // wait about 26 seconds to end the game; that's how long it takes
            // for the castle to center
            Debug.Log("returned from yielding...about to set success!!!");
            gameOver = true;
            SetPageState(PageState.GameOverSuccess);
        }
        else {
            SetPageState(PageState.GameOver);
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

        SetPageState(PageState.Start);
        scoreText.text = "0";
        //anim.SetTrigger("Idle");
        OnGameOverConfirmed();
    }

    public void StartGame()
    {
       
        SetPageState(PageState.Countdown);
    }

}