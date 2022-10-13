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

    /// <summary>
    /// Default Level
    /// </summary>
    public Level()
    {
        number = 0;
        maxFlies = 0;
    }
    /// <summary>
    /// Variable Level
    /// </summary>
    /// <param name="number">Level number</param>
    /// <param name="maxFlies">Number of flies in level - to open door - lets GM know</param>
    public Level(int number, int maxFlies)
    {
        this.number = number;
        this.maxFlies = maxFlies;
    }

    //-----Public Accessors-----//
    public int Number
    {
        get { return number; }
    }
    public int MaxFlies
    {
        get { return maxFlies; }
    }

}
public class LevelData : ScriptableObject
{
    private List<Level> levels;

    //-----Public Accessors-----//
    public List<Level> Levels
    {
        get
        {
            return levels;
        }
    }

    //-----Methods-----//
    /// <summary>
    /// Creates levels with data and returns first level
    /// </summary>
    /// <returns>First level</returns>
    public Level Init()
    {
        levels = new List<Level>();
        CreateLevel(1, 3);
        CreateLevel(2, 2);
        CreateLevel(3, 2);
        Debug.Log("Init Levels");

        return levels[0];
    }

    /// <summary>
    /// Creates level and adds it to level list
    /// </summary>
    /// <param name="number">Level number</param>
    /// <param name="maxFlies">Number of flies in level</param>
    private void CreateLevel(int number, int maxFlies)
    {
        Level level = new Level(number, maxFlies);
        levels.Add(level);
    }
}
