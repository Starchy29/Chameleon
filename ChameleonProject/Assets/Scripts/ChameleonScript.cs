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
    public Sprite openEye;
    public Sprite closedEye;
    public Sprite squintEye;

    [SerializeField] private BodyColor startColor; // allows each level to start as a different color
    [SerializeField] private float Friction;
    [SerializeField] private float Acceleration;
    [SerializeField] private float MaxSpeed;

    // Tile color sprites to be set as prefab, used to check the tile this is standing on
    [SerializeField] private Sprite redSprite;
    [SerializeField] private Sprite blueSprite;
    [SerializeField] private Sprite greenSprite;
    [SerializeField] private Sprite brownSprite;

    // animation variables
    [SerializeField] private Sprite[] animationFrames; // set as prefab only
    private int currentFrame;
    private float animationTimer;
    private const float FRAMERATE = 0.05f; // duration of each frame

    private Rigidbody2D body;
    private Tilemap tiles;

    private BodyColor color;
    private bool onMatchingTile;
    private bool inBush; // track if in bush to make chameleon invisible
    public bool Visible { get { return !onMatchingTile && !inBush; } }

    private GameManager manager;
    private UIManager uiManager;

    private SoundManager soundManager;
    Tile currentTile;

    private bool isMoving = false;

    void Start()
    {
        manager = GameManager.Instance;
        uiManager = manager.GetComponent<UIManager>();
        body = GetComponent<Rigidbody2D>();
        //GameObject tempGrid = GameObject.Find("Grid-" + manager.Level);
        try
        {
            GameObject tempGrid = GameObject.Find("Grid"); // Grid needs to be labeled "Grid"
            tiles = tempGrid.transform.GetChild(1).gameObject.GetComponent<Tilemap>(); // Base layer is 2 // used to identify tile
            //Debug.Log("Init Chameleon");
        }
        catch
        {
            Debug.LogError("Make sure to name the grid properly \"Grid\"");
        }

        SetColor(startColor);

        soundManager = GetComponent<SoundManager>();
    }

    // FixedUpdate helps with the wall jitteriness
    //private void FixedUpdate()
    //{
    //    UpdateChameleon();
    //}

    public void UpdateChameleon()
    {
        if(GameManager.Instance.IsLevelOver) {
            return; // prevent interactions after dying
        }

        if (body == null)
        {
            Debug.Log(gameObject.name);
            body = GetComponent<Rigidbody2D>();
        }

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

            isMoving = true;
        }

        //stops footstep sounds if not moving
        if (movement == Vector2.zero)
        {
            soundManager.InvokeAudio();
            isMoving = false;
        }

        // rigidbody translates using velocity on its own

        SpriteRenderer renderer = GetComponent<SpriteRenderer>();

        // set alpha to 1 in case it is made translucent later
        Color newColor = renderer.color;
        newColor.a = 1f;
        renderer.color = newColor;

        // rotate to face move direction
        if (body.velocity != Vector2.zero)
        {
            float angle = Mathf.Atan2(body.velocity.y, body.velocity.x) * 180f / Mathf.PI; // converted to degrees
            transform.rotation = Quaternion.Euler(0, 0, angle);

            // animate when moving only
            animationTimer -= Time.deltaTime;
            if (animationTimer <= 0)
            {
                // change frame
                animationTimer = FRAMERATE;
                currentFrame++;
                if (currentFrame >= animationFrames.Length)
                {
                    // loop when reaching end of array
                    currentFrame = 0;
                }
            }
        }
        else
        {
            // end animation when not moving
            currentFrame = 0;
            animationTimer = 0;

            // become translucent when not moving on matching color
            if (!Visible)
            {
                newColor = renderer.color;
                newColor.a = 0.5f;
                renderer.color = newColor;
            }
        }
        renderer.sprite = animationFrames[currentFrame];
        // check visibility from tile
        onMatchingTile = false; // visible until proven hidden
        currentTile = tiles.GetTile<Tile>(tiles.LocalToCell(transform.position));
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

                //plays a footstep sound based on the current tile
                switch (currentTile.name)
                {
                    case "Grass Tile":
                        if (isMoving == true)
                        {
                            soundManager.PlayGrassFootstep();
                            soundManager.StopWaterFootstep();
                            soundManager.StopRockFootstep();
                            soundManager.StopDirtFootstep();
                            soundManager.StopClayFootstep();
                        }
                        break;
                    case "Shallow Water Tile":
                        if (isMoving == true)
                        {
                            soundManager.PlayWaterFootstep();
                            soundManager.StopGrassFootstep();
                            soundManager.StopRockFootstep();
                            soundManager.StopDirtFootstep();
                            soundManager.StopClayFootstep();
                        }
                        break;
                    case "Wall Tile":
                        if (isMoving == true)
                        {
                            soundManager.PlayRockFootstep();
                            soundManager.StopGrassFootstep();
                            soundManager.StopWaterFootstep();
                            soundManager.StopDirtFootstep();
                            soundManager.StopClayFootstep();
                        }
                        break;
                    case "Clay Tile":
                        if (isMoving == true)
                        {
                            soundManager.PlayClayFootstep();
                            soundManager.StopGrassFootstep();
                            soundManager.StopRockFootstep();
                            soundManager.StopWaterFootstep();
                            soundManager.StopDirtFootstep();
                        }
                        break;
                    case "Dirt Tile":
                        if (isMoving == true)
                        {
                            soundManager.PlayDirtFootstep();
                            soundManager.StopGrassFootstep();
                            soundManager.StopRockFootstep();
                            soundManager.StopWaterFootstep();
                            soundManager.StopClayFootstep();
                        }
                        break;
                }

                // Updates Visibility UI
                uiManager.UpdateVisibilityUI(Visible ? openEye : (body.velocity != Vector2.zero ? squintEye : closedEye)); // yes this is a lot of logic for one line but it works right I promise
            }
    }

    private void SetColor(BodyColor color)
    {
        this.color = color;
        Color newColor = Color.white;
        switch (color)
        {
            case BodyColor.Blue:
                newColor = new Color(0.4f, 1.0f, 1.0f);
                break;
            case BodyColor.Green:
                newColor = new Color(0.2f, 0.7f, 0.2f);
                break;
            case BodyColor.Red:
                newColor = new Color(0.8f, 0.1f, 0.1f);
                break;
            case BodyColor.Brown:
                newColor = new Color(0.4f, 0.2f, 0.1f);
                break;
        }
        GetComponent<SpriteRenderer>().color = newColor;

        // Update UI
        uiManager.UpdateColorUI(newColor);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(GameManager.Instance.IsLevelOver) {
            return; // prevent interactions after dying
        }

        if (collision.gameObject.tag == "Bush")
        {
            SetColor(collision.gameObject.GetComponent<BushScript>().color);
            inBush = true;
            soundManager.PlayBushSound();
        }
        else if (collision.gameObject.tag == "Fly")
        {
            EatFly();
            Destroy(collision.gameObject);
            soundManager.PlayEatingSound();
        }

        else if (collision.gameObject.tag == "Enemy Vision")
        {
            //Debug.Log("COLLISION");
            transform.position = Vector3.zero;
        }

        else if (collision.gameObject.tag == "Snake")
        {

            soundManager.PlaySnakeDeathSound();
            soundManager.StopFootsteps();
            GameManager.Instance.PlayerDead();
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
        //localFlyCount++;
        //manager.UpdateFlyCount(localFlyCount);
        manager.EatFly();
    }
}
