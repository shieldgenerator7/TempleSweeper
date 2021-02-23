using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelManager : MonoBehaviour
{//2018-01-02: copied from WolfSim.LevelManager

    [Header("Levels")]
    public List<Level> levels;

    [Header("Objects")]
    public GameObject frame;

    public GameObject levelTilePrefab;

    //
    // Runtime vars
    //
    private LevelTile[,] tileMap;//the map of tiles
    private int currentLevelIndex = 0;
    public int LevelIndex
    {
        get { return currentLevelIndex; }
        set
        {
            if (value < 0)
            {
                throw new System.ArgumentOutOfRangeException("Value must be greater than 0. value: " + value);
            }
            currentLevelIndex = value % levels.Count;
        }
    }
    private Level Level
    {
        get { return levels[currentLevelIndex]; }
        set { currentLevelIndex = levels.IndexOf(value); }
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
        generateLevel(Level);
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
            foreach (LevelTileController ltc in FindObjectsOfType<LevelTileController>())
            {
                Destroy(ltc.gameObject);
            }
        }

        //Clear extra generated sprites
        foreach (LevelGenerator lgen in Level.postRevealLevelGenerators)
        {
            lgen.clearGeneratedObjects();
        }

        //Move to next level
        if (resetToBeginning)
        {
            LevelIndex = 0;
        }
        else
        {
            LevelIndex++;
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
        if (inBounds(xIndex, yIndex))
        {
            return instance.tileMap[xIndex, yIndex];
        }
        else
        {
            return null;//index out of bounds, return null
        }
    }
    public static Vector2 getPosition(LevelTile lt)
    {
        return getWorldPos(lt.x, lt.y);
    }
    private static LevelTileController getTileController(LevelTile lt)
    {
        return FindObjectsOfType<LevelTileController>().First(ltc => ltc.LevelTile == lt);
    }

    public List<LevelTile> getAllTiles(System.Predicate<LevelTile> condition)
    {
        List<LevelTile> tiles = new List<LevelTile>();
        for (int i = 0; i <= tileMap.GetLength(0); i++)
        {
            for (int j = 0; j <= tileMap.GetLength(0); j++)
            {
                if (condition(tileMap[i, j]))
                {
                    tiles.Add(tileMap[i, j]);
                }
            }
        }
        return tiles;
    }

    public LevelTile StartTile
        => getTile(Managers.Start.transform.position);
    public LevelTile XTile
        => getTile(FindObjectOfType<MapLineUpdater>().LastRevealedSpot);

    private static int getXIndex(Vector2 pos)
    {
        return Mathf.RoundToInt(pos.x + instance.Level.gridWidth / 2);
    }

    private static int getYIndex(Vector2 pos)
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
        tileMap = new LevelTile[width, height];
        foreach (LevelGenerator lgen in level.levelGenerators)
        {
            lgen.generate(tileMap);
        }
        for (int xi = 0; xi < width; xi++)
        {
            for (int yi = 0; yi < height; yi++)
            {
                LevelTile lt = tileMap[xi, yi];
                if (tileMap[xi, yi] == null)
                {
                    //skip empty space
                    continue;
                }
                lt.x = xi;
                lt.y = yi;
                GameObject go = GameObject.Instantiate(levelTilePrefab);
                go.transform.position = getWorldPos(xi, yi);
                go.GetComponent<LevelTileController>().LevelTile = tileMap[xi, yi];
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

    private void generatePostItemReveal(LevelTile.Contents content)
    {
        foreach (LevelGenerator lgen in Level.postRevealLevelGenerators)
        {
            lgen.generatePostReveal(tileMap, content);
        }
    }

    public void processTapGesture(Vector2 tapPos)
    {
        if (foundItem && Managers.Player.Alive)
        {
            recalculateNumbers();
            LevelTile foundLT = foundItem.levelTile.LevelTile;
            if (foundLT.Content != LevelTile.Contents.MAP)
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
                    //Check if map has been completed
                    if (Managers.Player.completedMap())
                    {
                        //Go to latest revealed location
                        Managers.Camera.moveTo(
                            FindObjectOfType<MapLineUpdater>().LastRevealedSpot
                            );
                    }
                    else
                    {
                        //Go to start
                        Managers.Camera.moveTo(Managers.Start);
                    }
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
                int itemCount = getDetectedCount(lt);
                if (!lt.Detectable && lt.Content != LevelTile.Contents.MAP &&
                    getAdjacentFlagCount(lt) == itemCount)
                {
                    //Reveal the surrounding non-flagged tiles
                    foreach (LevelTile neighbor in getSurroundingTiles(lt))
                    {
                        if (!neighbor.Flagged && !neighbor.Revealed)
                        {
                            if (neighbor.Content == LevelTile.Contents.TRAP)
                            {
                                Managers.Player.takeHit();
                            }
                            if (neighbor.Content == LevelTile.Contents.TREASURE)
                            {
                                Managers.Player.findTrophy();
                            }
                            revealTile(neighbor);
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
                if (!lt.Detectable && lt.Content != LevelTile.Contents.MAP &&
                    getAdjacentRevealedCount(lt, true) == itemCount)
                {
                    //Flag the surrounding non-revealed tiles
                    foreach (LevelTile neighbor in getSurroundingTiles(lt))
                    {
                        if (!neighbor.Flagged && !neighbor.Revealed)
                        {
                            //Flag it
                            processFlagGesture(getPosition(neighbor));
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
                if ((!lt.Revealed) || getDetectedCount(lt) > 0)
                {
                    Managers.Effect.highlightChange(lt);
                }
                LevelTile.Contents revealedItem = LevelTile.Contents.NONE;
                bool shouldRevealBoard = false;
                bool prevRevealed = lt.Revealed;
                if (lt.Content == LevelTile.Contents.TRAP)
                {
                    revealedItem = LevelTile.Contents.TRAP;
                    if (!Managers.Player.takeHit())
                    {
                        shouldRevealBoard = true;
                    }
                }
                if (lt.Content == LevelTile.Contents.TREASURE)
                {
                    revealedItem = LevelTile.Contents.TREASURE;
                    Managers.Player.findTrophy();
                }
                if (revealedItem == LevelTile.Contents.TRAP || revealedItem == LevelTile.Contents.TREASURE)
                {
                    lt.Revealed = true;
                    Managers.Effect.highlightChange(lt);
                    if (shouldRevealBoard)
                    {
                        revealBoard();
                    }
                    generatePostItemReveal(revealedItem);
                }
                else
                {
                    revealTile(lt);
                }
                if (lt.Content == LevelTile.Contents.MAP)
                {
                    //if it's already been revealed
                    //but not activated yet
                    if (prevRevealed)
                    {
                        Managers.Effect.highlightChange(lt);
                        lt.Content = LevelTile.Contents.NONE;
                        Managers.Player.MapFoundCount++;
                        LevelManager.getTileController(lt).contentsSR.gameObject.AddComponent<ItemDisplayer>();
                        generatePostItemReveal(LevelTile.Contents.MAP);
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
            Managers.Effect.highlightChange(lt);
            //Update flag counters (fc)
            foreach (LevelTile fc in getSurroundingTiles(lt))
            {
                if (fc.Revealed)
                {
                    getTileController(fc).numberDisplayer.displayNumber();
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
            else
            {
                Managers.Effect.highlightChange(lt);
            }
        }
        frame.transform.position = getPosition(lt);
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
            Managers.TileRevealer.revealTilesAround(lt);
        }
    }

    /// <summary>
    /// Reveals the important tiles of the board, namely the treasures
    /// </summary>
    private void revealBoard()
    {
        foreach (LevelTile lt in getAllTiles(alt => !alt.Revealed))
        {
            if (lt && !lt.Revealed)
            {
                if (lt.Content == LevelTile.Contents.TREASURE
                    || lt.Content == LevelTile.Contents.TRAP
                    || lt.Content == LevelTile.Contents.MAP)
                {
                    lt.Revealed = true;
                    Managers.Effect.highlightChange(lt);
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
    public static int getAdjacentCount(LevelTile lt, LevelTile.Contents content, bool notTheContent = false)
    {
        return getSurroundingTiles(lt).Count(slt => (slt.Content == content) != notTheContent);
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
        return getSurroundingTiles(lt).Count(slt => (slt.Flagged == true) != notFlagged);
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
        return getSurroundingTiles(lt).Count(slt => (slt.Revealed == true) != notRevealed);
    }

    /// <summary>
    /// Returns a list of all 8 tiles that surround the given tile. 
    /// Note that tiles on the edge have less than 8 surrounding tiles.
    /// </summary>
    /// <param name="lt"></param>
    /// <returns></returns>
    public static List<LevelTile> getSurroundingTiles(LevelTile lt)
    {
        List<LevelTile> surroundingTiles = new List<LevelTile>();
        if (!lt)
        {
            return surroundingTiles;
        }
        for (int i = lt.x - 1; i <= lt.x + 1; i++)
        {
            for (int j = lt.y - 1; j <= lt.y + 1; j++)
            {
                if (inBounds(i, j))
                {
                    if (i != lt.x || j != lt.y)
                    {
                        LevelTile tile = instance.tileMap[i, j];
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
    /// Returns count of detectable tiles around the given tile
    /// </summary>
    /// <param name="lt"></param>
    /// <returns></returns>
    public static int getDetectedCount(LevelTile lt)
    {
        return getSurroundingTiles(lt).Count(slt => slt.Detectable);
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
}
