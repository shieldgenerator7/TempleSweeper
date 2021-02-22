using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClusterGenerator : ObjectGenerator
{//2019-06-17: copied from ObjectGenerator

    public List<LevelTile.Contents> rings;

    public override void generate(LevelTile[,] tileMap)
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
    public override void generatePostStart(LevelTile[,] tileMap, int posX, int posY)
    {
        for (int i = 0; i < amount; i++)
        {
            while (true)//loop until broken out of
            {
                int rx = Random.Range(0, gridWidth(tileMap));
                int ry = Random.Range(0, gridHeight(tileMap));
                int radius = rings.Count;
                if (outOfAreaToAvoid(tileMap, rx, ry, posX, posY))
                {
                    LevelTile lt = tileMap[rx, ry];
                    if (lt && lt.Available)
                    {
                        lt.Content = rings[0];
                        for (int r = 1; r < radius; r++)
                        {
                            for (int ix = rx - r; ix <= rx + r; ix++)
                            {
                                for (int iy = ry - r; iy <= ry + r; iy++)
                                {
                                    if (inBounds(tileMap, ix, iy)
                                        && outOfAreaToAvoid(tileMap, ix, iy, posX, posY))
                                    {
                                        int manhattenDistance = Mathf.Abs(ix - rx) + Mathf.Abs(iy - ry);
                                        if (manhattenDistance == r)
                                        {
                                            LevelTile ilt = tileMap[ix, iy];
                                            if (ilt && ilt.Available)
                                            {
                                                ilt.Content = rings[r];
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;//break the while loop
                    }
                }
            }
        }
    }

    public override void generatePostReveal(LevelTile[,] tileMap, LevelTile.Contents content)
    {
        throw new System.NotImplementedException();
    }
}
