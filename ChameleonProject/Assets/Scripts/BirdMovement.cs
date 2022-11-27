using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdMovement : MonoBehaviour
{
    public enum BirdAction
    {
        Guarding,
        Chasing,
        Attacking,
        Searching,
        Returning
    }

    [Header("Character Data")]
    [SerializeField] [Range(0.0f, 5.0f)] private float maxSpeed = 1.0f;
    private Rigidbody2D birdRB;
    private BirdAction action;
    private GameObject player;
    private GameObject fov;
    private BirdFOV fovScript;

    [Header("Target Data")]
    [SerializeField] private GameObject waypointContainer;
    [SerializeField] private Vector2[] waypoints;
    [SerializeField] private Vector2 targetPosition;
    private int targetWPIndex;
    private void CheckError(string error)
    {
        if (error.Length > 0)
        {
            Debug.LogError(error);
        }
    }

    void Start()
    {
        birdRB = GetComponent<Rigidbody2D>();
        fovScript = GetComponent<BirdFOV>();
        fov = transform.GetChild(0).gameObject;
        action = BirdAction.Guarding;
        InitWaypoints();

        // Set initial waypoint
        targetWPIndex = 0;
        targetPosition = waypoints[targetWPIndex];
    }

    private void FixedUpdate()
    {
        UpdateBird();
    }

    public void UpdateBird()
    {
        switch (action)
        {
            case BirdAction.Guarding:
                UpdateBirdMovement();
                UpdateWaypoint();
                break;
            case BirdAction.Chasing:
                UpdateTarget();
                UpdateBirdMovement();
                break;
            case BirdAction.Attacking:
                break;
            case BirdAction.Searching:
                break;
            case BirdAction.Returning:
                break;
        }
    }

    /// <summary>
    /// Get waypoints for Bird Path
    /// </summary>
    /// <returns></returns>
    private void InitWaypoints()
    {
        waypointContainer = GameObject.Find("Waypoints");

        CheckError(GetWaypoints());
    }
    private string GetWaypoints()
    {
        if (waypointContainer == null) { return "Could not find waypoints"; }

        int waypointAmount = waypointContainer.transform.childCount;
        waypoints = new Vector2[waypointAmount];
        for (int i = 0; i < waypointAmount; i++)
        {
            waypoints[i] = waypointContainer.transform.GetChild(i).position;
        }
        return "";
    }
    private void UpdateWaypoint()
    {
        CheckIfReachedWaypoint();
    }
    private void UpdateBirdMovement()
    {
        MoveToTargetLinear();
        MoveToTargetAngular();
    }
    private void MoveToTargetLinear()
    {
        Vector3 direction = (targetPosition - birdRB.position).normalized; // Get direction to the target
        birdRB.velocity = direction * maxSpeed;
    }
    private void MoveToTargetAngular()
    {
        // Make sure we have a velocity
        if (birdRB.velocity.magnitude > 0)
        {
            // Calculate orientation from the velocity
            float angle = Mathf.Atan2(-birdRB.velocity.x, birdRB.velocity.y) * Mathf.Rad2Deg;
            birdRB.rotation = angle;
            fovScript.Rotation = (360-angle)%360;
            //fov.transform.rotation = transform.rotation; // Why does this not work
        }
    }
    private void CheckIfReachedWaypoint()
    {
        if (Vector2.Distance(transform.position, targetPosition) < 0.5f)
        {
            NextWaypoint();
        }
    }
    private void NextWaypoint()
    {
        if (targetWPIndex == waypoints.Length-1)
        {
            targetWPIndex = 0;
        }
        else
        {
            targetWPIndex++;
        }

        targetPosition = waypoints[targetWPIndex];
    }
    private void UpdateTarget()
    {
        targetPosition = player.transform.position;
    }

    // Transitions
    public void SpottedEnemy(GameObject player)
    {
        this.player = player;
        // Last waypoint is stored
        action = BirdAction.Chasing;
    }
    public void LostEnemy()
    {
        action = BirdAction.Searching;
    }
}
