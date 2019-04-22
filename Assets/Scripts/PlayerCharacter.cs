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

    public int trophiesFound = 0;
    public int goalTrophyCount = 10;

    public DisplayBar healthBar;

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
        trophiesFound++;
        return goalAchieved();
    }
    public bool goalAchieved()
    {
        return trophiesFound >= goalTrophyCount;
    }
    public void reset()
    {
        Health = startHealth;
        trophiesFound = 0;
    }
}
