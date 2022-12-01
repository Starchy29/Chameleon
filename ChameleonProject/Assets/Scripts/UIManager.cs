using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using Unity.UIElements;
using TMPro;

public class UIManager : MonoBehaviour
{
    private GameObject gameplayUI;
    private GameObject menuUI;
    private GameObject achievementUI;
    private GameObject endUI;
    // UI Ref
    [SerializeField] private Sprite emptyStar;
    [SerializeField] private Sprite fullStar;

    // Gameplay UI
    private TextMeshProUGUI levelValue;
    private Image colorUIimage;
    private TextMeshProUGUI progressValue;
    private TextMeshProUGUI progressCap;
    //private TextMeshProUGUI visibilityValue; // Visibility text
    private SpriteRenderer visibilityEye; 
    private TextMeshProUGUI objectiveLabel;
    // Achievement UI
    private TextMeshProUGUI timerValue;
    // End UI
    private TextMeshProUGUI levelTitle;
    private Image timerStarImage;
    private TextMeshProUGUI timerStarValue;
    private Image flyStarImage;
    private TextMeshProUGUI flyStarValue;
    private Image deathStarImage;
    private TextMeshProUGUI deathStarValue;

    private void Awake()
    {
        emptyStar = Resources.Load<Sprite>("Sprites/star-empty");
        fullStar = Resources.Load<Sprite>("Sprites/star-full");
    }

    /// <summary>
    /// Get UI objects of current scene
    /// </summary>
    /// <returns>If the method succeeded</returns>
    public bool GetSceneUI()
    {
        GameObject gui = GameObject.Find("GUI");
        if (gui == null)
        {
            Debug.LogError("Missing \"GUI\" Object");
            return false;
        }

        // - Gameplay UI
        gameplayUI = gui.transform.GetChild(0).gameObject;
        if (gameplayUI == null)
        {
            Debug.LogError("Missing \"GameplayUI\" Object");
            return false;
        }
        GetGameplayUI(gameplayUI);
        gameplayUI.SetActive(true);

        // - Menu UI
        menuUI = gui.transform.GetChild(1).gameObject;
        if (menuUI == null)
        {
            Debug.LogError("Missing \"GameplayUI\" Object");
            return false;
        }
        GetMenuUI(menuUI);
        menuUI.SetActive(false);

        // - Achievement UI
        achievementUI = gui.transform.GetChild(2).gameObject;
        if (achievementUI == null)
        {
            Debug.LogError("Missing \"AchievementUI\" Object");
            return false;
        }
        GetSceneAchievementUI(achievementUI);
        achievementUI.SetActive(true);

        // - End UI
        endUI = gui.transform.GetChild(3).gameObject;
        if (endUI == null)
        {
            Debug.LogError("Missing \"GameplayUI\" Object");
            return false;
        }
        GetEndUI(endUI);
        endUI.SetActive(false);


        return true;
    }

