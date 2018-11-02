using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedScore : MonoBehaviour {

    public GameObject destPreFab;
    private Transform target;
    private float speed = 16f;

    // Update is called once per frame
    void Update () {

        target = destPreFab.transform;
        float step = speed * Time.deltaTime;

        if (transform != null && transform.position == target.position) {

            Destroy(this.gameObject);

        }
        else if (transform != null) {
            transform.position = Vector3.MoveTowards(transform.position, target.position, step);
        }

    }
}
