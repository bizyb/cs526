using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TouchController : MonoBehaviour
{

    public delegate void PlayerDelegate(string direction, Vector3 pos);
    public static event PlayerDelegate OnJoystickTouch;
    public static event PlayerDelegate OnPlayerScored;

    public GameManager game;
    private Animator anim;
    AudioController audioController;

    private void Start()
    {
        game = GameManager.Instance;
        anim = GetComponent<Animator>();
        audioController = AudioController.Instance;
    }


    void Update()
    {
        //TODO: if the bird touches the joystick while falling, it will slow down
        // this shouldn't happen; its velocity should stay the same

        if (game.GameOver) {
            // not all objects have an animator component, e.g. joystick 
            // controls have none
            if (anim != null) { anim.speed = 0; }
        
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
           
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
            if (hit.collider != null)
            {
                Vector3 pos = hit.collider.gameObject.transform.position;
                if (hit.collider.gameObject.name == "UpArrow") {
                    OnJoystickTouch("Up", Vector3.zero);
                }
                else if (hit.collider.gameObject.name == "DownArrow")
                {
                    OnJoystickTouch("Down", Vector3.zero);
                }
                else if (hit.collider.gameObject.name == "LeftArrow")
                {
                    OnJoystickTouch("Left", Vector3.zero);
                }
                else if (hit.collider.gameObject.name == "RightArrow")
                {
                    OnJoystickTouch("Right", Vector3.zero);
                }
                else if (hit.collider.gameObject.tag == "RewardZone") {
                    OnPlayerScored("Coin", pos);
                    Destroy(hit.collider.gameObject);
                }
                else 
                {
                    StopAudio(hit.collider.gameObject);
                    Destroy(hit.collider.gameObject);
                    // update the health as well
                    OnPlayerScored(null, pos);
                }
               
            }
        }
    }

    void StopAudio(GameObject gameObject) {

        if (gameObject.name == "Helicopter" || gameObject.name == "Helicopter(Clone)"){
            // Stop the audio only if there are no more helicopters on the screen
            game.HeliCount--;
            if (game.HeliCount == 0) {
                audioController.AudioOnHelicopterStop();
            }
           
        }
    }

   

}