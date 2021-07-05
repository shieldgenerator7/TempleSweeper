using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallGenerator : ObjectGenerator
{//2019-06-17: copied from ObjectGenerator

    public Vector2 minSpan = new Vector2(5, 1);
    public Vector2 maxSpan = new Vector2(10, 1);

    public override void generate(TileMap tileMap)
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
    public override void generatePostStart(TileMap tileMap, int posX, int posY)
    {
        for (int i = 0; i < amount; i++)
        {
            while (true)//loop until broken out of
            {
                int rx = Random.Range(0, tileMap.width);
                int ry = Random.Range(0, tileMap.height);
                if (outOfAreaToAvoid(tileMap, rx, ry, posX, posY))
                {
                    LevelTile lt = tileMap[rx, ry];
                    if (lt.Available)
                    {
                        //Do the middle tile
                        lt.Content = content;
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
                                if (tileMap.inBounds(px, py) && outOfAreaToAvoid(tileMap, px, py, posX, posY))
                                {
                                    LevelTile ilt = tileMap[px, py];
                                    if (ilt.Available)
                                    {
                                        ilt.Content = content;
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

    public override void generatePostReveal(TileMap tileMap, LevelTile.Contents content)
    {
        throw new System.NotImplementedException();
    }
}
