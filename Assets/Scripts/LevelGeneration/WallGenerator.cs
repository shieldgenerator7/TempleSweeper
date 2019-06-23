using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallGenerator : ObjectGenerator
{//2019-06-17: copied from ObjectGenerator

    public Vector2 minSpan = new Vector2(5, 1);
    public Vector2 maxSpan = new Vector2(10, 1);

    public override void generate(GameObject[,] tileMap)
    {
        throw new System.NotImplementedException();
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
        for (int i = 0; i < amount; i++)
        {
            while (true)//loop until broken out of
            {
                int rx = Random.Range(0, gridWidth(tileMap));
                int ry = Random.Range(0, gridHeight(tileMap));
                if (outOfAreaToAvoid(tileMap, rx, ry, posX, posY))
                {
                    LevelTile lt = tileMap[rx, ry]?.GetComponent<LevelTile>();
                    if (lt && lt.Available)
                    {
                        //Do the middle tile
                        lt.tileType = tileType;
                        //Do the wings
                        int spanX = (int)Random.Range(minSpan.x, maxSpan.x);
                        int spanY = (int)Random.Range(minSpan.y, maxSpan.y);
                        int startX = (int)Mathf.Round(rx - (spanX / 2));
                        int startY = (int)Mathf.Round(ry - (spanY / 2));
                        for (int ix = 0; ix < spanX; ix++)
                        {
                            for (int iy = 0; iy < spanY; iy++)
                            {
                                int px = ix + startX;
                                int py = iy + startY;
                                if (inBounds(tileMap, px, py) && outOfAreaToAvoid(tileMap, px, py, posX, posY))
                                {
                                    LevelTile ilt = tileMap[px, py]?.GetComponent<LevelTile>();
                                    if (ilt && ilt.Available)
                                    {
                                        ilt.tileType = tileType;
                                    }
                                }
                            }
                        }
                        //break the while loop
                        break;
                    }
                }
            }
        }
    }

    public override void generatePostReveal(GameObject[,] tileMap, LevelTile.TileType tileType)
    {
        throw new System.NotImplementedException();
    }
}
