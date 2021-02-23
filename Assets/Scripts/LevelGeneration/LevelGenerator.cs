﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LevelGenerator : MonoBehaviour
{
    public abstract void generate(LevelTile[,] tileMap);

    public abstract void generatePostStart(LevelTile[,] tileMap, int posX, int posY);

    public abstract void generatePostReveal(LevelTile[,] tileMap, LevelTile.Contents content);

    public virtual void clearGeneratedObjects()
    {
        throw new System.NotImplementedException();
    }



    protected static int gridWidth(LevelTile[,] tileMap)
    {
        return tileMap.GetLength(0);
    }
    protected static int gridHeight(LevelTile[,] tileMap)
    {
        return tileMap.GetLength(1);
    }

    /// <summary>
    /// Returns true if there is a non-null cell around the given position in the given range
    /// Note: Counts the position itself too
    /// </summary>
    /// <param name="tiles"></param>
    /// <param name="posX"></param>
    /// <param name="posY"></param>
    /// <param name="range"></param>
    protected static bool containsLand(LevelTile[,] tiles, int posX, int posY, int range)
    {
        return landCount(tiles, posX, posY, range) > 0;
    }
    /// <summary>
    /// Returns the amount of non-null cells around the given position in the given range
    /// Note: Counts the position itself too
    /// </summary>
    /// <param name="tileMap"></param>
    /// <param name="posX"></param>
    /// <param name="posY"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    protected static int landCount(LevelTile[,] tileMap, int posX, int posY, int range)
    {
        int count = 0;
        for (int x = posX - range; x <= posX + range; x++)
        {
            for (int y = posY - range; y <= posY + range; y++)
            {
                if (inBounds(tileMap, x, y))
                {
                    if (tileMap[x, y] != null && tileMap[x,y].Walkable)
                    {
                        count++;
                    }
                }
            }
        }
        return count;
    }

    /// <summary>
    /// Returns whether the given indices are valid coordinates in the given map
    /// </summary>
    /// <param name="tileMap"></param>
    /// <param name="posX"></param>
    /// <param name="posY"></param>
    /// <returns></returns>
    protected static bool inBounds(LevelTile[,] tileMap, int posX, int posY)
    {
        return posX >= 0 && posX < tileMap.GetLength(0)
            && posY >= 0 && posY < tileMap.GetLength(1);
    }
}
