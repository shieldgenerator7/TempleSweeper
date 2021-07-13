using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCharacter : MonoBehaviour
{
    public delegate void OnStatChanged(int stat);

    private int health = 3;
    public int Health
    {
        get { return health; }
        set
        {
            //Set the new health value
            health = Mathf.Clamp(value, 0, startHealth);
            onHealthChanged?.Invoke(health);
        }
    }
    public int startHealth = 3;
    public event OnStatChanged onHealthChanged;

    private int trophiesFound = 0;
    public int TrophiesFound
    {
        get { return trophiesFound; }
        set
        {
            trophiesFound = Mathf.Clamp(value, 0, goalTrophyCount);
            onTrophiesFoundChanged?.Invoke(trophiesFound);
        }
    }
    public int goalTrophyCount = 10;
    public event OnStatChanged onTrophiesFoundChanged;

    public int goalMapCount = 10;
    private int mapFoundCount = 0;
    public int MapFoundCount
    {
        get { return mapFoundCount; }
        set
        {
            mapFoundCount = Mathf.Clamp(value, 0, goalMapCount);
            onMapFoundCountChanged?.Invoke(mapFoundCount);
        }
    }
    public event OnStatChanged onMapFoundCountChanged;

    private void Start()
    {
        reset();
    }

    /// <summary>
    /// Call this when the player reveals a mine
    /// </summary>
    /// <returns>True if he's still alive, false if game over</returns>
    public bool takeHit()
    {
        Health--;
        return Alive;
    }
    public bool Alive
    {
        get
        {
            return Health > 0;
        }
        set
        {
            if (value)
            {
                Health = startHealth;
            }
            else
            {
                Health = 0;
            }
        }
    }
    public bool findTrophy()
    {
        TrophiesFound++;
        return GoalAchieved;
    }
    public bool GoalAchieved
    {
        get
        {
            return trophiesFound >= goalTrophyCount;
        }
        set
        {
            if (value)
            {
                trophiesFound = goalTrophyCount;
            }
            else
            {
                trophiesFound = 0;
            }
        }
    }
    public bool completedMap()
    {
        return MapFoundCount >= goalMapCount;
    }
    public void reset()
    {
        Health = startHealth;
        TrophiesFound = 0;
        MapFoundCount = 0;
    }
}
