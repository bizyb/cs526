﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Tap : MonoBehaviour {

	public float upForce = 200f;
    private bool isDead = false;
    //public float tiltSmooth = 5;
	public Vector2 startLoc;
    private Animator anim;


	Rigidbody2D rigidbod;

    //Quaternion downRotation;
    //Quaternion forwardRotation;



	

	// Use this for initialization
	void Start () {

		rigidbod = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        //downRotation = Quaternion.Euler(0, 0, -90);
        //forwardRotation = Quaternion.Euler(0, 0, 45);

		
	}
	
	// Update is called once per frame
	void Update () {
        if (!isDead)
        {
            if (Input.GetMouseButtonDown(0))
            {
                //transform.rotation = forwardRotation;
                rigidbod.velocity = Vector2.zero;
                rigidbod.AddForce(new Vector2(0, upForce));
                anim.SetTrigger("Flap");
            }
        }
        //transform.rotation = Quaternion.Lerp(transform.rotation, downRotation, tiltSmooth * Time.deltaTime);
	}
    void OnTriggerEnter2D(Collider2D col) {
        if (col.gameObject.tag == "DeadZone") {
            // play sound, update score, etc
            isDead = true;
            rigidbod.simulated = false;
            rigidbod.velocity = Vector2.zero;
            anim.SetTrigger("Die");
        }
        if (col.gameObject.tag == "ScoreZone") {


        }
    }
}
