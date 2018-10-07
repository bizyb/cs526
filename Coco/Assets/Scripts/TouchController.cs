using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TouchController : MonoBehaviour
{

    public delegate void PlayerDelegate();
    public static event PlayerDelegate OnJoystickTouch;

   
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
            if (hit.collider != null)
            {
                if (hit.collider.gameObject.name == "Joystick") {
                    OnJoystickTouch();
                }
                else{
                    Destroy(hit.collider.gameObject);
                }
               
            }
        }
    }

   

}