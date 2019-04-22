﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{//2018-01-02: copied from WolfSim.LevelManager

    public GameObject levelTilePrefab;
    public int tileHeight = 16;//how many tiles across
    public int tileWidth = 30;//how many tiles from top to bottom
    public int mineCount = 89;//how many mines there are
    public int treasureCount = 10;//how many treasures there are
    public GameObject[,] tileMap;//the map of tiles

    public PlayerCharacter playerCharacter;
    public GameObject frame;

    private bool anyRevealed = false;//true if any tile has been revealed

    private static LevelManager instance;
    private bool usedFirstHoldFrame;

    private ItemDisplayer foundItem;
    public static ItemDisplayer FoundItem
    {
        get { return instance.foundItem; }
        set { instance.foundItem = value; }
    }

    // Use this for initialization
    void Start()
    {
        //Singleton sorting out
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
            return;
        }
        //Initialization stuff
        generateLevel(tileWidth / 2, tileHeight / 2);
        updateOrthographicSize();
    }
    public bool checkReset()
    {
        bool gameOver = false;
        gameOver = !playerCharacter.alive() || playerCharacter.goalAchieved();
        if (gameOver)
        {
            reset();
        }
        return gameOver;
    }
    public void reset()
    {
        if (instance.tileMap != null)
        {
            foreach (GameObject go in instance.tileMap)
            {
                Destroy(go);
            }
        }
        instance.generateLevel(instance.tileWidth / 2, instance.tileHeight / 2);
        instance.anyRevealed = false;
        instance.playerCharacter.reset();
    }

    public static LevelTile getTile(Vector2 pos)
    {
        int xIndex = getXIndex(pos);
        int yIndex = getYIndex(pos);
        if (inBounds(xIndex, yIndex))
        {
            return instance.tileMap[xIndex, yIndex]?.GetComponent<LevelTile>();
        }
        else
        {
            return null;//index out of bounds, return null
        }
    }

    private static int getXIndex(Vector2 pos)
    {
        return Mathf.RoundToInt(pos.x + instance.tileWidth / 2);
    }

    private static int getYIndex(Vector2 pos)
    {
        return Mathf.RoundToInt(pos.y + instance.tileHeight / 2);
    }

    public static int getDisplaySortingOrder(Vector2 pos)
    {
        return (int)((instance.tileHeight / 2 - pos.y) * 100);
    }
    public static Vector2 randomPosition()
    {
        return new Vector2(
            Random.Range(-instance.tileWidth / 2, instance.tileWidth / 2) * 0.9f,
            Random.Range(-instance.tileHeight / 2, instance.tileHeight / 2) * 0.9f
            );
    }
    public static bool inBounds(Vector2 pos)
    {
        return pos.x > -instance.tileWidth / 2 * 0.99f
            && pos.x < instance.tileWidth / 2 * 0.99f
            && pos.y > -instance.tileHeight / 2 * 0.99f
            && pos.y < instance.tileHeight / 2 * 0.99f;
    }

    public static bool inBounds(int ix, int iy)
    {
        return ix >= 0 && ix < instance.tileMap.GetLength(0)
            && iy >= 0 && iy < instance.tileMap.GetLength(1);
    }

    private void generateLevel(int width, int height)
    {
        GameObject[,] tiles = generateIsland(levelTilePrefab, width, height);
        width *= 2;
        height *= 2;
        //generateFill(levelTilePrefab, tiles, width, height);
        tileMap = new GameObject[width, height];
        for (int xi = 0; xi < width; xi++)
        {
            for (int yi = 0; yi < height; yi++)
            {
                GameObject prefab = tiles[xi, yi];
                if (prefab == null)
                {
                    //skip empty space
                    continue;
                }
                GameObject go = GameObject.Instantiate(prefab);
                go.transform.position = new Vector2(xi - width / 2, yi - height / 2);
                tileMap[xi, yi] = go;
                go.GetComponent<LevelTile>().indexX = xi;
                go.GetComponent<LevelTile>().indexY = yi;
                go.transform.parent = transform;
            }
        }
    }

    /// <summary>
    /// After the initial reveal, generate the mines and treasures,
    /// avoiding the "position to avoid" and the tiles within the "radius to avoid"
    /// </summary>
    /// <param name="posToAvoid"></param>
    /// <param name="radiusToAvoid"></param>
    private void generateLevelPostTap(Vector2 posToAvoid, int radiusToAvoid)
    {
        int itaX = getXIndex(posToAvoid);
        int itaY = getYIndex(posToAvoid);
        generateObject(itaX, itaY, radiusToAvoid, mineCount, LevelTile.TileType.TRAP);
        generateObject(itaX, itaY, radiusToAvoid, treasureCount, LevelTile.TileType.TREASURE);
    }

    /// <summary>    /// 
    /// Generate the given tileType,
    /// avoiding the "position to avoid" and the tiles within the "radius to avoid"
    /// </summary>
    /// <param name="itaX">Index to Avoid X</param>
    /// <param name="itaY">Index to Avoid Y</param>
    /// <param name="radiusToAvoid"></param>
    /// <param name="amount">How many to generate</param>
    /// <param name="tileType"></param>
    public void generateObject(int itaX, int itaY, int radiusToAvoid, int amount, LevelTile.TileType tileType)
    {
        for (int i = 0; i < amount; i++)
        {
            while (true)//loop until broken out of
            {
                int ix = Random.Range(0, tileWidth);
                int iy = Random.Range(0, tileHeight);
                if (Mathf.Abs(itaX - ix) > radiusToAvoid
                    || Mathf.Abs(itaY - iy) > radiusToAvoid)
                {
                    LevelTile lt = tileMap[ix, iy]?.GetComponent<LevelTile>();
                    if (lt && lt.tileType == LevelTile.TileType.EMPTY)
                    {
                        lt.tileType = tileType;
                        break;//break the while loop
                    }
                }
            }
        }
    }

    private void generateFill(GameObject prefab, GameObject[,] prefabMap, int width, int height)
    {
        for (int xi = 0; xi < width; xi++)
        {
            for (int yi = 0; yi < height; yi++)
            {
                prefabMap[xi, yi] = prefab;
            }
        }
    }

    private GameObject[,] generateIsland(GameObject prefab, int gridWidth, int gridHeight)
    {
        Vector2 min, max;
        min = max = new Vector2(gridWidth, gridHeight);
        int landCount = gridWidth * gridHeight;
        GameObject[,] map = new GameObject[gridWidth * 2, gridHeight * 2];
        //Place the first one
        map[(int)min.x, (int)min.y] = prefab;
        //Place the rest of them
        int maxRange = 2;//how far a placed land can be from the nearest land
        for (int i = 0; i < landCount - 1; i++)
        {
            bool placed = false;
            while (!placed)
            {
                //Randomize new position
                int randX = Random.Range((int)min.x - 1, (int)max.x + 2);
                int randY = Random.Range((int)min.y - 1, (int)max.y + 2);
                //If the position is valid,
                if (inBounds(map, randX, randY))
                {
                    //If the spot is empty,
                    if (map[randX, randY] == null)
                    {
                        //And it's next to another land,                    
                        if (containsLand(map, randX, randY, maxRange))
                        {
                            //Place it here
                            map[randX, randY] = prefab;
                            placed = true;
                            //Update min and max
                            min.x = Mathf.Min(randX, min.x);
                            min.y = Mathf.Min(randY, min.y);
                            max.x = Mathf.Max(randX, max.x);
                            max.y = Mathf.Max(randY, max.y);
                        }
                    }
                }
            }
        }
        return map;
    }
    /// <summary>
    /// Called in generateIslands()
    /// </summary>
    /// <param name="tiles"></param>
    /// <param name="posX"></param>
    /// <param name="posY"></param>
    /// <param name="range"></param>
    private bool containsLand(GameObject[,] tiles, int posX, int posY, int range)
    {
        for (int x = posX - range; x <= posX + range; x++)
        {
            for (int y = posY - range; y <= posY + range; y++)
            {
                if (inBounds(tiles, x, y))
                {
                    if (tiles[x, y] != null)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
    /// <summary>
    /// Called in generateIslands()
    /// </summary>
    /// <param name="tiles"></param>
    /// <param name="posX"></param>
    /// <param name="posY"></param>
    /// <returns></returns>
    private bool inBounds(GameObject[,] tiles, int posX, int posY)
    {
        return posX >= 0 && posX < tiles.GetLength(0)
            && posY >= 0 && posY < tiles.GetLength(1);
    }

    public void processTapGesture(Vector2 tapPos)
    {
        if (checkReset())
        {
            return;
        }
        if (foundItem)
        {
            recalculateNumbers();
            LevelTile foundLT = foundItem.levelTile;
            //Reveal the found LT
            revealTile(foundLT, true);
            //Reveal the tiles around the found LT
            foreach (LevelTile levelTile in getSurroundingTiles(foundLT))
            {
                if (levelTile.Revealed)
                {
                    revealTile(levelTile, true);
                }
            }
            foundItem.retire();
            foundItem = null;
            return;
        }
        LevelTile lt = getTile(tapPos);
        if (lt != null && !lt.Flagged)
        {
            if (!anyRevealed)
            {
                generateLevelPostTap(tapPos, 1);
                anyRevealed = true;
            }
            bool isItem = false;
            bool shouldRevealBoard = false;
            if (lt.tileType == LevelTile.TileType.TRAP)
            {
                isItem = true;
                if (!playerCharacter.takeHit())
                {
                    shouldRevealBoard = true;
                }
            }
            if (lt.tileType == LevelTile.TileType.TREASURE)
            {
                isItem = true;
                if (playerCharacter.findTrophy())
                {
                    shouldRevealBoard = true;
                }
            }
            if (isItem)
            {
                lt.Revealed = true;
                if (shouldRevealBoard)
                {
                    revealBoard();
                }
            }
            else
            {
                revealTile(lt);
            }
        }
    }
    public void processFlagGesture(Vector2 flagPos)
    {
        if (checkReset())
        {
            return;
        }
        LevelTile lt = getTile(flagPos);
        if (lt != null && !lt.Revealed)
        {
            lt.Flagged = !lt.Flagged;
        }
    }
    public void processHoldGesture(Vector2 holdPos, bool finished)
    {
        LevelTile lt = getTile(holdPos);
        if (!usedFirstHoldFrame)
        {
            usedFirstHoldFrame = true;
            processFlagGesture(holdPos);
            Vibration.Vibrate(75);
            if (lt.Revealed)
            {
                frame.SetActive(true);
            }
        }
        frame.transform.position = lt.transform.position;
        if (finished)
        {
            usedFirstHoldFrame = false;
            frame.SetActive(false);
        }
    }

    private void revealTile(LevelTile lt, bool forceReveal = false)
    {
        if ((!lt.Revealed || forceReveal) && !lt.Flagged)
        {
            lt.Revealed = true;
            //Check to make sure surrounding tiles are empty
            foreach (LevelTile levelTile in getSurroundingTiles(lt))
            {
                if (levelTile.tileType != LevelTile.TileType.EMPTY)
                {
                    //break out of the method
                    return;
                }
            }
            //If all surrounding tiles are empty,
            //Reveal surrounding tiles
            //Reveal surrounding tiles
            foreach (LevelTile levelTile in getSurroundingTiles(lt))
            {
                revealTile(levelTile);
            }
        }
    }

    /// <summary>
    /// Reveals the important tiles of the board, namely the treasures
    /// </summary>
    private void revealBoard()
    {
        foreach (GameObject go in tileMap)
        {
            LevelTile lt = go.GetComponent<LevelTile>();
            if (!lt.Revealed)
            {
                if (lt.tileType == LevelTile.TileType.TREASURE
                    || lt.tileType == LevelTile.TileType.TRAP)
                {
                    lt.Revealed = true;
                }
            }
        }
    }

    /// <summary>
    /// Tells all the counter sprites to update after the board changes
    /// </summary>
    void recalculateNumbers()
    {
        foreach (NumberDisplayer nd in FindObjectsOfType<NumberDisplayer>())
        {
            nd.displayNumber();
        }
    }

    /// <summary>
    /// Returns the number of tiles of the given type are surrounding the given tile,
    /// not including the tile itself
    /// </summary>
    /// <param name="lt"></param>
    /// <param name="tileType"></param>
    /// <param name="notTheType">True to get the amount that is NOT the given type</param>
    /// <returns></returns>
    public static int getAdjacentCount(LevelTile lt, LevelTile.TileType tileType, bool notTheType = false)
    {
        int count = 0;
        foreach (LevelTile levelTile in getSurroundingTiles(lt))
        {
            if ((levelTile.tileType == tileType)
                != notTheType)
            {
                count++;
            }
        }
        return count;
    }

    /// <summary>
    /// Returns a list of all 8 tiles that surround the given tile. 
    /// Note that tiles on the edge have less than 8 surrounding tiles.
    /// </summary>
    /// <param name="lt"></param>
    /// <returns></returns>
    static List<LevelTile> getSurroundingTiles(LevelTile lt)
    {
        List<LevelTile> surroundingTiles = new List<LevelTile>();
        for (int i = lt.indexX - 1; i <= lt.indexX + 1; i++)
        {
            for (int j = lt.indexY - 1; j <= lt.indexY + 1; j++)
            {
                if (inBounds(i, j))
                {
                    if (i != lt.indexX || j != lt.indexY)
                    {
                        GameObject tile = instance.tileMap[i, j];
                        if (tile != null) {
                            surroundingTiles.Add(tile.GetComponent<LevelTile>());
                        }
                    }
                }
            }
        }
        return surroundingTiles;
    }

    public void updateOrthographicSize()
    {
        while (true)//loop until broken out of
        {
            Vector2 screenSizeWorld = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height))
                - Camera.main.ScreenToWorldPoint(Vector2.zero);
            if (screenSizeWorld.x > tileWidth && screenSizeWorld.y > tileHeight)
            {
                break;//all good hear
            }
            Camera.main.orthographicSize++;
        }
    }

    //Score

    //Legacy generation methods
    private void generateRiver(GameObject prefab, GameObject[,] prefabMap, int width, int height, int startY)
    {
        int currentY = startY;
        int prevY = currentY;
        for (int xi = 0; xi < width; xi++)
        {
            if (Random.Range(0, 2) > 0)
            {
                currentY += Random.Range(-2, 2);
            }
            for (int yi = prevY; yi != currentY; yi += (int)Mathf.Sign(currentY - prevY))
            {
                if (yi >= 0 && yi < height)
                {
                    prefabMap[xi, yi] = prefab;
                }
            }
            if (currentY >= 0 && currentY < height)
            {
                prefabMap[xi, currentY] = prefab;
            }
            prevY = currentY;
        }
    }

    private void generateForest(GameObject prefab, Vector2 size, Vector2 pos)
    {
        for (int count = 0; count < size.x * size.y / 3; count++)
        {
            float randomX = Random.Range(-size.x / 2, size.x / 2) + pos.x;
            float randomY = Random.Range(-size.y / 2, size.y / 2) + pos.y;
            Vector2 randomPos = new Vector2(randomX, randomY);
            if (!getTile(randomPos).gameObject.name.Contains("water"))
            {
                GameObject go = GameObject.Instantiate(prefab);
                go.transform.position = randomPos;
                go.GetComponent<SpriteRenderer>().sortingOrder = getDisplaySortingOrder(randomPos);
            }
        }
    }
}
