using UnityEngine;
using System.Collections;

public class BackgroundScroll : MonoBehaviour
{


    public float speed = 0.5f;
    private void Update()
    {
        if (GameManager.Instance.GameOver) { return; }
        Vector2 offset = new Vector2(Time.time * speed, 0);
        GetComponent<Renderer>().material.mainTextureOffset = offset;
    }

}