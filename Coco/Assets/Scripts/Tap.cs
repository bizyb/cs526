using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Tap : MonoBehaviour
{

    public delegate void PlayerDelegate();
    public static event PlayerDelegate OnPlayerDied;
    public static event PlayerDelegate OnPlayerScored;

    public float upForce = 200f;
    public bool isDead = false;
    float reward = 20f;
    float decay = -3.0666f; // player health decay rate per second
    public Vector2 startLoc;
    private Animator anim;


    Rigidbody2D rigidbod;
    //public Rigidbody2D Bird { get { return rigidbod; } }

    GameManager game;
    PlayerHealth health;


    void OnEnable()
    {
        GameManager.OnGameStarted += OnGameStarted;
        GameManager.OnGameOverConfirmed += OnGameOverConfirmed;
    }

    void OnDisable()
    {
        GameManager.OnGameStarted -= OnGameStarted;
        GameManager.OnGameOverConfirmed -= OnGameOverConfirmed;
    }

    void OnGameStarted()
    {

        rigidbod.velocity = Vector2.zero;
        rigidbod.simulated = true;
        isDead = false;
        anim.speed = 1;
        anim.SetTrigger("Flap");
        //rigidbod.transform.position.x;
    }

    void OnGameOverConfirmed()
    {
        transform.localPosition = startLoc;
        anim.speed = 1;
        //anim.SetTrigger("Idle");
    }

    // Use this for initialization
    void Start()
    {

        rigidbod = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        game = GameManager.Instance;
        health = PlayerHealth.Instance;
        InvokeRepeating("HealthDecay", 1f, 1f);


    }

    // Update is called once per frame
    void Update()
    {
        if (game.GameOver) { return; }
        if (!isDead)
        {

            if (Input.GetMouseButtonDown(0))
            {
                rigidbod.velocity = Vector2.zero;
                rigidbod.AddForce(new Vector2(0, upForce));
                //Debug.Log("current bird y: " + transform.position.y);
                anim.SetTrigger("Flap");
            }
        }
        //if (game.backgroundFive.activeInHierarchy && !game.FinalLeg) {
        //    game.FinalLeg = true;
        //    StartCoroutine("OnGameOverSuccess");
        //}

    }
    void OnTriggerEnter2D(Collider2D col)
    {
        // TODO: blimp collision area is too wide
       
        if (col.gameObject.tag == "DeadZone") 
        {

            Dead();
        }

        else if (col.gameObject.tag == "RewardZone")
        {
            // make the reward object/food disappear after 
            // collision and update the fuel meter accordingly
            col.gameObject.SetActive(false);
            //Destroy(col.gameObject);
            health.UpdateHealth(reward, this);
        }
        else if (col.gameObject.tag == "TopPerimeter") {
            rigidbod.AddForce(new Vector2(0, -upForce));
        }

    }
    IEnumerator OnGameOverSuccess() {
        Debug.Log("Entering OnGameOverSuccess");
        yield return new WaitForSeconds(15);
        Dead();

    }

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

        OnPlayerDied();

    }

    void HealthDecay()
    {
        if (game.GameOver) { return; }
        health.UpdateHealth(decay, this);
    }
}
