using UnityEngine;
using System.Collections;

public class BackgroundScroll : MonoBehaviour
{
    public float speed = 0.05f;
    private void Update()
    {

        GameManager game = GameManager.Instance;
        if (game.GameOver) { return; }

        Vector2 offset = new Vector2(Time.time * speed, 0);
        GetComponent<Renderer>().material.mainTextureOffset = offset;
    }

}