using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileTypeConverter : LevelGenerator
{//2019-06-17: copied from ObjectGenerator

    public LevelTileController.TileType fromTileType;
    public LevelTileController.TileType toTileType;

    public int maxConvert = -1;
    public bool randomConvert = false;

    public override void generate(GameObject[,] tileMap)
    {
        convert(tileMap);
    }

    /// <summary>
    /// Generate the given tileType,
    /// avoiding the "position to avoid" and the tiles within the "radius to avoid"
    /// </summary>
    /// <param name="tileMap">The tilemap to edit</param>
    /// <param name="posX">Index to Avoid X</param>
    /// <param name="posY">Index to Avoid Y</param>
    public override void generatePostStart(GameObject[,] tileMap, int posX, int posY)
    {
        convert(tileMap);
    }

    public override void generatePostReveal(GameObject[,] tileMap, LevelTileController.TileType tileType)
    {
        convert(tileMap);
    }

    private void convert(GameObject[,] tileMap)
    {
        int width = gridWidth(tileMap);
        int height = gridHeight(tileMap);
        if (!randomConvert)
        {
            int converted = 0;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    GameObject tile = tileMap[i, j];
                    if (tile)
                    {
                        LevelTileController lt = tile.GetComponent<LevelTileController>();
                        if (lt.tileType == fromTileType)
                        {
                            lt.tileType = toTileType;
                            converted++;
                            if (maxConvert > 0 && converted >= maxConvert)
                            {
                                return;
                            }
                        }
                    }
                }
            }
        }
        else
        {
            for(int n = 0; n < maxConvert; n++)
            {
                while (true)
                {
                    int rx = Random.Range(0, width);
                    int ry = Random.Range(0, height);
                    LevelTileController lt = tileMap[rx, ry]?.GetComponent<LevelTileController>();
                    if (lt && lt.tileType == fromTileType)
                    {
                        lt.tileType = toTileType;
                        break;
                    }
                }
            }
        }
    }
}