    private bool GetGameplayUI(GameObject gameplayUI)
    {
        // -- Level UI
        GameObject levelUI = gameplayUI.transform.GetChild(0).gameObject;
        if (levelUI == null)
        {
            Debug.LogError("Missing \"LevelUI\" Object");
            return false;
        }
        levelValue = levelUI.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();

        // -- Color UI
        GameObject colorUI = gameplayUI.transform.GetChild(1).gameObject;
        if (colorUI == null)
        {
            Debug.LogError("Missing \"ColorUI\" Object");
            return false;
        }
        GameObject colorUIobj = colorUI.transform.GetChild(1).GetChild(1).gameObject;
        colorUIimage = colorUIobj.GetComponent<Image>();

        // Progress UI
        GameObject progressUI = gameplayUI.transform.GetChild(2).gameObject;
        if (progressUI == null)
        {
            Debug.LogError("Missing \"ProgressUI\" Object");
            return false;

        }
        progressValue = progressUI.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();
        progressCap = progressUI.transform.GetChild(3).gameObject.GetComponent<TextMeshProUGUI>();

        // Visibility UI
        GameObject visibilityUI = gameplayUI.transform.GetChild(3).gameObject;
        if (visibilityUI == null)
        {
            Debug.LogError("Missing \"VisibilityUI\" Object");
            return false;

        }
        //visibilityValue = visibilityUI.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();
        visibilityEye = visibilityUI.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>();

        // Objective UI
        GameObject objectiveUI = gameplayUI.transform.GetChild(4).gameObject;
        if (objectiveUI == null)
        {
            Debug.LogError("Missing \"ObjectiveUI\" Object");
            return false;

        }
        objectiveLabel = objectiveUI.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
        return true;
    }
    private bool GetMenuUI(GameObject menuUI)
    {
        // -- Resume Button
        GameObject resumeSceneButtonObj = menuUI.transform.GetChild(1).gameObject;
        if (resumeSceneButtonObj == null)
        {
            Debug.LogError("Missing \"Resume-Button\" Object");
            return false;
        }
        Button resumeButton = resumeSceneButtonObj.GetComponent<Button>();
        resumeButton.onClick.AddListener(GameManager.Instance.ResumeGame);

        // -- Menu Button
        GameObject menuSceneButtonObj = menuUI.transform.GetChild(2).gameObject;
        if (menuSceneButtonObj == null)
        {
            Debug.LogError("Missing \"Menu-Button\" Object");
            return false;
        }
        Button menuButton = menuSceneButtonObj.GetComponent<Button>();
        menuButton.onClick.AddListener(GameManager.Instance.OpenMainMenu);

        return true;
    }
    private bool GetSceneAchievementUI(GameObject achievementUI)
    {
        // -- Level UI
        GameObject timerUI = achievementUI.transform.GetChild(0).gameObject;
        if (timerUI == null)
        {
            Debug.LogError("Missing \"LevelUI\" Object");
            return false;
        }
        timerValue = timerUI.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();
        return true;
    }
    private bool GetEndUI(GameObject endUI)
    {
        // -- Title UI
        GameObject levelTitleUI = endUI.transform.GetChild(0).gameObject;
        if (levelTitleUI == null)
        {
            Debug.LogError("Missing \"LevelTitleUI\" Object");
            return false;
        }
        levelTitle = levelTitleUI.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();

        // -- Timer Star
        GameObject timerStarUI = endUI.transform.GetChild(1).gameObject;
        if (timerStarUI == null)
        {
            Debug.LogError("Missing \"TimerStarUI\" Object");
            return false;
        }
        timerStarImage = timerStarUI.transform.GetChild(0).gameObject.GetComponent<Image>();
        timerStarValue = timerStarUI.transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>();

        // -- Fly Star
        GameObject flyStarUI = endUI.transform.GetChild(2).gameObject;
        if (flyStarUI == null)
        {
            Debug.LogError("Missing \"FlyStarUI\" Object");
            return false;
        }
        flyStarImage = flyStarUI.transform.GetChild(0).gameObject.GetComponent<Image>();
        flyStarValue = flyStarUI.transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>();

        // -- Death Star
        GameObject deathStarUI = endUI.transform.GetChild(3).gameObject;
        if (deathStarUI == null)
        {
            Debug.LogError("Missing \"DeathStarUI\" Object");
            return false;
        }
        deathStarImage = deathStarUI.transform.GetChild(0).gameObject.GetComponent<Image>();
        deathStarValue = deathStarUI.transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>();


        // -- Reset Button
        GameObject resetSceneButtonObj = endUI.transform.GetChild(4).gameObject;
        if (resetSceneButtonObj == null)
        {
            Debug.LogError("Missing \"DeathStarUI\" Object");
            return false;
        }
        Button resetButton = resetSceneButtonObj.GetComponent<Button>();
        resetButton.onClick.AddListener(GameManager.Instance.RestartLevel);

        // -- Next Button
        GameObject nextSceneButtonObj = endUI.transform.GetChild(5).gameObject;
        if (nextSceneButtonObj == null)
        {
            Debug.LogError("Missing \"DeathStarUI\" Object");
            return false;
        }
        Button nextButton = nextSceneButtonObj.GetComponent<Button>();
        if (GameManager.Instance.IsTutorial)
        {
            nextButton.onClick.AddListener(GameManager.Instance.OpenMainMenu);
        }
        else
        {
            nextButton.onClick.AddListener(GameManager.Instance.NextScene);
        }

        return true;
    }

