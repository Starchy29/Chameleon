using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// a quick and dirty script to provide a 3 frame animation to birds
public class BirdAnimator : MonoBehaviour
{
    public List<Sprite> frames;
    public float frameRate;
    private SpriteRenderer renderer;  
    private float timer;
    private int currentSprite;

    // Start is called before the first frame update
    void Start()
    {
        renderer = gameObject.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if(timer <= 0) {
            timer = frameRate;

            // each tick, change the sprite
            currentSprite++;
            if(currentSprite > frames.Count - 1) {
                // loop animation
                currentSprite = 0;
            }
            renderer.sprite = frames[currentSprite];
        }
    }
}
