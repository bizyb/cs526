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
        score.text = "Travelled: " + GameManager.Instance.Score.ToString() + " miles";
    }
}