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
        score.text = "Longest Journey: " + PlayerPrefs.GetInt("HighScore").ToString();
        if (GameManager.Instance.Score == 1)
        {
            score.text += " mile";
        }
        else { score.text += " miles"; }
    }
}