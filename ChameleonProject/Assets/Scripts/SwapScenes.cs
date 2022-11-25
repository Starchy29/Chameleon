using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwapScenes : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().name == "Level-1")
        {
            BGSoundScript.Instance.GetComponent<AudioSource>().Play();
        }
        if (SceneManager.GetActiveScene().name == "Menu")
        {
             BGSoundScript.Instance.GetComponent<AudioSource>().Stop();
        }
    }
}
