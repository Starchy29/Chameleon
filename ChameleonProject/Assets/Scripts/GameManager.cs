using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject player;
    private GameObject[] flies;
    private int flyCount;
    private const int MAX_FLIES = 3;
    private int level = 0;

    public enum GameState
    {
        PLAY,
        PAUSE
    }
    private GameState state;
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>(); // Looks for existing
                if (instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(GameManager).Name;
                    instance = obj.AddComponent<GameManager>();
                }
            }
            return instance;
        }
    }
    public int FlyCount
    {
        get
        {
            return flyCount;
        }
    }
    public int Level
    {
        get
        {
            return level;
        }
    }
    public virtual void Awake()
    {
        // Singleton instantiation
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // why does it need to be destroyed of contains instance
        }

        state = GameState.PLAY;
    }
    private void Start()
    {
        // Scene name convention "Level-<#>"
        string[] sceneNameArray = SceneManager.GetActiveScene().name.Split('-');
        if(sceneNameArray.Length >= 1)
        {
            level = int.Parse(sceneNameArray[1]);
        }
        else
        {
            level++;
        }
        Debug.Log("Level " + level);
    }
    private void Update()
    {
        switch (state)
        {
            case GameState.PLAY:
                break;
            case GameState.PAUSE:
                break;
        }
    }

    public void UpdateFlyCount(int playerFlyCount)
    {
        flyCount = playerFlyCount;
        Debug.Log("FlyCount: " + flyCount);
    }

    /// <summary>
    /// Checks if player can win
    /// </summary>
    /// <returns>If player can win - bool</returns>
    public void AteEnoughFlies()
    {
        // Should be a player method
        if(flyCount >= MAX_FLIES)
        {
            NextScene();
            level++;
        }
        Debug.Log("Did not eat enough flies");
    }

    /// <summary>
    /// Loads next scene
    /// </summary>
    private void NextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); // This gets the next scene in the build index
    }

    /// <summary>
    /// Loads scene again
    /// </summary>
    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Quits the game.
    /// </summary>
    public void QuitApplication()
    {
        Application.Quit();
    }
}
