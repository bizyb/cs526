using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RandomPath : MonoBehaviour
{
    //[SerializeField] Vector3 _targetPosition;

    //// Use this for initialization
    //void Start()
    //{

    //}

    //// Update is called once per frame
    //void Update()
    //{
    //    //if (Input.GetKeyDown(KeyCode.Space)) MoveToExample();
    //    MoveToExample();
    //}

    //void MoveToExample()
    //{
    //    iTween.MoveTo(transform.gameObject, iTween.Hash("position", _targetPosition, "speed", 1f, "easetype", iTween.EaseType.easeInOutSine));
    //}
    //private float timeToChangeDirection;
    //Rigidbody rigidbody;

    //// Use this for initialization
    //public void Start()
    //{
    //    rigidbody = GetComponent<Rigidbody>();
    //    ChangeDirection();
    //}

    //// Update is called once per frame
    //public void Update()
    //{
    //    timeToChangeDirection -= Time.deltaTime;

    //    if (timeToChangeDirection <= 0)
    //    {
    //        ChangeDirection();
    //    }

    //    //rigidbody.velocity = transform.up * 2;


    //}



    //private void ChangeDirection()
    //{
    //    float angle = Random.Range(0f, 360f);
    //    Quaternion quat = Quaternion.AngleAxis(angle, Vector3.forward);
    //    Vector3 newUp = quat * Vector3.up;
    //    newUp.z = 0;
    //    newUp.Normalize();
    //    //transform.up = newUp;
    //    transform.position = newUp;
    //     //pos.y = UnityEngine.Random.Range(ySpawnRange.minY, ySpawnRange.maxY);


    //    //pos.y = UnityEngine.Random.Range(ySpawnRange.minY, ySpawnRange.maxY);



    //    timeToChangeDirection = 2.5f;
    //}

    // The target marker.
    public GameManager game;
    private Transform target;

    // Speed in units per sec.
    public float speed = 1f;
    private void Start(){
        game = GameManager.Instance;
    }

    void Update()
    {

        target = game.bird.transform;
        // The step size is equal to speed times frame time.
        if (target != null)
        {


            float step = speed * Time.deltaTime;

            // Move our position a step closer to the target.
            transform.position = Vector3.MoveTowards(transform.position, target.position, step);
            Debug.Log("current bird position: " + game.bird.transform.position);
        }
    }
}