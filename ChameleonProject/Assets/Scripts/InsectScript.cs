using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InsectScript : MonoBehaviour
{

    private bool active;
    public bool Active { get { return active; } }

    private GameManager gameManager;

    private void Awake()
    {
        gameManager = GameManager.Instance;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //gameManager.AteFly(); // could do this instead of player calculating
    }
}
