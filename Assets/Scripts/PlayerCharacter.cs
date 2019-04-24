﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCharacter : MonoBehaviour
{

    private int health = 3;
    public int Health
    {
        get { return health; }
        set
        {
            //Set the new health value
            health = Mathf.Clamp(value, 0, startHealth);
            //Update health bar
            healthBar.updateDisplay(health);
        }
    }
    public int startHealth = 3;

    private int trophiesFound = 0;
    public int TrophiesFound
    {
        get { return trophiesFound; }
        set
        {
            trophiesFound = Mathf.Clamp(value, 0, goalTrophyCount);
            treasureBar.updateDisplay(trophiesFound);
        }
    }
    public int goalTrophyCount = 10;

    public int goalMapCount = 10;
    private int mapFoundCount = 0;
    public int MapFoundCount
    {
        get { return mapFoundCount; }
        set
        {
            mapFoundCount = Mathf.Clamp(value, 0, goalMapCount);
            mapBar.updateDisplay(mapFoundCount);
        }
    }

    public DisplayBar healthBar;
    public DisplayBar treasureBar;
    public DisplayBar crateBar;
    public DisplayBar mapBar;

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
        return alive();
    }
    public bool alive()
    {
        return Health > 0;
    }
    public bool findTrophy()
    {
        TrophiesFound++;
        return goalAchieved();
    }
    public bool goalAchieved()
    {
        return trophiesFound >= goalTrophyCount;
    }
    public bool completedMap()
    {
        return MapFoundCount >= goalMapCount;
    }
    public void reset()
    {
        Health = startHealth;
        TrophiesFound = 0;
        crateBar.updateDisplay(goalTrophyCount);
        MapFoundCount = 0;
    }
}
