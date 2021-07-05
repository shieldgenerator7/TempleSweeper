using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileMap
{
    private readonly LevelTile[,] tileMap;//the map of tiles
    private readonly List<LevelTile> tileList = new List<LevelTile>();
    public readonly int width;
    public readonly int height;
    public readonly Vector2Int size;

    public TileMap(int width, int height)
    {
        this.width = width;
        this.height = height;
        this.size = new Vector2Int(width, height);
        tileMap = new LevelTile[this.width, this.height];
    }

    public LevelTile this[int x, int y]
    {
        get
        {
            if (inBounds(x, x))
            {
                return tileMap[x, y];
            }
            else
            {
                return null;
            }
        }
        set
        {
            if (tileMap[x, y])
            {
                tileList.Remove(tileMap[x, y]);
            }
            LevelTile tile = value;
            tileMap[x, y] = tile;
            if (tile)
            {
                tile.x = x;
                tile.y = y;
                tileList.Add(tile);
            }
        }
    }

    public LevelTile this[Vector2Int pos]
    {
        get => this[pos.x, pos.y];
        set => this[pos.x, pos.y] = value;
    }

    public List<LevelTile> getTiles(System.Predicate<LevelTile> condition)
        => tileList.FindAll(tile => tile && condition(tile));

    public bool inBounds(int x, int y)
        => x >= 0 && x < width
        && y >= 0 && y < height;


    /// <summary>
    /// Returns a list of all 8 tiles that surround the given coordinate. 
    /// Note that coordinates on the edge have less than 8 surrounding tiles.
    /// </summary>
    /// <param name="lt"></param>
    /// <returns></returns>
    public List<LevelTile> getSurroundingTiles(int x, int y)
    {
        List<LevelTile> surroundingTiles = new List<LevelTile>();
        for (int i = x - 1; i <= x + 1; i++)
        {
            for (int j = y - 1; j <= y + 1; j++)
            {
                if (inBounds(i, j))
                {
                    if (i != x || j != y)
                    {
                        LevelTile tile = tileMap[i, j];
                        if (tile != null)
                        {
                            surroundingTiles.Add(tile);
                        }
                    }
                }
            }
        }
        return surroundingTiles;
    }

    /// <summary>
    /// Returns the number of tiles of the given type are surrounding the given coordinate,
    /// but not at the coordinate tile itself
    /// </summary>
    /// <param name="lt"></param>
    /// <param name="tileType"></param>
    /// <param name="notTheType">True to get the amount that is NOT the given type</param>
    /// <returns></returns>
    public int getAdjacentCount(int x, int y, LevelTile.Contents content, bool notTheContent = false)
    {
        return getSurroundingTiles(x, y).Count(
            slt => (slt.Content == content) != notTheContent
            );
    }

    /// <summary>
    /// Returns the number of flagged tiles that are surrounding the given coordinate,
    /// but not at the coordinate tile itself
    /// </summary>
    /// <param name="lt"></param>
    /// <param name="notFlagged">True to get the amount that is NOT flagged</param>
    /// <returns></returns>
    public int getAdjacentFlagCount(int x, int y, bool notFlagged = false)
    {
        return getSurroundingTiles(x, y).Count(
            slt => (slt.Flagged == true) != notFlagged
            );
    }

    /// <summary>
    /// Returns the number of revealed tiles that are surrounding the given coordinate,
    /// but not at the coordinate tile itself
    /// </summary>
    /// <param name="lt"></param>
    /// <param name="notRevealed">True to get the amount that is NOT revealed</param>
    /// <returns></returns>
    public int getAdjacentRevealedCount(int x, int y, bool notRevealed = false)
    {
        return getSurroundingTiles(x, y).Count(slt => (slt.Revealed == true) != notRevealed);
    }

    /// <summary>
    /// Returns count of detectable tiles around the given coordinate
    /// </summary>
    /// <param name="lt"></param>
    /// <returns></returns>
    public int getDetectedCount(int x, int y)
        => getSurroundingTiles(x, y).Count(slt => slt.Detectable);

    /// <summary>
    /// Returns true if there is a non-null cell around the given position in the given range
    /// Note: Counts the position itself too
    /// </summary>
    /// <param name="tiles"></param>
    /// <param name="posX"></param>
    /// <param name="posY"></param>
    /// <param name="range"></param>
    public bool containsLand(int x, int y, int range)
    {
        return landCount(x, y, range) > 0;
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
    public int landCount(int x, int y, int range)
    {
        int count = 0;
        for (int i = x - range; i <= x + range; i++)
        {
            for (int j = y - range; j <= y + range; j++)
            {
                if (inBounds(i, j))
                {
                    if (tileMap[i, j] != null && tileMap[i, j].Walkable)
                    {
                        count++;
                    }
                }
            }
        }
        return count;
    }
}
