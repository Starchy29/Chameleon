using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeMovement : MonoBehaviour
{
    public enum SnakeAction
    {
        Guarding,
        Stalking,
        Attacking,
        Returning
    }

    [Header("Character Data")]
    [SerializeField] [Range(1.0f, 10.0f)] private float impulse = 20.0f;
    private float rotateSpeed = 5f;
    private Rigidbody2D snakeRB;
    private SnakeAction action;
    private GameObject player;
    private GameObject fov;
    private SnakeFOV fovScript;
    private float stalkTimer;
    private Vector2 homePosition;

    [Header("Target Data")]
    [SerializeField] private Vector2 targetPosition;
    private int targetWPIndex;

    void Start()
    {
        snakeRB = GetComponent<Rigidbody2D>();
        fovScript = GetComponent<SnakeFOV>();
        fov = transform.GetChild(0).gameObject;
        action = SnakeAction.Guarding;
        homePosition = transform.position;
    }

    private void FixedUpdate()
    {
        UpdateSnake();
    }

    public void UpdateSnake()
    {
        switch (action)
        {
            case SnakeAction.Guarding:
                // Normal movement
                Rotate();
                break;
            case SnakeAction.Stalking:
                // While target is visible, wait 2 seconds
                stalkTimer += Time.deltaTime;
                MoveToTargetAngular();

                if (stalkTimer > 3.0f)
                {
                    stalkTimer = 0f; // Reset timer
                    action = SnakeAction.Attacking;
                    Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
                    snakeRB.AddForce(direction * impulse, ForceMode2D.Impulse);
                }
                break;
            case SnakeAction.Attacking:
                // If it still sees the chameleon, lunge again. Otherwise return
                break;
            case SnakeAction.Returning:
                UpdateTarget(homePosition);
                break;
        }
    }
    private void MoveToTargetLinear()
    {
        Vector3 direction = (targetPosition - snakeRB.position).normalized; // Get direction to the target
        snakeRB.velocity = direction * 1.0f;
    }

    private void MoveToTargetAngular()
    {
        // Calculate orientation from the velocity
        //float angle = Mathf.Atan2(-snakeRB.velocity.x, snakeRB.velocity.y) * Mathf.Rad2Deg;
        float angle = Vector2.SignedAngle(transform.position, targetPosition);
        snakeRB.rotation = (angle + 180) % 360;
        fovScript.Rotation = (360 - snakeRB.rotation) % 360;
    }
    private void UpdateTarget(Vector3 targetPosition)
    {
        this.targetPosition = targetPosition;
    }
    public void SpottedEnemy(GameObject player)
    {
        this.player = player;
        UpdateTarget(player.transform.position);
        action = SnakeAction.Stalking;
    }

    private void Rotate()
    {
        float angle = rotateSpeed * Time.deltaTime;
        snakeRB.rotation += angle;
        fovScript.Rotation = (360 - snakeRB.rotation) % 360;
    }
}
