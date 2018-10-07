using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RandomPath : MonoBehaviour
{

    public GameManager game;
    private Transform target;

    // Speed in units per sec.
    public float speed = 1f;
    private void Start()
    {
        game = GameManager.Instance;
    }

    void Update()
    {
        if (game.GameOver) { return; }
        target = game.bird.transform;
        // The step size is equal to speed times frame time.
        if (target != null)
        {
            float step = speed * Time.deltaTime;

            // Move our position a step closer to the target.
            transform.position = Vector3.MoveTowards(transform.position, target.position, step);
        }
    }
}