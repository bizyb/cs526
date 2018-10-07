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

    private void Awake()
    {
        if (audioController == null) {
            audioController = this;
        }
        else {
            audioSource.mute = true;
        }
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

    public void AudioOnGamePlay() {

    }
}
