using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BGSoundScript : MonoBehaviour
{
    private static BGSoundScript instance;
    public static BGSoundScript Instance
    {
        get { return instance; }
    }

    // Start is called before the first frame update
    void Start()
    {

      //  instance.GetComponent<AudioSource>().Play();
    }


    //keeps BGM running between levels
    void Awake()
    {
        //if (instance != null && instance != this)
        //{
        //    Destroy(this.gameObject);
        //    return;
        //}
        //else
        //{
        //    instance = this;
        //    DontDestroyOnLoad(this.gameObject);
        //}

    }


}