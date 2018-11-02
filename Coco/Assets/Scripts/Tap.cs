using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Tap : MonoBehaviour
{

    public delegate void PlayerDelegate(string optional, Vector3 pos);
    public static event PlayerDelegate OnPlayerDied;
    static Tap instance;
    public static Tap Instance { get { return instance; } }

    readonly float upForce = 4f;
    public bool isDead = false;
    float decay = -2.0f; // player health decay rate per second
    public Vector2 startLoc;
    private Animator anim;

    Rigidbody2D rigidbod;

    GameManager game;
    PlayerHealth health;
    AudioController audioController;

    private void Awake()
    {
        if (instance == null) { instance = this; }
    }

    void OnEnable()
    {
        GameManager.OnGameStarted += OnGameStarted;
        GameManager.OnGameOverConfirmed += OnGameOverConfirmed;
        TouchController.OnJoystickTouch += OnJoystickTouch;
    }

    void OnDisable()
    {
        GameManager.OnGameStarted -= OnGameStarted;
        GameManager.OnGameOverConfirmed -= OnGameOverConfirmed;
        TouchController.OnJoystickTouch -= OnJoystickTouch;
    }

    void OnGameStarted()
    {


        rigidbod.velocity = Vector2.zero;
        rigidbod.simulated = true;
        isDead = false;
        anim.speed = 1;
        anim.SetTrigger("FlapCoco");
       
    }

    void OnGameOverConfirmed()
    {
        transform.localPosition = startLoc;
        anim.speed = 0;
        TouchController.OnJoystickTouch += OnJoystickTouch;
    }

    void Start()
    {

        rigidbod = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        game = GameManager.Instance;
        health = PlayerHealth.Instance;
        audioController = AudioController.Instance;
        InvokeRepeating("HealthDecay", 1f, 1f);
        anim.speed = 0;

    }

    public void OnJoystickTouch(string direction, Vector3 pos) {

        if (game.GameOver) { return; }
        if (!isDead)
        {
           
            rigidbod.velocity = Vector2.zero;
            switch (direction) {
                case "Up":
                    rigidbod.velocity = new Vector2(0, upForce);
                    break;
                case "Down":
                    Debug.Log("Down button pressed");
                    rigidbod.velocity = new Vector2(0, -upForce);
                    break;
                case "Left":
                    Debug.Log("Left button pressed");
                    rigidbod.velocity = new Vector2(-upForce, 0);
                    break;
                case "Right":
                    Debug.Log("Right button pressed");
                    rigidbod.velocity = new Vector2(upForce, 0);
                    break;
            }
            anim.SetTrigger("FlapCoco");
        }

    } 

    void OnTriggerEnter2D(Collider2D col)
    {
     
       
        if (col.gameObject.tag == "DeadZone") 
        {

            Dead();
            anim.speed = 0;
        }
        else {
            rigidbod.velocity = Vector2.zero;
            switch (col.gameObject.name) {
                case "TopPerimeter":
                    Debug.Log("Top perimeter breached!!!");
                    rigidbod.velocity = new Vector2(0, -upForce);
                    break;
                case "LeftPerimeter":
                    Debug.Log("Left perimeter breached!!!");
                    rigidbod.velocity = new Vector2(upForce, 0);
                    break;
                case "RightPerimeter":
                    Debug.Log("Right perimeter breached!!!");
                    rigidbod.velocity = new Vector2(-upForce, 0);
                    break;
            }
        }

    }

    public void Dead()
    {

        isDead = true;
        rigidbod.simulated = false;
        rigidbod.velocity = Vector2.zero;
        audioController.AudioOnDeath();

        OnPlayerDied(null, Vector3.zero);

    }

    void HealthDecay()
    {
        if (game.GameOver) { return; }
        health.UpdateHealth(decay, this);
    }
}
