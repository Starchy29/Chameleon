using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BushScript : MonoBehaviour
{
    public ChameleonScript.BodyColor color;

    // Start is called before the first frame update
    void Start()
    {
        switch(color) {
            case ChameleonScript.BodyColor.Blue:
                GetComponent<SpriteRenderer>().color = Color.blue;
                break;
            case ChameleonScript.BodyColor.Green:
                GetComponent<SpriteRenderer>().color = Color.green;
                break;
            case ChameleonScript.BodyColor.Red:
                GetComponent<SpriteRenderer>().color = Color.red;
                break;
            case ChameleonScript.BodyColor.Yellow:
                GetComponent<SpriteRenderer>().color = Color.yellow;
                break;
        }
    }
}