using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TouchController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            //RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //Destroy(this.gameObject);
            //Debug.Log("collider name: " + hit.collider.name);
            if (Physics.Raycast(ray, out hit))
            {
                BoxCollider bc = hit.collider as BoxCollider;
                if (bc != null)
                {
                    Debug.Log("collider name: " + hit.collider.name);
                    //Destroy(bc.gameObject);
                }
                //if (hit != null) {

                //}
               //;
            }

            //if (hit.collider != null)
            //{
            //    //Debug.Log("collider name: " + hit.collider.name);
            //    if (hit.collider.gameObject == this.gameObject) Destroy(this.gameObject);
            //}
        }
        Debug.Log("Exiting TouchController update");

    }

    /*
  * If an obstacle has been touched, destroy it.
  */
    //void OnTouch()
    //{

    //    Debug.Log("Entering OnTouch");
    //    Touch[] touches = Input.touches;
    //    Debug.Log("touch length: " + touches.Length);
    //    for (int i = 0; i < touches.Length; i++)
    //    {
    //        Ray ray = Camera.main.ScreenPointToRay(touches[i].position);
    //        RaycastHit hit;
    //        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
    //        {
    //            Debug.Log("collider name: " + hit.collider.name);
    //            if (hit.collider.name == "Asteroid")
    //            {
    //                Destroy(hit.collider.gameObject);

    //            }
    //        }
    //    }
    //    Debug.Log("Exiting OnTouch");
    //}
}
