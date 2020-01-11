using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Maintains a pool of levels to pick from
/// Each level can only be picked once
/// It can pick the levels in order or randomly
/// </summary>
public class LevelGroup : MonoBehaviour
{
    public enum Order
    {
        RANDOM,
        SEQUENTIAL,
        SEQUENTIAL_BACKWARD
    }
    [Header("Settings")]
    public Order order;
    [Header("Levels")]
    public List<Level> levelPool;
    private Level currentLevel;
    public Level Level
    {
        get
        {
            if (currentLevel == null)
            {
                NextLevel();
            }
            return currentLevel;
        }
    }
    public void NextLevel()
    {
        if (levelPool.Count == 0)
        {
            currentLevel = null;
        }
        else
        {
            int nextIndex = 0;
            switch (order)
            {
                case Order.RANDOM:
                    nextIndex = Random.Range(0, levelPool.Count);
                    break;
                case Order.SEQUENTIAL:
                    nextIndex = 0;
                    break;
                case Order.SEQUENTIAL_BACKWARD:
                    nextIndex = levelPool.Count - 1;
                    break;
            }
            currentLevel = levelPool[nextIndex];
            levelPool.RemoveAt(nextIndex);
        }
    }
}
