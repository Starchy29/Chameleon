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
    private LevelManager levelManager;
    private Level currentLevel;
    private GameState state;
    private static GameManager instance;
    private static UIManager ui;
    private float gameTimer;
    private float timerSeconds;
    private int timerMinutes;
    private PlayerData playerData;

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
    public static UIManager UI
    {
        get { return ui; }
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

        // Init LevelManager
        levelManager = new LevelManager();
        //Debug.Log("Init Level Data");

        // Init UI
        currentLevel = GetLevelFromScene(SceneManager.GetActiveScene()); // Get current level
        SceneManager.sceneLoaded += OnSceneLoaded;
        playerData = new PlayerData();
    }

    private void Update()
    {
        switch (state)
        {
            case GameState.PLAY:
                gameTimer += Time.deltaTime;
                timerSeconds = gameTimer % 60;
                timerMinutes = (int)gameTimer / 60;
                ui.UpdateTimerUI(timerMinutes.ToString()+":"+timerSeconds.ToString("f2"));
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
            return levelManager.Levels[levelNumber - 1];
        }
        return null;
    }
    
    public void EatFly()
    {
        ui.UpdateProgressUI(playerData.EatFly().ToString());

        // Checks if enough flies are eaten to go to next level
        if (playerData.FliesEaten >= currentLevel.MaxFlies)
        {
            ui.UpdateObjectiveUI("You have enough Flies!\nGet to your tree");
        }
    }

    /// <summary>
    /// Checks if player can win
    /// </summary>
    /// <returns>If player can win - bool</returns>
    public void CheckWin()
    {
        // Should be a player method
        //Debug.Log("Check finish");
        if (playerData.FliesEaten >= currentLevel.MaxFlies)
        {
            //Debug.Log("You have enough Flies!");
            ui.UpdateObjectiveUI("You have enough Flies!\nGet to your tree");
            playerData.Timestamp = gameTimer;
            levelManager.UpdateLevelData(currentLevel, playerData); // Store data
            // Needs to calculate stars - compare against level data
            NextScene();
            return;
        }

        ui.UpdateObjectiveUI("Eat more flies!");
        //Debug.Log("Did not eat enough flies");
    }

    /// <summary>
    /// Loads next scene
    /// </summary>
    private void NextScene()
    {
        // check if there are more levels
        if(currentLevel.Number < levelManager.Levels.Count)
        {
            //Debug.Log("Next Level!");
            ResetVariables();
            playerData = new PlayerData(); // new set

            // Jank - updating current level depends on scene while scene (start method) depends on current level
            // So currentLevel needs to be updated before 
            int nextLevelIndex = (currentLevel.Number - 1) + 1; // +-1 makes sense relative to index
            currentLevel = levelManager.Levels[nextLevelIndex];
            //Scene nextScene = SceneManager.GetSceneByBuildIndex(SceneManager.GetActiveScene().buildIndex + 1); // Gets next scene
            //currentLevel = GetLevelFromScene(nextScene); // get new level

            // Load Scene - level# needs to be updated
            //SceneManager.LoadScene(nextScene.buildIndex); // This gets the next scene in the build index
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); // This gets the next scene in the build index
            //currentLevel = levelManager.Levels[currentLevel.Number]; // gets next level
            //SceneManager.LoadScene(currentLevel.Number); // Could use current level if sync'd up

            return;
        }
        else
        {
            state = GameState.WIN;
            levelManager.Print();
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.LoadScene(0); // This gets the next scene in the build index
            Destroy(gameObject);
        }
        Debug.Log("There are no more levels");
    }

    /// <summary>
    /// Triggers once scene is loaded
    /// </summary>
    /// <param name="scene">Scene that loaded</param>
    /// <param name="mode">Mode of loading</param>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) 
    {
        // Get Scene UI
        if (!ui.GetSceneUI())
        {
            Debug.LogError("GetSceneUI Failed");
            return;
        }

        //Debug.Log("Init Level " + currentLevel.Number);
        Debug.Log("OnSceneLoaded: " + scene.name);
        //Debug.Log(mode);

        // Update Scene UI
        ui.UpdateLevelUI(currentLevel.Number.ToString());
        ui.UpdateColorUI(Color.green);
        ui.UpdateProgressUI(playerData.FliesEaten.ToString());
        ui.UpdateProgressCapUI(currentLevel.MaxFlies.ToString());
        //ui.UpdateVisibilityUI();
        ui.UpdateObjectiveUI("Eat flies");
        //Debug.Log("Init UI"); // Debug

        // Init GameState
        ResetVariables();
        state = GameState.PLAY;
        //Debug.Log("Init Game State"); // Debug
    }

    /// <summary>
    /// Loads scene again
    /// </summary>
    public void RestartLevel()
    {
        Die();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Resets scene and player data
    /// </summary>
    private void Die()
    {
        playerData.Die(); // adds death resets flies
        gameTimer = 0f; // resets timer
        //playerData.Print();
        ui.UpdateProgressUI(playerData.FliesEaten.ToString());
    }

    private void ResetVariables()
    {
        gameTimer = 0f; // resets timer
        playerData.Print();
    }

    /// <summary>
    /// Quits the game.
    /// </summary>
    public void QuitApplication()
    {
        Application.Quit();
    }
}
