﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Tap : MonoBehaviour
{

    public delegate void PlayerDelegate(string optional);
    public static event PlayerDelegate OnPlayerDied;
    static Tap instance;
    public static Tap Instance { get { return instance; } }

    readonly float upForce = 4f;
    //readonly float downForce = -20f;
    public bool isDead = false;
    float reward = 20f;
    float decay = -3.0666f; // player health decay rate per second
    public Vector2 startLoc;
    private Animator anim;

    //Audio

   
    Rigidbody2D rigidbod;
    //public Rigidbody2D Bird { get { return rigidbod; } }

    GameManager game;
    PlayerHealth health;
    AudioController audioController;
    //JoystickController joystick;

    //public static Joystick JoystickDirections {get {return Joystick;}}


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
        anim.SetTrigger("Flap");
        //upForce = 100f;
        //downForce = -200f; // the bird is a bit bouncy but it's fine
        //rigidbod.transform.position.x;
    }

    void OnGameOverConfirmed()
    {
        transform.localPosition = startLoc;
        anim.speed = 1;
        TouchController.OnJoystickTouch += OnJoystickTouch;
        //anim.SetTrigger("Idle");
    }

    // Use this for initialization
    void Start()
    {

        rigidbod = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        game = GameManager.Instance;
        health = PlayerHealth.Instance;
        audioController = AudioController.Instance;
        //joystick = JoystickController.Instance;
        InvokeRepeating("HealthDecay", 1f, 1f);
        //upForce = 20f;
        //downForce = -200f;



    }

    public void OnJoystickTouch(string direction) {

        if (game.GameOver) { return; }
        if (!isDead)
        {
            Debug.Log("Force: " + upForce);
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
            anim.SetTrigger("Flap");
        }

    } 

    void OnTriggerEnter2D(Collider2D col)
    {
        // TODO: blimp collision area is too wide
       
        if (col.gameObject.tag == "DeadZone") 
        {

            Dead();
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

    //IEnumerator OnGameOverSuccess() {
    //    //Debug.Log("Entering OnGameOverSuccess");
    //    yield return new WaitForSeconds(15);
    //    Dead();

    //}

    public void Dead()
    {
        // TODO: make sure the bird is restored to its idle state when the start 
        // page loads

        // play sound, update score, etc
        isDead = true;
        rigidbod.simulated = false;
        rigidbod.velocity = Vector2.zero;
        //anim.SetTrigger("Idle");
        //anim.speed = 0;
        //health.UpdateHealth(100f, this); //restore the health for next time

        OnPlayerDied("");

    }

    void HealthDecay()
    {
        if (game.GameOver) { return; }
        health.UpdateHealth(decay, this);
    }
}
