using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ChameleonScript : MonoBehaviour
{
    public enum BodyColor {
        Green,
        Blue,
        Red,
        Yellow
    }
    [SerializeField] private FieldOfView fieldOfView;

    [SerializeField] private BodyColor startColor; // allows each level to start as a different color
    [SerializeField] private float Friction;
    [SerializeField] private float Acceleration;
    [SerializeField] private float MaxSpeed;

    // Tile color sprites to be set as prefab, used to check the tile this is standing on
    [SerializeField] private Sprite redSprite;
    [SerializeField] private Sprite blueSprite;
    [SerializeField] private Sprite greenSprite;
    [SerializeField] private Sprite yellowSprite;

    private Rigidbody2D body;
    private Tilemap tiles;

    private BodyColor color;
    private bool visible;
    [SerializeField] private bool inBush; // track if in bush to make chameleon invisible
    public bool Visisble { get { return visible && !inBush; } }

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        tiles = GameObject.Find("Jungle").transform.GetChild(0).gameObject.GetComponent<Tilemap>();
        SetColor(startColor);
    }

    // FixedUpdate helps with the wall jitteriness
    void FixedUpdate()
    {
        // move
        if(body.velocity != Vector2.zero) {
            // apply friction
            Vector2 friction = body.velocity;
            friction.Normalize();
            friction *= -Friction * Time.deltaTime;
            body.velocity += friction;

            // check if crossed zero (if velocity now points in direction of friction)
            if(Vector2.Dot(friction, body.velocity) > 0) {
                body.velocity = Vector2.zero;
            }
        }
        Vector2 movement = GetMoveDirection();
        if(movement != Vector2.zero) {
            // accelerate
            body.velocity += Acceleration * GetMoveDirection() * Time.deltaTime;

            // cap speed
            if(body.velocity.magnitude > MaxSpeed) {
                body.velocity = body.velocity.normalized;
                body.velocity *= MaxSpeed;
            }
        }

        // rigidbody translates using velocity on its own
        
        // rotate to face move direction
        if(body.velocity != Vector2.zero) {
            float angle = Mathf.Atan2(body.velocity.y, body.velocity.x) * 180f / Mathf.PI; // converted to degrees
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        // check visibility from tile
        visible = true; // visible until proven hidden
        Tile currentTile = tiles.GetTile<Tile>(tiles.LocalToCell(transform.position));
        if(currentTile) {
            switch(color) {
                case BodyColor.Blue:
                    if(currentTile.sprite == blueSprite) {
                        visible = false;
                    }
                    break;
                case BodyColor.Green:
                    if(currentTile.sprite == greenSprite) {
                        visible = false;
                    }
                    break;
                case BodyColor.Yellow:
                    if(currentTile.sprite == yellowSprite) {
                        visible = false;
                    }
                    break;
                case BodyColor.Red:
                    if(currentTile.sprite == redSprite) {
                        visible = false;
                    }
                    break;
            }
        }

        // tongue shot
        if(Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.Joystick1Button0)) {

        }

        //adjusts field of vision
        fieldOfView.SetAimDirection(movement);
        fieldOfView.SetOrigin(transform.position);
    }

    private void SetColor(BodyColor color) {
        this.color = color;
        switch(color) {
            case BodyColor.Blue:
                GetComponent<SpriteRenderer>().color = Color.blue;
                break;
            case BodyColor.Green:
                GetComponent<SpriteRenderer>().color = Color.green;
                break;
            case BodyColor.Red:
                GetComponent<SpriteRenderer>().color = Color.red;
                break;
            case BodyColor.Yellow:
                GetComponent<SpriteRenderer>().color = Color.yellow;
                break;
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Bush") {
            SetColor(collision.gameObject.GetComponent<BushScript>().color);
            inBush = true;
        }
        else if(collision.gameObject.tag == "Fly") {
            Destroy(collision.gameObject);
        }

        else if (collision.gameObject.tag == "Enemy Vision")
        {
            //Debug.Log("COLLISION");
            transform.position = Vector3.zero;
        }
    }

    // detect that the chameleon has left its bush. Bushes must not overlap for this to work
    public void OnTriggerExit2D(Collider2D collision) {
        if(collision.gameObject.tag == "Bush") {
            inBush = false;
        }
    }

    private Vector2 GetMoveDirection() {
        // check for gamepad
        Vector2 joystick = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if(joystick != Vector2.zero) {
            return joystick;
        }

        // check for keyboard
        Vector2 result = new Vector2();
        if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) { // right
            result.x += 1;
        }
        if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) { // left
            result.x += -1;
        }
        if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) { // up
            result.y += 1;
        }
        if(Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) { // down
            result.y += -1;
        }
        if(result != Vector2.zero) {
            result.Normalize();
        }
        return result;
    }
}
