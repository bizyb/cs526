using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour {

    public static AudioController audioController;
    public static AudioController Instance {get { return audioController;} }

    public AudioSource mainAudioSrc;
    public AudioSource rewardAudioSrc;
    public AudioSource backgroundAudioSrc;
    public AudioSource wingAudioSrc;
    public AudioClip scoreClip;
    public AudioClip rewardClip;
    public AudioClip deathClip;
    public AudioClip wingFlapClip;
    //public AudioClip joystickClip;
    public AudioClip backgroundClip;
    GameManager game;
    Tap tapController;

    private void Awake()
    {
        if (audioController == null) {
            audioController = this;
        }
        else {
            mainAudioSrc.mute = true;
        }
    }
   
    private void Update()
    {
       
        if (game.GameOver || !game.GameStarted)
        {
            wingAudioSrc.Stop();
            backgroundAudioSrc.Stop();
            return;
        }
        if (!wingAudioSrc.isPlaying) {
            wingAudioSrc.PlayOneShot(wingFlapClip);
        }
        if (!backgroundAudioSrc.isPlaying) {
            backgroundAudioSrc.PlayOneShot(backgroundClip);
        }
              
    }

    private void Start()
    {
        game = GameManager.Instance;
        tapController = Tap.Instance;
    }
   
    public void AudioOnScore() {
        mainAudioSrc.PlayOneShot(scoreClip);
    }

    public void AudioOnDeath() {
        mainAudioSrc.PlayOneShot(deathClip);
       
    }

    public void AudioOnReward() {
        rewardAudioSrc.PlayOneShot(rewardClip);
    }

    public void AudioOnJoystick() {

    }
}
