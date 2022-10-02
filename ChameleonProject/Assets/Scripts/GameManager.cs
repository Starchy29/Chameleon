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

    private void Awake()
    {
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
}
