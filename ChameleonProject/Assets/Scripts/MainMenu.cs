using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    static AudioSource buttonPress;

    private void Start()
    {
        buttonPress = gameObject.AddComponent<AudioSource>();
        buttonPress.clip = Resources.Load<AudioClip>("SoundEffects/button-click2");
    }
    /// <summary>
    /// Loads desired scene
    /// </summary>
    /// <param name="scene">The scenes number</param>
    public void LoadScene(int scene)
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + scene); // This gets the next scene in the build index - not necessary at the moment
        SceneManager.LoadScene(scene);

        buttonPress.Play();
    }

    /// <summary>
    /// Loads tutorial level
    /// </summary>
    public void LoadTutorial()
    {
        SceneManager.LoadScene(11);

        buttonPress.Play();
    }

    /// <summary>
    /// Quits the game.
    /// </summary>
    public void QuitApplication()
    {
        Application.Quit();
    }
}
