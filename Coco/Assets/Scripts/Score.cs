using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{

    Text score;

    void OnEnable()
    {
        score = GetComponent<Text>();
        score.text = "Longest Journey: " + PlayerPrefs.GetInt("HighScore").ToString() + " miles";
    }
}