using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour {

    public static AudioController audioController;
    public static AudioController Instance {get { return audioController;} }

    public AudioSource audioSource;
    public AudioClip scoreClip;
    public AudioClip deathClip;
    public AudioClip wingFlapClip;
    public AudioClip joystickClip;
    public AudioClip backgroundClip;
    bool wingAudioPlaying;
    bool backgroundPlaying;
    bool deathPlaying;
    GameManager game;
    Tap tapController;

    private void Awake()
    {
        if (audioController == null) {
            audioController = this;
        }
        else {
            audioSource.mute = true;
        }
    }
   
    private void Update()
    {
        //Debug.Log("Game over state: " + game.GameOver);
        if (game.GameOver)
        {
            if (!deathPlaying) {
                audioSource.Stop();
                AudioOnDeath();
            }
            return;
        }
        if (!audioSource.isPlaying) {
            audioSource.PlayOneShot(backgroundClip);
            audioSource.PlayOneShot(wingFlapClip);
        }
        if (!wingAudioPlaying) {
            StartCoroutine("PlayBirdWingFlap");
        }
        if (!backgroundPlaying) {
            StartCoroutine("PlayBackground");
        }

       
    }

    private void Start()
    {
        game = GameManager.Instance;
        tapController = Tap.Instance;
        deathPlaying = false;
    }
    public void AudioOnScore() {
        audioSource.PlayOneShot(scoreClip);
    }

    void AudioOnDeath() {


        deathPlaying = true;
        Debug.Log("playing death....");
        audioSource.PlayOneShot(deathClip);

    }

    IEnumerator PlayBirdWingFlap() {
        wingAudioPlaying = true;
        yield return new WaitForSeconds(2.4f);
        wingAudioPlaying = false;
        if (!game.GameOver && tapController.CanPlayAudio)
        { 
            audioSource.PlayOneShot(wingFlapClip); 
        }
       
    }

    IEnumerator PlayBackground() {
        // TODO: different levels have background music of different length; 
        // should not be hard-coded
        backgroundPlaying = true;
        yield return new WaitForSeconds(14f);
        backgroundPlaying = false;
        if (!game.GameOver && tapController.CanPlayAudio) {
            audioSource.PlayOneShot(backgroundClip);
        }

    }

    public void AudioOnReward() {

    }

    public void AudioOnJoystick() {

    }
}
