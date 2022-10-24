using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        PLAY,
        PAUSE,
        WIN
    }
    private int flyCount;
    private LevelData levelData;
    private Level currentLevel;
    private GameState state;
    private static GameManager instance;
    private static UIManager ui;

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
                    ui = obj.AddComponent<UIManager>();
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
            return currentLevel.Number;
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

        // Init LevelData
        levelData = new LevelData();
        Debug.Log("Init Level Data");

        // Init UI
        currentLevel = GetLevelFromScene(SceneManager.GetActiveScene()); // Get current level
        Debug.Log("Init Level " + currentLevel.Number);

        // Init GameState
        state = GameState.PLAY;
        Debug.Log("Init Game State");
    }
    private void Start()
    {
        ui.UpdateLevelUI(currentLevel.Number.ToString());
        ui.UpdateProgressUI(flyCount.ToString());
        ui.UpdateProgressCapUI(currentLevel.MaxFlies.ToString());
        ui.UpdateObjectiveUI("Get to your home tree");
        Debug.Log("Init UI");
    }
    private void Update()
    {
        switch (state)
        {
            case GameState.PLAY:
                break;
            case GameState.PAUSE:
                break;
            case GameState.WIN:
                Debug.Log("WIN");
                break;
        }
    }

    /// <summary>
    /// Gets level from scene name
    /// Checks if started from another level
    /// </summary>
    private Level GetLevelFromScene(Scene scene)
    {
        string[] sceneNameArray = scene.name.Split('-'); // Scene name convention "Level-<#>"

        if (sceneNameArray.Length >= 1)
        {
            int levelNumber = int.Parse(sceneNameArray[1]);
            return levelData.Levels[levelNumber - 1];
        }
        return null;
    }
    
    /// <summary>
    /// Updates manager fly count
    /// </summary>
    /// <param name="playerFlyCount">number of flies eaten in the current level</param>
    public void UpdateFlyCount(int playerFlyCount)
    {
        flyCount = playerFlyCount;
        ui.UpdateProgressUI(flyCount.ToString());
        Debug.Log("FlyCount: " + flyCount);
    }

    /// <summary>
    /// Checks if player can win
    /// </summary>
    /// <returns>If player can win - bool</returns>
    public void AteEnoughFlies()
    {
        // Should be a player method
        Debug.Log("Check finish");
        if (flyCount >= currentLevel.MaxFlies)
        {
            Debug.Log("You have enough Flies!");
            NextScene();
            return;
        }

        Debug.Log("Did not eat enough flies");
    }

    /// <summary>
    /// Loads next scene
    /// </summary>
    private void NextScene()
    {
        // check if there are more levels
        if(currentLevel.Number < levelData.Levels.Count)
        {
            Debug.Log("Next Level!");
            ResetVariables();

            // Jank - updating current level depends on scene while scene (start method) depends on current level
            // So currentLevel needs to be updated before 


            int nextLevelIndex = (currentLevel.Number - 1) + 1; // +-1 makes sense relative to index
            currentLevel = levelData.Levels[nextLevelIndex];
            //Scene nextScene = SceneManager.GetSceneByBuildIndex(SceneManager.GetActiveScene().buildIndex + 1); // Gets next scene
            //currentLevel = GetLevelFromScene(nextScene); // get new level

            // Load Scene - level# needs to be updated
            //SceneManager.LoadScene(nextScene.buildIndex); // This gets the next scene in the build index
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); // This gets the next scene in the build index

            Debug.Log("Level " + currentLevel.Number);
            //currentLevel = levelData.Levels[currentLevel.Number]; // gets next level
            //SceneManager.LoadScene(currentLevel.Number); // Could use current level if sync'd up
            ui.UpdateLevelUI(currentLevel.Number.ToString());

            return;
        }
        else
        {
            state = GameState.WIN;
            SceneManager.LoadScene(0); // This gets the next scene in the build index
            Destroy(gameObject);
            ui.UpdateObjectiveUI("You Win");
        }
        Debug.Log("There are no more levels");
    }

    /// <summary>
    /// Loads scene again
    /// </summary>
    public void RestartLevel()
    {
        ResetVariables();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Resets level variables
    /// </summary>
    private void ResetVariables()
    {
        UpdateFlyCount(0); // Reset fly count
    }
    /// <summary>
    /// Quits the game.
    /// </summary>
    public void QuitApplication()
    {
        Application.Quit();
    }
}
