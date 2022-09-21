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

    [SerializeField] private BodyColor startColor; // allows each level to start as a different color
    [SerializeField] private float Friction;
    [SerializeField] private float Acceleration;
    [SerializeField] private float MaxSpeed;

    private Rigidbody2D body;
    private Tilemap tiles;

    private BodyColor color;
    private bool visible;
    public bool Visisble { get { return visible; } }

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        tiles = GameObject.Find("Jungle").transform.GetChild(0).gameObject.GetComponent<Tilemap>();
        SetColor(startColor);
    }

    // Update is called once per frame
    void Update()
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

        transform.position = new Vector3(transform.position.x + body.velocity.x * Time.deltaTime, transform.position.y + body.velocity.y * Time.deltaTime, transform.position.z);

        // check visibility from tile
        TileBase currentTile = tiles.GetTile(new Vector3Int(0, 0, 0));
        //currentTile.GetTileData(

        // tongue shot
        if(Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.Joystick1Button0)) {

        }
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
        }
        else if(collision.gameObject.tag == "Fly") {
            
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
