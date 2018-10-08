using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TouchController : MonoBehaviour
{

    public delegate void PlayerDelegate(string direction);
    public static event PlayerDelegate OnJoystickTouch;
    public static event PlayerDelegate OnPlayerScored;

   
    void Update()
    {
        //TODO: if the bird touches the joystick while falling, it will slow down
        // this shouldn't happen; its velocity should stay the same

        //Debug.Log("entered update in TouchController");
        if (Input.GetMouseButtonDown(0))
        {
           
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
            if (hit.collider != null)
            {
                if (hit.collider.gameObject.name == "UpArrow") {
                    OnJoystickTouch("Up");
                }
                else if (hit.collider.gameObject.name == "DownArrow")
                {
                    OnJoystickTouch("Down");
                }
                else if (hit.collider.gameObject.name == "LeftArrow")
                {
                    OnJoystickTouch("Left");
                }
                else if (hit.collider.gameObject.name == "RightArrow")
                {
                    OnJoystickTouch("Right");
                }
                else
                {
                    Destroy(hit.collider.gameObject);

                    // update the health as well
                    OnPlayerScored(null);
                }
               
            }
        }
    }

   

}