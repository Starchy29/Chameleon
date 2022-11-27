using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdDetection : MonoBehaviour
{
    private BirdMovement birdMovement;

    private void Awake()
    {
        
        birdMovement = transform.parent.GetComponent<BirdMovement>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            GameObject player = collision.gameObject;
            birdMovement.SpottedEnemy(player);
        }
    }
}
