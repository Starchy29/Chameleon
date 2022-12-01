using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Achievements for each level
/// Attaches to star system
/// </summary>
public struct Achievements
{
    float timestamp;
    int flies;
    int deaths;

    public float TimestampToBeat
    {
        get { return timestamp; }
    }
    public float FliesToEat
    {
        get { return flies; }
    }
    public float MaxDeaths
    {
        get { return deaths; }
    }

    public Achievements(float timeReq, int flyReq, int deathReq)
    {
        timestamp = timeReq;
        flies = flyReq;
        deaths = deathReq;
    }
}

/// <summary>
/// Manages level data
/// </summary>
public class LevelManager : ScriptableObject
{
    private List<Level> levels;
    private int levelCount = 0;
    Dictionary<Level, PlayerData> levelStats;

    public List<Level> Levels
    {
        get
        {
            return levels;
        }
    }

    /// <summary>
    /// Level data constructor
    /// </summary>
    public LevelManager(bool isTutorial)
    {
        if (isTutorial)
        {
            InitDict(InitTutorial());
        }
        else
        {
            InitDict(InitLevels());
        }
    }

    /// <summary>
    /// Creates level with achievements and adds it to level list
    /// </summary>
    /// <param name="maxFlies">Max flies in level</param>
    /// <param name="achievements">Level achievements</param>
    private void CreateLevel(int maxFlies, Achievements achievements)
    {
        Level level = new Level(++levelCount, maxFlies, achievements);
        levels.Add(level);
    }

    /// <summary>
    /// Creates levels with data and returns first level
    /// </summary>
    /// <returns>Level count</returns>
    public int InitLevels()
    {
        levels = new List<Level>();
        CreateLevel(3, new Achievements(20, 4, 2));  // Level 1 // Best - 7.57s
        CreateLevel(3, new Achievements(15, 3, 2));  // Level 2 // Best - 10.12s
        CreateLevel(2, new Achievements(14, 2, 2));  // Level 3 // Best - 7.91s
        CreateLevel(4, new Achievements(15, 4, 0));  // Level 4 // Best - 19.71s
        CreateLevel(6, new Achievements(20, 8, 2));  // Level 5 // Best - 15.96s
        CreateLevel(5, new Achievements(20, 6, 1));  // Level 6 // Best - 16.79s
        CreateLevel(6, new Achievements(26, 8, 2));  // Level 7 // Best - 21.80s
        CreateLevel(7, new Achievements(50, 8, 2));  // Level 8 // Best - 49.64s
        CreateLevel(8, new Achievements(30, 11, 2)); // Level 9 // Best - 24.78s
        CreateLevel(10, new Achievements(40, 13, 0)); // Level 10 // Best - 38.11s
        //Debug.Log("Init Levels");

        return levels.Count;
    }

    /// <summary>
    /// Creates tutorial level
    /// </summary>
    /// <returns>Level count</returns>
    private int InitTutorial()
    {
        levels = new List<Level>();
        CreateLevel(2, new Achievements(30, 1, 5));  // Level Ts
        return levels.Count;
    }

    /// <summary>
    /// Initializes and fills dictionary
    /// Could probably have added it in createLevel, but this is fine
    /// </summary>
    /// <param name="levelCount">Number of levels</param>
    public void InitDict(int levelCount)
    {
        levelStats = new Dictionary<Level, PlayerData>(levelCount);
        for (int i = 0; i < levelCount; i++)
        {
            levelStats[levels[i]] = new PlayerData(0f);
        }
    }

    public PlayerData GetLevelPlayerData(Level level)
    {
        return levelStats[level];
    }
    /// <summary>
    /// Updates player data for a level
    /// Checks if it is better too
    /// </summary>
    /// <param name="level">Level that you want its data updated</param>
    /// <param name="playerData">New player data</param>
    public PlayerData UpdateLevelData(Level level, PlayerData playerData)
    {
        // if better than previous data, update;
        PlayerData previousData = levelStats[level];
        if (previousData.Timestamp != 0)
        {
            // Keep the run that has more stars
            //if(playerData.Stars > previousData.Stars)
            //{
            //    levelStats[level] = playerData;
            //}
            // Or replace data 1x1
            if (playerData.Timestamp < previousData.Timestamp) { previousData.Timestamp = playerData.Timestamp; }
            if (playerData.FliesEaten > previousData.FliesEaten) { previousData.FliesEaten = playerData.FliesEaten; }
            if (playerData.Deaths < previousData.Deaths) { previousData.Deaths = playerData.Deaths; }
            levelStats[level] = previousData;
        }
        else
        {
            levelStats[level] = playerData;
        }

        CheckStars(level);
        //playerData.Print();
        return levelStats[level];
    }
    /// <summary>
    /// Checks how many stars the player has on the level
    /// </summary>
    /// <param name="level">Level to check stars</param>
    /// <returns>Number of stars for the specified level</returns>
    private void CheckStars(Level level)
    {
        // Check each variable against its baseline
        PlayerData playerData = levelStats[level];
        // Check if level is already 3-stared
        if(playerData.Stars == 3)
        {
            Debug.Log("Already has 3 stars");
        }

        // Update star status
        Achievements ach = level.Achievements;
        if (playerData.Timestamp < ach.TimestampToBeat) { playerData.AddTimeStar(); }
        if (playerData.FliesEaten >= ach.FliesToEat) { playerData.AddFlyStar(); }
        if (playerData.Deaths < ach.MaxDeaths) { playerData.AddDeathStar(); }

        Debug.Log("Level " + level.Number +" has " + playerData.Stars  + " stars");
        levelStats[level] = playerData;
    }
    /// <summary>
    /// Prints player data from each level
    /// </summary>
    public void Print()
    {
        for (int i = 0; i < levels.Count; i++)
        {
            Level level = levels[i];
            PlayerData playerData = levelStats[level];
            playerData.Print(level.Number);
        }
    }
}

