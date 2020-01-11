using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{//2018-01-02: copied from WolfSim.LevelManager

    public Level testStartLevel = null;//level to jump to when testing

    [Header("Level Groups")]
    public List<LevelGroup> levelGroups;

    [Header("Objects")]
    public GameObject frame;

    //
    // Runtime vars
    //
    private GameObject[,] tileMap;//the map of tiles
    private int currentLevelGroupIndex = 0;
    public int LevelGroupIndex
    {
        get { return currentLevelGroupIndex; }
        set
        {
            if (value < 0)
            {
                throw new System.ArgumentOutOfRangeException("Value must be greater than 0. value: " + value);
            }
            currentLevelGroupIndex = value % levelGroups.Count;
        }
    }
    private LevelGroup LevelGroup
    {
        get { return levelGroups[currentLevelGroupIndex]; }
        set { currentLevelGroupIndex = levelGroups.IndexOf(value); }
    }
    private Level Level
    {
        get { return levelGroups[currentLevelGroupIndex].Level; }
    }

    private bool anyRevealed = false;//true if any tile has been revealed

    private bool usedFirstHoldFrame;

    private ItemDisplayer foundItem;
    public static ItemDisplayer FoundItem
    {
        get { return instance.foundItem; }
        set { instance.foundItem = value; }
    }

    private static LevelManager instance;

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
        if (Application.isEditor && testStartLevel)
        {
            generateLevel(testStartLevel);
        }
        else
        {
            generateLevel(Level);
        }
        updateOrthographicSize();
    }
    public bool checkReset(Vector2 tapPos)
    {
        bool gameOver = false;
        if (Managers.Player.Alive)
        {
            if (tapOnObject(Managers.Start, tapPos))
            {
                if (Managers.Start.activeSelf)
                {
                    gameOver = true;
                    reset();
                }
            }
            else if (tapOnObject(Managers.End, tapPos))
            {
                if (Managers.Player.completedMap())
                {
                    if (getTile(tapPos).Revealed)
                    {
                        gameOver = true;
                        reset(false);
                    }
                }
            }
        }
        else
        {
            gameOver = true;
            reset();
        }
        return gameOver;
    }
    public void reset(bool resetToBeginning = true)
    {
        if (instance.tileMap != null)
        {
            foreach (GameObject go in instance.tileMap)
            {
                Destroy(go);
            }
        }

        //Clear extra generated sprites
        foreach (LevelGenerator lgen in Level.postStartLevelGenerators)
        {
            lgen.clearGeneratedObjects();
        }
        foreach (LevelGenerator lgen in Level.postRevealLevelGenerators)
        {
            lgen.clearGeneratedObjects();
        }

        //Move to next level
        if (resetToBeginning)
        {
            SceneManager.LoadScene("startScene");
        }
        else
        {
            LevelGroup.NextLevel();
            //If this level group has a next level,
            if (Level)
            {
                //we're good, do nothing
            }
            //Else if this level group is now empty,
            else
            {
                //Ask the next level group for a level
                LevelGroupIndex++;
                LevelGroup.NextLevel();
            }
        }
        generateLevel(Level);

        //Reset runtime variables
        instance.anyRevealed = false;
        Managers.Player.reset();
    }

    public static LevelTile getTile(Vector2 pos)
    {
        int xIndex = getXIndex(pos);
        int yIndex = getYIndex(pos);
        return getTile(xIndex, yIndex);
    }
    public static LevelTile getTile(int xIndex, int yIndex)
    {
        if (inBounds(xIndex, yIndex))
        {
            return instance.tileMap[xIndex, yIndex]?.GetComponent<LevelTile>();
        }
        else
        {
            return null;//index out of bounds, return null
        }
    }

    public static int getXIndex(Vector2 pos)
    {
        return Mathf.RoundToInt(pos.x + instance.Level.gridWidth / 2);
    }

    public static int getYIndex(Vector2 pos)
    {
        return Mathf.RoundToInt(pos.y + instance.Level.gridHeight / 2);
    }

    public static Vector2 getGridPos(Vector2 worldPos)
    {
        return new Vector2(getXIndex(worldPos), getYIndex(worldPos));
    }

    public static Vector2 getWorldPos(Vector2 iv)
    {
        return getWorldPos((int)iv.x, (int)iv.y);
    }
    public static Vector2 getWorldPos(int ix, int iy)
    {
        Vector2 pos = Vector2.zero;
        pos.x = ix - instance.Level.gridWidth / 2;
        pos.y = iy - instance.Level.gridHeight / 2;
        return pos;
    }

    public static bool tapOnObject(GameObject go, Vector2 tapPos)
    {
        return getGridPos(go.transform.position) == getGridPos(tapPos);
    }

    public static int getDisplaySortingOrder(Vector2 pos)
    {
        return (int)((instance.Level.gridHeight / 2 - pos.y) * 100);
    }
    public static Vector2 randomPosition()
    {
        return new Vector2(
            Random.Range(-instance.Level.gridWidth / 2, instance.Level.gridWidth / 2) * 0.9f,
            Random.Range(-instance.Level.gridHeight / 2, instance.Level.gridHeight / 2) * 0.9f
            );
    }
    public static bool inBounds(Vector2 pos)
    {
        return pos.x > -instance.Level.gridWidth / 2 * 0.99f
            && pos.x < instance.Level.gridWidth / 2 * 0.99f
            && pos.y > -instance.Level.gridHeight / 2 * 0.99f
            && pos.y < instance.Level.gridHeight / 2 * 0.99f;
    }

    public static bool inBounds(int ix, int iy)
    {
        return ix >= 0 && ix < instance.tileMap.GetLength(0)
            && iy >= 0 && iy < instance.tileMap.GetLength(1);
    }

    private void generateLevel(Level level)
    {
        int width = level.gridWidth;
        int height = level.gridHeight;
        GameObject[,] tiles = new GameObject[width, height];
        foreach (LevelGenerator lgen in level.levelGenerators)
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
        foreach (LevelGenerator lgen in Level.postStartLevelGenerators)
        {
            lgen.generatePostStart(tileMap, itaX, itaY);
        }
    }

    private void generatePostItemReveal(LevelTile.TileType tileType)
    {
        foreach (LevelGenerator lgen in Level.postRevealLevelGenerators)
        {
            lgen.generatePostReveal(tileMap, tileType);
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
        if (foundItem && Managers.Player.Alive)
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
                //Check if goals have been achieved
                if (Managers.Player.GoalAchieved)
                {
                    //Go to start
                    Managers.Camera.moveTo(Managers.Start);
                }
            }
            else
            {
                //Check if map has been completed
                if (Managers.Player.completedMap())
                {
                    //Go to latest revealed location
                    Managers.Camera.moveTo(
                        FindObjectOfType<MapLineUpdater>().LastRevealedSpot
                        );
                }
            }
            foundItem.retire();
            foundItem = null;
            return;
        }
        if (checkReset(tapPos))
        {
            return;
        }
        LevelTile lt = getTile(tapPos);
        if (lt != null)
        {
            //If it's revealed
            if (lt.Revealed)
            {
                //Auto-Reveal
                //If the count of surrounding flags equals
                //the count of surrounding trap tiles,
                int itemCount = getAdjacentCount(lt, LevelTile.TileType.TRAP);
                itemCount += getAdjacentCount(lt, LevelTile.TileType.TREASURE);
                if (lt.Empty && lt.tileType != LevelTile.TileType.MAP &&
                    getAdjacentFlagCount(lt) == itemCount)
                {
                    //Reveal the surrounding non-flagged tiles
                    foreach (LevelTile neighbor in getSurroundingTiles(lt))
                    {
                        if (!neighbor.Flagged && !neighbor.Revealed)
                        {
                            if (neighbor.tileType == LevelTile.TileType.TRAP)
                            {
                                Managers.Player.takeHit();
                            }
                            if (neighbor.tileType == LevelTile.TileType.TREASURE)
                            {
                                Managers.Player.findTrophy();
                            }
                            revealTile(neighbor);
                            Managers.Time.moveForward();
                        }
                    }
                    if (!Managers.Player.Alive)
                    {
                        revealBoard();
                    }
                }
                //Auto-Flag
                //If the count of surrounding unrevealed tiles equals
                //the count of surrounding trap tiles,
                if (lt.Empty && lt.tileType != LevelTile.TileType.MAP &&
                    getAdjacentRevealedCount(lt, true) == itemCount)
                {
                    //Flag the surrounding non-revealed tiles
                    foreach (LevelTile neighbor in getSurroundingTiles(lt))
                    {
                        if (!neighbor.Flagged && !neighbor.Revealed)
                        {
                            //Flag it
                            processFlagGesture(neighbor.transform.position);
                        }
                    }
                }
            }
            //If it's not flagged
            if (!lt.Flagged)
            {
                if (!anyRevealed)
                {
                    generateLevelPostTap(tapPos);
                    anyRevealed = true;
                }
                LevelTile.TileType revealedItem = LevelTile.TileType.EMPTY;
                bool shouldRevealBoard = false;
                bool prevRevealed = lt.Revealed;
                if (lt.tileType == LevelTile.TileType.TRAP)
                {
                    revealedItem = LevelTile.TileType.TRAP;
                    if (!Managers.Player.takeHit())
                    {
                        shouldRevealBoard = true;
                    }
                }
                if (lt.tileType == LevelTile.TileType.TREASURE)
                {
                    revealedItem = LevelTile.TileType.TREASURE;
                    Managers.Player.findTrophy();
                }
                if (!LevelTile.empty(revealedItem))
                {
                    lt.Revealed = true;
                    if (shouldRevealBoard)
                    {
                        revealBoard();
                    }
                    else
                    {
                        Managers.Time.moveForward();
                    }
                    generatePostItemReveal(revealedItem);
                }
                else if (!lt.Revealed)
                {
                    revealTile(lt);
                    Managers.Time.moveForward();
                }
                if (lt.tileType == LevelTile.TileType.MAP)
                {
                    //if it's already been revealed
                    //but not activated yet
                    if (prevRevealed && !lt.Activated)
                    {
                        lt.Activated = true;
                        generatePostItemReveal(LevelTile.TileType.MAP);
                    }
                }
            }
        }
    }
    public void processFlagGesture(Vector2 flagPos)
    {
        if (checkReset(flagPos))
        {
            return;
        }
        LevelTile lt = getTile(flagPos);
        if (lt != null && !lt.Revealed)
        {
            lt.Flagged = !lt.Flagged;
            //Update flag counters (fc)
            foreach (LevelTile fc in getSurroundingTiles(lt))
            {
                if (fc.Revealed)
                {
                    fc.numberDisplayer.displayNumber();
                }
            }
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
    /// Returns the number of flagged tiles that are surrounding the given tile,
    /// not including the tile itself
    /// </summary>
    /// <param name="lt"></param>
    /// <param name="notFlagged">True to get the amount that is NOT flagged</param>
    /// <returns></returns>
    public static int getAdjacentFlagCount(LevelTile lt, bool notFlagged = false)
    {
        int count = 0;
        foreach (LevelTile levelTile in getSurroundingTiles(lt))
        {
            if ((levelTile.Flagged == true)
                != notFlagged)
            {
                count++;
            }
        }
        return count;
    }

    /// <summary>
    /// Returns the number of revealed tiles that are surrounding the given tile,
    /// not including the tile itself
    /// </summary>
    /// <param name="lt"></param>
    /// <param name="notRevealed">True to get the amount that is NOT revealed</param>
    /// <returns></returns>
    public static int getAdjacentRevealedCount(LevelTile lt, bool notRevealed = false)
    {
        int count = 0;
        foreach (LevelTile levelTile in getSurroundingTiles(lt))
        {
            if ((levelTile.Revealed == true)
                != notRevealed)
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
    public static List<LevelTile> getSurroundingTiles(LevelTile lt, int range = 1)
    {
        List<LevelTile> surroundingTiles = new List<LevelTile>();
        for (int i = lt.indexX - range; i <= lt.indexX + range; i++)
        {
            for (int j = lt.indexY - range; j <= lt.indexY + range; j++)
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

    public static List<LevelTile> getSurroundingTilesManhattan(LevelTile lt, int range = 1)
    {
        List<LevelTile> surroundingTiles = new List<LevelTile>();
        for (int i = lt.indexX - range; i <= lt.indexX + range; i++)
        {
            for (int j = lt.indexY - range; j <= lt.indexY + range; j++)
            {
                if (inBounds(i, j))
                {
                    if (i != lt.indexX || j != lt.indexY)
                    {
                        if (Mathf.Abs(i - lt.indexX) + Mathf.Abs(j - lt.indexY) <= range)
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
        }
        return surroundingTiles;
    }

    public static void hideSurroundingTiles(LevelTile center, int obscureRange = 1)
    {
        List<LevelTile> tilesToHide = new List<LevelTile>();
        if (obscureRange > 0)
        {
            tilesToHide.AddRange(getSurroundingTiles(center, obscureRange));
        }
        if (obscureRange >= 0)
        {
            tilesToHide.Add(center);
        }
        foreach (LevelTile lt in tilesToHide)
        {
            lt.Revealed = false;
        }
    }

    public static void updateSurroundingTiles(LevelTile center)
    {
        List<LevelTile> tilesToUpdate = getSurroundingTiles(center);
        tilesToUpdate.Add(center);
        foreach (LevelTile lt in tilesToUpdate)
        {
            if (lt.Revealed)
            {
                lt.numberDisplayer.displayNumber();
            }
        }
    }

    public void updateOrthographicSize()
    {
        while (true)//loop until broken out of
        {
            Vector2 screenSizeWorld = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height))
                - Camera.main.ScreenToWorldPoint(Vector2.zero);
            if (screenSizeWorld.x > Level.gridWidth && screenSizeWorld.y > Level.gridHeight)
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
