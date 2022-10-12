using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Level - probably could be struct
/// </summary>
public class Level
{
    private int number;
    private int maxFlies;
    private GameObject grid;

    // Default level
    public Level()
    {
        number = 0;
        maxFlies = 0;
        grid = null;
    }
    public Level(int number, int maxFlies, GameObject grid)
    {
        this.number = number;
        this.maxFlies = maxFlies;
        this.grid = grid;
    }

}
public class LevelData : ScriptableObject
{
    public static GameObject grid1;
    private List<Level> levels;

    public List<Level> Levels
    {
        get
        {
            return levels;
        }
    }


    public void Main()
    {
        Debug.Log("Punched");
    }

    public void Init()
    {
        levels = new List<Level>();
        CreateLevel(1, 3, grid1);
    }

    private void CreateLevel(int number, int maxFlies, GameObject grid)
    {
        Level level = new Level(1, 3, grid1);
        levels.Add(level);
    }
}
