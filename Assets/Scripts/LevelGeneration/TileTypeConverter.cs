using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileTypeConverter", menuName = "Level/Converter/TileType")]
public class TileTypeConverter : LevelGenerator
{//2019-06-17: copied from ObjectGenerator

    public LevelTile.Contents fromContent;
    public LevelTile.Contents toContent;

    public int maxConvert = -1;
    public bool randomConvert = false;

    public override void generate(TileMap tileMap)
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
    public override void generatePostStart(TileMap tileMap, int posX, int posY)
    {
        convert(tileMap);
    }

    public override void generatePostReveal(TileMap tileMap, LevelTile.Contents content)
    {
        convert(tileMap);
    }

    private void convert(TileMap tileMap)
    {
        int width = tileMap.width;
        int height = tileMap.height;
        if (!randomConvert)
        {
            int converted = 0;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    LevelTile lt = tileMap[i, j];
                    if (lt.Walkable)
                    {
                        if (lt.Content == fromContent)
                        {
                            lt.Content = toContent;
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
                    LevelTile lt = tileMap[rx, ry];
                    if (lt.Walkable && lt.Content == fromContent)
                    {
                        lt.Content = toContent;
                        break;
                    }
                }
            }
        }
    }
}
