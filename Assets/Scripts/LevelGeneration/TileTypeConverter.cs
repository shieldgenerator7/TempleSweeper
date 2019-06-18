using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileTypeConverter : LevelGenerator
{//2019-06-17: copied from ObjectGenerator

    public LevelTile.TileType fromTileType;
    public LevelTile.TileType toTileType;

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

    public override void generatePostReveal(GameObject[,] tileMap, LevelTile.TileType tileType)
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
                        LevelTile lt = tile.GetComponent<LevelTile>();
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
                    LevelTile lt = tileMap[rx, ry]?.GetComponent<LevelTile>();
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
