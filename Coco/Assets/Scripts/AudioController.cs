using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour {

    public static AudioController audioController;
    public static AudioController Instance {get { return audioController;} }

    public AudioSource audioSource;
    public AudioSource backgroundSource;
    public AudioClip scoreClip;
    public AudioClip deathClip;
    public AudioClip wingFlapClip;
    public AudioClip joystickClip;
    public AudioClip backgroundClip;
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
            backgroundSource.Stop();
            return;
        }
        if (!backgroundSource.isPlaying) {
            backgroundSource.PlayOneShot(backgroundClip);
        }



    }
    //private void OnEnable()
    //{
    //    InvokeRepeating("AudioOnGamePlay", 1f, 0.01f);
    //}
    //private void OnDisable()
    //{
    //    CancelInvoke();
    //}
    private void Start()
    {
        game = GameManager.Instance;
    }
    public void AudioOnScore() {
        audioSource.PlayOneShot(scoreClip);
    }

    public void AudioOnDeath() {

    }

    public void AudioOnReward() {

    }

    public void AudioOnJoystick() {

    }

    //public void AudioOnGamePlay() {
    //    if (game.GameOver) {
    //        audioSource.Stop();
    //    }
    //    if (audioSource.isPlaying(backgroundClip))
    //    audioSource.pl
    //}
}
