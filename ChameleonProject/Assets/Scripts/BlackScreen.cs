using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Fades in from black at the beginning of each level, and cuts to black when dying
public class BlackScreen : MonoBehaviour
{
    private SpriteRenderer sprite;
    private float opacity;
    private const float FADE_SPEED = 0.8f; // opacity per second

    // Start is called before the first frame update
    void Start()
    {
        sprite = gameObject.GetComponent<SpriteRenderer>();
        sprite.color = Color.black;
        opacity = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if(!GameManager.Instance.IsLevelOver && opacity > 0) {
            // fade in at start of level
            opacity -= FADE_SPEED * Time.deltaTime;
            if(opacity < 0) {
                opacity = 0;
            }
            sprite.color = new Color(0, 0, 0, opacity);
        }
    }

    // makes the screen all black, then resets the level after a bit
    public void CutToBlack() {
        Vector4.Lerp(sprite.color, new Color(0, 0, 0, 1), .5f);
        //sprite.color = new Color(0, 0, 0, 1);
    }
}
