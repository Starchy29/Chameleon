using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    public float speed;
    private Waypoints wPoints;

    private int waypointIndex;

    void Start()
    {
        wPoints = GameObject.FindGameObjectWithTag("Waypoints").GetComponent<Waypoints>();
    }

    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, wPoints.waypoints[waypointIndex].position, speed * Time.deltaTime);

        if(Vector2.Distance(transform.position, wPoints.waypoints[waypointIndex].position) < 0.1f)
        {
     
            if(waypointIndex < wPoints.waypoints.Length - 1)
            {
                waypointIndex++;
            }
            else
            {
                waypointIndex = 0;
            }
        }
    }
}
