using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using Unity.UIElements;
using TMPro;

public class UIManager : MonoBehaviour
{
    private TextMeshProUGUI levelValue;
    private Image colorUIimage;
    private TextMeshProUGUI progressValue;
    private TextMeshProUGUI progressCap;
    //private TextMeshProUGUI visibilityValue; // Visibility text
    private SpriteRenderer visibilityEye; 
    private TextMeshProUGUI objectiveLabel;
    private TextMeshProUGUI timerValue;

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

        // - Gameplay UI (parent)
        GameObject gameplayUI = gui.transform.GetChild(0).gameObject;
        if (gameplayUI == null)
        {
            Debug.LogError("Missing \"GameplayUI\" Object");
            return false;
        }

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

        GetSceneAchievementUI(gui);
        return true;
    }

    public bool GetSceneAchievementUI(GameObject gui)
    {
        GameObject achievementUI = gui.transform.GetChild(2).gameObject;
        if (achievementUI == null)
        {
            Debug.LogError("Missing \"AchievementUI\" Object");
            return false;
        }

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
    public bool UpdateTimerUI(string time)
    {
        timerValue.text = time;
        return true;
    }

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
}
