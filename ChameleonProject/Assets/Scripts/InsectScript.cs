using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InsectScript : MonoBehaviour
{
    public Sprite wingsUp;
    public Sprite wingsDown;
    private SpriteRenderer sprite;
    private float timer;

    private bool active;
    public bool Active { get { return active; } }

    private GameManager gameManager;

    private void Awake()
    {
        gameManager = GameManager.Instance;
        sprite = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //gameManager.AteFly(); // could do this instead of player calculating
    }

    private void Update()
    {
        // flap wings "animation"
        timer -= Time.deltaTime;
        if(timer <= 0) {
            timer = 0.05f;
            if(sprite.sprite == wingsDown) {
                sprite.sprite = wingsUp;
            } else {
                sprite.sprite = wingsDown;
            }
        }
    }
}
