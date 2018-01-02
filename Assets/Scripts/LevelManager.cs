using System.Collections;
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

    private bool anyRevealed = false;//true if any tile has been revealed

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
        generateLevel(tileWidth, tileHeight);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public static LevelTile getTile(Vector2 pos)
    {
        int xIndex = getXIndex(pos);
        int yIndex = getYIndex(pos);
        if (inBounds(xIndex, yIndex))
        {
            return instance.tileMap[xIndex, yIndex].GetComponent<LevelTile>();
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
        GameObject[,] tiles = new GameObject[width, height];
        generateFill(levelTilePrefab, tiles, width, height);
        tileMap = new GameObject[width, height];
        for (int xi = 0; xi < width; xi++)
        {
            for (int yi = 0; yi < height; yi++)
            {
                GameObject go = GameObject.Instantiate(tiles[xi, yi]);
                go.transform.position = new Vector2(xi - width / 2, yi - height / 2);
                tileMap[xi, yi] = go;
                go.GetComponent<LevelTile>().indexX = xi;
                go.GetComponent<LevelTile>().indexY = yi;
                go.transform.parent = transform;
            }
        }
        //Zoom camera out to fit whole board
        Camera.main.orthographicSize = tileHeight / 2;
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
                    && Mathf.Abs(itaY - iy) > radiusToAvoid)
                {
                    LevelTile lt = tileMap[ix, iy].GetComponent<LevelTile>();
                    if (lt.tileType == LevelTile.TileType.EMPTY)
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

    public void processTapGesture(Vector2 tapPos)
    {
        LevelTile lt = getTile(tapPos);
        if (lt != null)
        {
            if (!anyRevealed)
            {
                generateLevelPostTap(tapPos, 1);
                anyRevealed = true;
            }
            revealTile(lt);
        }
    }

    private void revealTile(LevelTile lt)
    {
        if (!lt.hasRevealed())
        {
            lt.reveal();
            Debug.Log("Revealing recursively: (" + lt.indexX + ", " + lt.indexY + ")");
            //Check to make sure surrounding tiles are empty
            for (int i = lt.indexX - 1; i <= lt.indexX + 1; i++)
            {
                for (int j = lt.indexY - 1; j <= lt.indexY + 1; j++)
                {
                    if (inBounds(i, j))
                    {
                        if (tileMap[i, j].GetComponent<LevelTile>().tileType != LevelTile.TileType.EMPTY)
                        {
                            //break out of the method
                            return;
                        }
                    }
                }
            }
            //Reveal surrounding tiles
            for (int i = lt.indexX - 1; i <= lt.indexX + 1; i++)
            {
                for (int j = lt.indexY - 1; j <= lt.indexY + 1; j++)
                {
                    if (inBounds(i, j))
                    {
                        revealTile(tileMap[i, j].GetComponent<LevelTile>());
                    }
                }
            }
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
    public static int getAdjacentCount(LevelTile lt, LevelTile.TileType tileType, bool notTheType=false)
    {
        int count = 0;
        for (int i = lt.indexX - 1; i <= lt.indexX + 1; i++)
        {
            for (int j = lt.indexY - 1; j <= lt.indexY + 1; j++)
            {
                if (inBounds(i, j))
                {
                    if (i != lt.indexX || j != lt.indexY)
                    {
                        if ((instance.tileMap[i, j].GetComponent<LevelTile>().tileType == tileType)
                            != notTheType)
                        {
                            count++;
                        }
                    }
                }
            }
        }
        return count;
    }

    //Legacy

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
