using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ChameleonScript : MonoBehaviour
{
    public enum BodyColor
    {
        Green,
        Blue,
        Red,
        Brown
    }
    //[SerializeField] private FieldOfView fieldOfView;

    [SerializeField] private BodyColor startColor; // allows each level to start as a different color
    [SerializeField] private float Friction;
    [SerializeField] private float Acceleration;
    [SerializeField] private float MaxSpeed;

    // Tile color sprites to be set as prefab, used to check the tile this is standing on
    [SerializeField] private Sprite redSprite;
    [SerializeField] private Sprite blueSprite;
    [SerializeField] private Sprite greenSprite;
    [SerializeField] private Sprite brownSprite;

    private Rigidbody2D body;
    private Tilemap tiles;

    private BodyColor color;
    private bool onMatchingTile;
    private bool inBush; // track if in bush to make chameleon invisible
    public bool Visible { get { return !onMatchingTile && !inBush; } }

    private GameManager manager;
    private int localFlyCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        manager = GameManager.Instance;
        body = GetComponent<Rigidbody2D>();
        // Grid needs to be labeled "Grid-<level#>"
        tiles = GameObject.Find("Grid-" + manager.Level).transform.GetChild(0).gameObject.GetComponent<Tilemap>();
        if (tiles == null) { Debug.LogError("Make sure to name the grid \"Grid\""); }

        SetColor(startColor);
    }

    // FixedUpdate helps with the wall jitteriness
    void FixedUpdate()
    {
        // move
        if (body.velocity != Vector2.zero)
        {
            // apply friction
            Vector2 friction = body.velocity;
            friction.Normalize();
            friction *= -Friction * Time.deltaTime;
            body.velocity += friction;

            // check if crossed zero (if velocity now points in direction of friction)
            if (Vector2.Dot(friction, body.velocity) > 0)
            {
                body.velocity = Vector2.zero;
            }
        }
        Vector2 movement = GetMoveDirection();
        if (movement != Vector2.zero)
        {
            // accelerate
            body.velocity += Acceleration * GetMoveDirection() * Time.deltaTime;

            // cap speed
            if (body.velocity.magnitude > MaxSpeed)
            {
                body.velocity = body.velocity.normalized;
                body.velocity *= MaxSpeed;
            }
        }

        // rigidbody translates using velocity on its own

        // rotate to face move direction
        if (body.velocity != Vector2.zero)
        {
            float angle = Mathf.Atan2(body.velocity.y, body.velocity.x) * 180f / Mathf.PI; // converted to degrees
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        // check visibility from tile
        onMatchingTile = false; // visible until proven hidden
        Tile currentTile = tiles.GetTile<Tile>(tiles.LocalToCell(transform.position));
        if (currentTile)
        {
            switch (color)
            {
                case BodyColor.Blue:
                    if (currentTile.sprite == blueSprite)
                    {
                        onMatchingTile = true;
                    }
                    break;
                case BodyColor.Green:
                    if (currentTile.sprite == greenSprite)
                    {
                        onMatchingTile = true;
                    }
                    break;
                case BodyColor.Brown:
                    if (currentTile.sprite == brownSprite)
                    {
                        onMatchingTile = true;
                    }
                    break;
                case BodyColor.Red:
                    if (currentTile.sprite == redSprite)
                    {
                        onMatchingTile = true;
                    }
                    break;
            }
        }

        // tongue shot
        if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.Joystick1Button0))
        {

        }

        //adjusts field of vision
        //fieldOfView.SetAimDirection(movement);
        //fieldOfView.SetOrigin(transform.position);
    }

    private void SetColor(BodyColor color)
    {
        this.color = color;
        switch (color)
        {
            case BodyColor.Blue:
                GetComponent<SpriteRenderer>().color = Color.blue;
                break;
            case BodyColor.Green:
                GetComponent<SpriteRenderer>().color = Color.green;
                break;
            case BodyColor.Red:
                GetComponent<SpriteRenderer>().color = Color.red;
                break;
            case BodyColor.Brown:
                GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.1f, 0.1f);
                break;
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Bush")
        {
            SetColor(collision.gameObject.GetComponent<BushScript>().color);
            inBush = true;
        }
        else if (collision.gameObject.tag == "Fly")
        {
            EatFly();
            Destroy(collision.gameObject);
        }

        else if (collision.gameObject.tag == "Enemy Vision")
        {
            //Debug.Log("COLLISION");
            transform.position = Vector3.zero;
        }
    }

    // detect that the chameleon has left its bush. Bushes must not overlap for this to work
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Bush")
        {
            inBush = false;
        }
    }

    private Vector2 GetMoveDirection()
    {
        // check for gamepad
        Vector2 joystick = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if (joystick != Vector2.zero)
        {
            return joystick;
        }

        // check for keyboard
        Vector2 result = new Vector2();
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        { // right
            result.x += 1;
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        { // left
            result.x += -1;
        }
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        { // up
            result.y += 1;
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        { // down
            result.y += -1;
        }
        if (result != Vector2.zero)
        {
            result.Normalize();
        }
        return result;
    }

    private void EatFly()
    {
        localFlyCount++;
        manager.UpdateFlyCount(localFlyCount);
    }
}