/// <summary>
/// Level - probably could be struct
/// </summary>
public class Level
{
    private Achievements achievements;
    private int number;
    private int maxFlies;

    public int Number
    {
        get { return number; }
    }
    public int MaxFlies
    {
        get { return maxFlies; }
    }
    public Achievements Achievements
    {
        get { return achievements; }
        set
        {
            achievements = value;
            // Update dict
            //levelStats[this] = 
        }
    }

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
    public Level(int number, int maxFlies, float timeReq = 0, int flyReq = 0, int deathReq = 5)
    {
        this.number = number;
        this.maxFlies = maxFlies;
        achievements = new Achievements(timeReq, flyReq, deathReq);
    }
    /// <summary>
    /// Level with achievements
    /// </summary>
    /// <param name="number">Level number</param>
    /// <param name="maxFlies">Max flies in level</param>
    /// <param name="levelAchievements">Level Achievements</param>
    public Level(int number, int maxFlies, Achievements levelAchievements)
    {
        this.number = number;
        this.maxFlies = maxFlies;
        achievements = levelAchievements;
    }
    /// <summary>
    /// Sets a levels achievements
    /// </summary>
    /// <param name="timeReq">Time for the player to beat</param>
    /// <param name="flyReq">Number of flies the player needs to get</param>
    /// <param name="deathReq">Number of deaths the player needs to complete level by</param>
    public void SetAchievements(float timeReq, int flyReq, int deathReq)
    {
        achievements = new Achievements(timeReq, flyReq, deathReq);
    }
}

/// <summary>
/// Player data for each level.
/// Changed from reference to value type in order to update score
/// </summary>
public struct PlayerData
{
    // current level stats
    float timestamp;
    int flies;
    int deaths;
    int timeStar;
    int flyStar;
    int deathStar;

    public float Timestamp
    {
        get { return timestamp; }
        set { timestamp = value; }
    }
    public int FliesEaten
    {
        get { return flies; }
        set { flies = value; }
    }
    public int Deaths
    {
        get { return deaths; }
        set { deaths = value; }
    }
    public int Stars
    {
        get { return timeStar + flyStar + deathStar; }
    }

    public PlayerData(float timestamp)
    {
        this.timestamp = timestamp;
        deaths = 0;
        flies = 0;
        timeStar = 0;
        flyStar = 0;
        deathStar = 0;
    }

    public bool GotTimeStar()
    {
        if (timeStar == 1) { return true; }
        return false;
    }
    public bool GotFlyStar()
    {
        if (flyStar == 1) { return true; }
        return false;
    }
    public bool GotDeathStar()
    {
        if (deathStar == 1) { return true; }
        return false;
    }

    public void AddTimeStar()
    {
        //Debug.Log("Got time star");
        timeStar=1;
    }
    public void AddFlyStar()
    {
        //Debug.Log("Got fly star");
        flyStar = 1;
    }
    public void AddDeathStar()
    {
        //Debug.Log("Got death star");
        deathStar = 1;
    }
    public void Die()
    {
        deaths++;
        ResetFlies();
    }
    public void ResetFlies()
    {
        flies = 0;
    }
    public void ResetDeaths()
    {
        deaths = 0;
    }
    public int EatFly()
    {
        flies++;
        return flies;
    }
    public void Print()
    {
        Debug.Log("\nTime: " + timestamp+" | Flies Eaten: "+flies+" | Deaths: "+deaths);
    }
    public void Print(int number)
    {
        Debug.Log("Level "+number+"\nTime: " + timestamp + " | Flies Eaten: " + flies + " | Deaths: " + deaths + " | Stars: " + Stars);
    }
}
