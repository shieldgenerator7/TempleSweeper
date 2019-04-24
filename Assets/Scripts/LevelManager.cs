﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{//2018-01-02: copied from WolfSim.LevelManager

    public GameObject linePrefab;
    public int tileHeight = 16;//how many tiles across
    public int tileWidth = 30;//how many tiles from top to bottom
    public List<LevelGenerator> levelGenerators;
    public List<LevelGenerator> postTapLevelGenerators;
    public int mapCount = 10;
    public int mapLineDistMin = 3;
    public int mapLineDistMax = 5;
    public GameObject[,] tileMap;//the map of tiles
    private List<Vector2> mapPath;
    private int mapLineSegmentRevealedCount = 0;
    private List<GameObject> drawnLines = new List<GameObject>();

    public PlayerCharacter playerCharacter;
    public GameObject frame;
    public GameObject startSpot;
    public GameObject theSpot;

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
        generateLevel(tileWidth, tileHeight);
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
        instance.generateLevel(instance.tileWidth, instance.tileHeight);
        instance.anyRevealed = false;
        instance.playerCharacter.reset();

        startSpot.SetActive(false);
        theSpot.SetActive(false);
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

    private static Vector2 getWorldPos(Vector2 iv)
    {
        return getWorldPos((int)iv.x, (int)iv.y);
    }
    private static Vector2 getWorldPos(int ix, int iy)
    {
        Vector2 pos = Vector2.zero;
        pos.x = ix - instance.tileWidth / 2;
        pos.y = iy - instance.tileHeight / 2;
        return pos;
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
        GameObject[,] tiles = new GameObject[width, height];
        foreach (LevelGenerator lgen in levelGenerators)
        {
            lgen.generate(tiles);
        }
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
    /// avoiding the "position to avoid"
    /// </summary>
    /// <param name="posToAvoid"></param>
    private void generateLevelPostTap(Vector2 posToAvoid)
    {
        int itaX = getXIndex(posToAvoid);
        int itaY = getYIndex(posToAvoid);
        foreach (LevelGenerator lgen in postTapLevelGenerators)
        {
            lgen.generatePostTap(tileMap, itaX, itaY);
        }
        generateMapPath(itaX, itaY);
        startSpot.SetActive(true);
        startSpot.transform.position = getWorldPos(itaX, itaY);
    }



    private void generateMapPath(int beginX, int beginY)
    {
        mapLineSegmentRevealedCount = 0;
        //Clear old path objects
        foreach (GameObject go in drawnLines)
        {
            Destroy(go);
        }
        drawnLines.Clear();
        drawnLines = new List<GameObject>();
        int curX = beginX;
        int curY = beginY;
        int prevX = curX - 1;//set to one less to avoid infinite loop
        int prevY = curY - 1;
        //Make new path
        while (true)
        {
            curX = beginX;
            curY = beginY;
            mapPath = new List<Vector2>();
            mapPath.Add(new Vector2(curX, curY));
            for (int i = 0; i < mapCount; i++)
            {
                int newX = curX;
                int newY = curY;
                while (true)
                {
                    newX = curX;
                    newY = curY;
                    int randBool = Random.Range(0, 2);
                    int randDist = Random.Range(mapLineDistMin, mapLineDistMax + 1);
                    randDist *= (Random.Range(0, 2) == 1) ? 1 : -1;
                    if (randBool == 1)
                    {
                        newX += randDist;
                    }
                    else
                    {
                        newY += randDist;
                    }
                    //Check to see if this new point is valid
                    if (
                        (newX != prevX && newY != prevY)
                        && (newX != curX || newY != curY)
                        && inBounds(newX, newY)
                    )
                    {
                        bool hallClear = true;
                        //Check to make sure the new point is
                        //not in the same column or row as another point of the line
                        for (int j = 0; j < mapPath.Count; j++)
                        {
                            Vector2 point = mapPath[i];
                            if (point.x == curX && point.y == curY)
                            {
                                //Don't test against the current position
                                continue;
                            }
                            //Check to make sure the row and column is clear
                            if (point.x == newX || point.y == newY)
                            {
                                hallClear = false;
                                break;
                            }
                        }
                        if (hallClear)
                        {
                            //Break out of the while loop
                            break;
                        }
                    }
                }
                prevX = curX;
                prevY = curY;
                curX = newX;
                curY = newY;
                mapPath.Add(new Vector2(curX, curY));
            }
            //Test to see if it's acceptable
            Vector2 theSpot = mapPath[mapPath.Count - 1];
            LevelTile spotTile = tileMap[(int)theSpot.x, (int)theSpot.y]?.GetComponent<LevelTile>();
            //If the spot is on land
            if (spotTile != null
                //And the spot is not a treasure, mine, or map fragment
                && spotTile.tileType == LevelTile.TileType.EMPTY)
            {
                bool noOverlap = true;
                //Check to make sure the spot is not on another point of the line
                for (int i = 0; i < mapPath.Count - 1; i++)
                {
                    Vector2 point = mapPath[i];
                    if (point == theSpot)
                    {
                        noOverlap = false;
                        break;
                    }
                }
                if (noOverlap)
                {
                    //Break out of the while loop
                    break;
                }
                else
                {
                    //Continue through to the next random iteration
                    continue;
                }
            }
            else
            {
                continue;
            }
        }
    }
    public static void drawNextMapSegment()
    {
        GameObject line = Instantiate(instance.linePrefab);
        Vector2 startPos = getWorldPos(instance.mapPath[instance.mapLineSegmentRevealedCount]);
        Vector2 endPos = getWorldPos(instance.mapPath[instance.mapLineSegmentRevealedCount + 1]);
        line.transform.position = startPos;
        line.transform.right = (endPos - startPos);
        line.GetComponent<SpriteRenderer>().size = new Vector2((startPos - endPos).magnitude, 1);
        instance.drawnLines.Add(line);
        instance.mapLineSegmentRevealedCount++;
        if (instance.mapLineSegmentRevealedCount == instance.mapCount)
        {
            instance.theSpot.SetActive(true);
            instance.theSpot.transform.position = endPos;
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
            if (foundLT.tileType != LevelTile.TileType.MAP)
            {
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
                generateLevelPostTap(tapPos);
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
            if (lt.tileType == LevelTile.TileType.MAP)
            {
                //if it's already been revealed
                //but not activated yet
                if (lt.Revealed && !lt.Activated)
                {
                    lt.Activated = true;
                }
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
        if (!lt)
        {
            //don't process empty spaces
            return;
        }
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
                if (!levelTile.Empty)
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
            LevelTile lt = go?.GetComponent<LevelTile>();
            if (lt && !lt.Revealed)
            {
                if (lt.tileType == LevelTile.TileType.TREASURE
                    || lt.tileType == LevelTile.TileType.TRAP
                    || lt.tileType == LevelTile.TileType.MAP)
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
                        if (tile != null)
                        {
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
