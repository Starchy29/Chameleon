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
        END,
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
    private ChameleonScript chameleon;
    private BirdMovement bird1;
    private PlayerData playerData;
    private float deathLinger = 0; // timer to track how long the linger after dying has lasted
    private const float LINGER_DURATION = 1.0f; // number of seconds to wait after dying before resetting
    private bool isTutorial = false;

    public bool IsLevelOver { 
        get 
        {
            if(deathLinger > 0)
            {
                return true;
            }
            return false; 
        } 
    } // track if level is lost but not restarted yet

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

    public bool IsTutorial
    {
        get { return isTutorial; }
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

        Scene currentScene = SceneManager.GetActiveScene();
        if(currentScene.buildIndex == 11){ isTutorial = true; }
        else { isTutorial = false; }

        // Init LevelManager
        levelManager = new LevelManager(isTutorial);
        //Debug.Log("Init Level Data");

        // Init UI
        currentLevel = GetLevelFromScene(currentScene); // Get current level
        SceneManager.sceneLoaded += OnSceneLoaded;
        playerData = new PlayerData(0f);

        GetChameleon();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            state = GameState.PAUSE;
            ui.OpenPauseMenu();
        }

        switch (state)
        {
            case GameState.PLAY:
                // normal update during game
                gameTimer += Time.deltaTime;
                timerSeconds = gameTimer % 60;
                timerMinutes = (int)gameTimer / 60;
                ui.UpdateTimerUI(timerMinutes.ToString()+":"+timerSeconds.ToString("f2"));
                break;
            case GameState.PAUSE:
                break;
            case GameState.END:
                // pause for a moment after death
                if (deathLinger > 0)
                {
                    deathLinger -= Time.deltaTime;
                    if (deathLinger <= 0)
                    {
                        // restart scene
                        deathLinger = 0;
                        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                    }
                }
                break;
            case GameState.WIN:
                break;
        }
    }

    private void FixedUpdate()
    {
        switch (state)
        {
            case GameState.PLAY:
                if (chameleon)
                {
                    chameleon.UpdateChameleon();
                }
                else
                {
                    GetChameleon();
                }
                //bird1.UpdateBird();
                break;
            case GameState.PAUSE:
                if (chameleon)
                {
                    chameleon.Stop();
                }
                break;
            case GameState.END:
                if (chameleon)
                {
                    chameleon.Stop();
                }
                break;
            case GameState.WIN:
                break;
        }
    }

    private void GetChameleon()
    {
        // Get chameleon
        GameObject chameleonObj = GameObject.Find("Chameleon");
        if (chameleonObj != null)
        {
            chameleon = chameleonObj.GetComponent<ChameleonScript>();
        }
        else { Debug.LogError("Coundn't find Chameleon Object"); }
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
            Level level = null;
            try
            {
                level = levelManager.Levels[levelNumber - 1];
            }
            catch
            {
                Debug.Log("Need to create a level in LevelManager");
            }

            return level;
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
        // Should probably be a player method
        //Debug.Log("Check finish");
        if (playerData.FliesEaten >= currentLevel.MaxFlies)
        {
            playerData.Timestamp = gameTimer;
            PlayerData pd = levelManager.UpdateLevelData(currentLevel, playerData); // Store data
            ui.SetEndLevelScreen();
            ui.UpdateEndLevelScreen(currentLevel.Number, pd, isTutorial);
            playerData.ResetDeaths();
            state = GameState.END;
            return;
        }

        ui.UpdateObjectiveUI("Eat more flies!");
        //Debug.Log("Did not eat enough flies");
    }

    /// <summary>
    /// Loads next scene
    /// </summary>
    public void NextScene()
    {
        // check if there are more levels
        if(currentLevel.Number < levelManager.Levels.Count)
        {
            //Debug.Log("Next Level!");
            ResetVariables();
            playerData = new PlayerData(0f); // new set

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
        //if (isTutorial) { SceneManager.sceneLoaded -= OnSceneLoaded; } // Make sure it doesnt trugger when going back to menu
        // Get Scene UI
        if (!ui.GetSceneUI())
        {
            Debug.LogError("GetSceneUI Failed");
            return;
        }

        //Debug.Log("Init Level " + currentLevel.Number);
        //Debug.Log("OnSceneLoaded: " + scene.name);
        //Debug.Log(mode);

        // Update Scene UI
        ui.UpdateLevelUI(currentLevel.Number.ToString());
        ui.UpdateColorUI(Color.green);
        ui.UpdateProgressUI(playerData.FliesEaten.ToString());
        ui.UpdateProgressCapUI(currentLevel.MaxFlies.ToString());
        //ui.UpdateVisibilityUI();
        ui.UpdateObjectiveUI("Eat flies");
        //Debug.Log("Init UI"); // Debug

        //chameleon = GameObject.Find("Chameleon").GetComponent<ChameleonScript>();
        //bird1 = GameObject.Find("Bird").GetComponent<BirdMovement>();

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
        deathLinger = LINGER_DURATION;
        GameObject.Find("Black Overlay").GetComponent<BlackScreen>().CutToBlack();

        gameTimer = 0f; // resets timer
        //playerData.Print();
        playerData.ResetFlies();
        ui.UpdateProgressUI(playerData.FliesEaten.ToString());
    }

    /// <summary>
    /// Resets scene and player data
    /// </summary>
    public void PlayerDead()
    {
        if (state == GameState.END)
        {
            return;
        }
        playerData.Die(); // adds death resets flies
        RestartLevel();
        state = GameState.END;
    }

    private void ResetVariables()
    {
        gameTimer = 0f; // resets timer
        playerData.Print();
    }

    public void OpenMainMenu()
    {
        if (isTutorial)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Destroy(gameObject);
        }
        SceneManager.LoadScene(0);
    }

    public void ResumeGame()
    {
        state = GameState.PLAY;
        ui.ClosePauseMenu();
    }
}
