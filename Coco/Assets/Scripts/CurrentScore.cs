using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurrentScore : MonoBehaviour
{

    Text score;

    void OnEnable()
    {
        score = GetComponent<Text>();
        score.text = "You have travelled " + GameManager.Instance.Score.ToString();
        if (GameManager.Instance.Score == 1)
        {
            score.text += " mile";
        }
        else { score.text += " miles"; }
    }
}