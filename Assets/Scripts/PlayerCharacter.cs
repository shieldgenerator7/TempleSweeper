using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour {

    public int health = 3;
    public int startHealth = 3;

    public int trophiesFound = 0;
    public int goalTrophyCount = 10;

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
        health--;
        return alive();
    }
    public bool alive()
    {
        return health > 0;
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
        health = startHealth;
        trophiesFound = 0;
    }
}
