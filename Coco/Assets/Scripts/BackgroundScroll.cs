using UnityEngine;
using System.Collections;

public class BackgroundScroll : MonoBehaviour
{
    public float speed = 0.05f;
    private void Update()
    {
        GameManager game = GameManager.Instance;
        if (game.GameOver) { return; }
        if (game.backgroundFive.activeInHierarchy) { speed = 0.01f; }

        Vector2 offset = new Vector2(Time.time * speed, 0);
        Debug.Log("speed: " + speed);

        GetComponent<Renderer>().material.mainTextureOffset = offset;
    }

}