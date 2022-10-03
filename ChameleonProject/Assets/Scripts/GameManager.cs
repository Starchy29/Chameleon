using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject player;
    private GameObject[] flies;
    private int flyCount;

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

    public virtual void Awake()
    {
        // Singleton instantiation
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(gameObject); // dont need yet - will allow to save data when going to menu
        }
        else
        {
            Destroy(gameObject); // why does it need to be destroyed of contains instance
        }

        state = GameState.PLAY;
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
}
