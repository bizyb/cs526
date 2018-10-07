using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour {

    public static AudioController audioController;
    public static AudioController Instance {get { return audioController;} }

    public AudioSource audioSource;
    //public AudioSource backgroundSource;
    //public AudioSource birdWingSource;
    //public AudioSource joystickSource;
    public AudioClip scoreClip;
    public AudioClip deathClip;
    public AudioClip wingFlapClip;
    public AudioClip joystickClip;
    public AudioClip backgroundClip;
    bool wingAudioPlaying;
    bool backgroundPlaying;
    GameManager game;

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
        if (game.GameOver)
        {
            audioSource.Stop();
            //birdWingSource.Stop();
            //joystickSource.Stop();
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
    }
    public void AudioOnScore() {
        audioSource.PlayOneShot(scoreClip);
    }

    public void AudioOnDeath() {

    }

    IEnumerator PlayBirdWingFlap() {
        wingAudioPlaying = true;
        yield return new WaitForSeconds(2.4f);
        wingAudioPlaying = false;
        audioSource.PlayOneShot(wingFlapClip);
    }

    IEnumerator PlayBackground() {
        // TODO: different levels have background music of different length; 
        // should not be hard-coded
        backgroundPlaying = true;
        yield return new WaitForSeconds(14f);
        backgroundPlaying = false;
        audioSource.PlayOneShot(backgroundClip);
    }

    public void AudioOnReward() {

    }

    public void AudioOnJoystick() {

    }
}