    // Gameplay UI
    /// <summary>
    /// Updates the level number
    /// </summary>
    /// <param name="levelNumber">The number of the level</param>
    /// <returns>If the method succeeded</returns>
    public bool UpdateLevelUI(string levelNumber)
    {
        levelValue.text = levelNumber;
        return true;
    }
    /// <summary>
    /// Updates the color indicator to show the color of the chameleon
    /// </summary>
    /// <param name="chameleonColor">Color to update as. Should be chameleons color</param>
    /// <returns>If the method succeeded</returns>
    public bool UpdateColorUI(Color chameleonColor)
    {
        colorUIimage.color = chameleonColor;
        return true;
    }
    /// <summary>
    /// Updates number of flies eaten
    /// </summary>
    /// <param name="fliesEaten">Number of flies eaten</param>
    /// <returns>If the method succeeded</returns>
    public bool UpdateProgressUI(string fliesEaten)
    {
        progressValue.text = fliesEaten;
        return true;
    }
    /// <summary>
    /// Updates the number of required collectables to eat before being able to complete the level
    /// In this case our collectables are flies
    /// </summary>
    /// <param name="numberOfCollectables">Number of collectables to obtain</param>
    /// <returns>If the method succeeded</returns>
    public bool UpdateProgressCapUI(string numberOfCollectables)
    {
        progressCap.text = numberOfCollectables;
        return true;
    }
    /// <summary>
    /// Updates the visibility indicator of the player
    /// </summary>
    /// <param name="eyeImage">Sprite that represents if the player is visible or not</param>
    /// <returns>If the method succeeded</returns>
    public bool UpdateVisibilityUI(Sprite eyeImage)
    {
        //visibilityValue.text = visibility;
        visibilityEye.sprite = eyeImage;
        return true;
    }
    /// <summary>
    /// Updates the players objective on screen
    /// </summary>
    /// <param name="objective">The players objective in text</param>
    /// <returns>If the method succeeded</returns>
    public bool UpdateObjectiveUI(string objective)
    {
        objectiveLabel.text = objective;
        return true;
    }

    // Achievement UI
    public bool UpdateTimerUI(string time)
    {
        timerValue.text = time;
        return true;
    }

    // Menu UI
    public void OpenPauseMenu()
    {
        gameplayUI.SetActive(false);
        menuUI.SetActive(true);
        achievementUI.SetActive(true); // keep timer visable
        endUI.SetActive(false);
    }
    public void ClosePauseMenu()
    {
        gameplayUI.SetActive(true);
        menuUI.SetActive(false);
        achievementUI.SetActive(true);
        endUI.SetActive(false);
    }

    // End UI
    public bool SetEndLevelScreen()
    {
        gameplayUI.SetActive(false);
        menuUI.SetActive(false);
        achievementUI.SetActive(false);
        endUI.SetActive(true);
        return true;
    }
    public bool UpdateEndLevelScreen(int levelNumber, PlayerData playerData)
    {
        UpdateEndLevelUI(levelNumber);
        UpdateTimerStarUI(playerData);
        UpdateFlyStarUI(playerData);
        UpdateDeathStarUI(playerData);
        return true;
    }
    public void UpdateEndLevelUI(int levelNumber)
    {
        levelTitle.text = "Level " + levelNumber.ToString();
    }
    public void UpdateTimerStarUI(PlayerData playerData)
    {
        timerStarValue.text = playerData.Timestamp.ToString("f2")+'s';
        if (playerData.GotTimeStar())
        {
            //timerStarImage.color = Color.yellow;
            timerStarImage.sprite = fullStar;
        }
        else
        {
            timerStarImage.color = Color.gray;
            timerStarImage.sprite = emptyStar;
        }
    }
    public void UpdateFlyStarUI(PlayerData playerData)
    {
        flyStarValue.text = playerData.FliesEaten.ToString();
        if (playerData.GotFlyStar())
        {
            flyStarImage.sprite = fullStar;
        }
        else
        {
            flyStarImage.color = Color.gray;
            flyStarImage.sprite = emptyStar;
        }
    }
    public void UpdateDeathStarUI(PlayerData playerData)
    {
        deathStarValue.text = playerData.Deaths.ToString();
        if (playerData.GotDeathStar())
        {
            deathStarImage.sprite = fullStar;
        }
        else
        {
            deathStarImage.color = Color.gray;
            deathStarImage.sprite = emptyStar;
        }
    }
}
