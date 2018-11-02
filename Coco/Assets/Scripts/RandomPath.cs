using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RandomPath : MonoBehaviour
{

    public GameManager game;
    public GameObject rewardPrefab;
    private Transform target;
    private Animator anim;

    // Speed in units per sec.
    public float speed = 1f;
    private float shiftSpeed = 1f;
    private void Start()
    {
        game = GameManager.Instance;
        anim = GetComponent<Animator>();
    }

    void Update()
    {
       
        if (game.GameOver) {
            // prefab only applies to rewards, i.e. coins
            if (rewardPrefab == null)
            {
                anim.SetTrigger("Idle");
            }
            return; 
        }

        // All non
        if (rewardPrefab == null) { anim.SetTrigger("Flap"); }
        //else { anim.SetTrigger("RotateCoin"); }

        target = game.bird.transform;
        // The step size is equal to speed times frame time.
        if (target != null)
        {
            float step = speed * Time.deltaTime;


            // Move our position a step closer to the target.
            if (rewardPrefab != null)
            {
                // rewardPrefabs do not track our main character, the bird. 
                // Instead, they should maintain their y coordinate and only 
                // move along the x axis
                transform.position += Vector3.left * shiftSpeed * Time.deltaTime;
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, target.position, step);
            }
        }
    }
}