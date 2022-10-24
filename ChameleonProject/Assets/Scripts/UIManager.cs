using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using Unity.UIElements;
using TMPro;

public class UIManager : MonoBehaviour
{
    private GameObject gui;

    private TextMeshProUGUI levelValue;
    private Image colorUIimage; // image?
    private TextMeshProUGUI progressValue;
    private TextMeshProUGUI progressCap;
    private TextMeshProUGUI visibilityValue;
    private TextMeshProUGUI objectiveLabel;

    private void Awake()
    {
        gui = GameObject.Find("GUI");
        if(gui == null)
        {
            Debug.LogError("Missing \"GUI\" Object");
            return;
        }

        // Gameplay UI (parent)
        GameObject gameplayUI = gui.transform.GetChild(0).gameObject;
        if (gameplayUI == null)
        {
            Debug.LogError("Missing \"GameplayUI\" Object");
            return;
        }

        // Level UI
        GameObject levelUI = gameplayUI.transform.GetChild(0).gameObject;
        if (levelUI == null)
        {
            Debug.LogError("Missing \"LevelUI\" Object");
            return;
        }
        levelValue = levelUI.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();

        // Color UI
        GameObject colorUI = gameplayUI.transform.GetChild(1).gameObject;
        if (colorUI == null)
        {
            Debug.LogError("Missing \"ColorUI\" Object");
            return;
        }
        GameObject colorUIobj = colorUI.transform.GetChild(1).GetChild(1).gameObject;
        colorUIimage = colorUIobj.GetComponent<Image>();

        // Progress UI
        GameObject progressUI = gameplayUI.transform.GetChild(2).gameObject;
        if (progressUI == null)
        {
            Debug.LogError("Missing \"ProgressUI\" Object");
            return;
        }
        progressValue = progressUI.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();
        progressCap = progressUI.transform.GetChild(3).gameObject.GetComponent<TextMeshProUGUI>();

        // Visibility UI
        GameObject visibilityUI = gameplayUI.transform.GetChild(3).gameObject;
        if (visibilityUI == null)
        {
            Debug.LogError("Missing \"VisibilityUI\" Object");
            return;
        }
        visibilityValue = visibilityUI.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();

        // Objective UI
        GameObject objectiveUI = gameplayUI.transform.GetChild(4).gameObject;
        if (objectiveUI == null)
        {
            Debug.LogError("Missing \"ObjectiveUI\" Object");
            return;
        }
        objectiveLabel = objectiveUI.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
    }

    public bool UpdateLevelUI(string levelNumber)
    {
        levelValue.text = levelNumber;
        return true;
    }

    public bool UpdateColorUI(Color chameleonColor)
    {
        colorUIimage.color = chameleonColor;
        return true;
    }
    

    public bool UpdateProgressUI(string fliesEaten)
    {
        progressValue.text = fliesEaten;
        return true;
    }

    public bool UpdateProgressCapUI(string numberOfCollectables)
    {
        progressCap.text = numberOfCollectables;
        return true;
    }


    public bool UpdateVisibilityUI(string visibility)
    {
        visibilityValue.text = visibility;
        return true;
    }

    public bool UpdateObjectiveUI(string objective)
    {
        objectiveLabel.text = objective;
        return true;
    }
}
