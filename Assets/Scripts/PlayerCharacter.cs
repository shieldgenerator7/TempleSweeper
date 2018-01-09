using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour {

    public int health = 3;
    public int startHealth = 3;

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
    public void reset()
    {
        health = startHealth;
    }
}
