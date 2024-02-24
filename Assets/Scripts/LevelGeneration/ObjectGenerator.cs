using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ObjectGenerator", menuName = "Level/Generator/Object")]
public class ObjectGenerator : LevelGenerator
{
    public int amount = 10;
    public int radiusToAvoid = 1;
    public LevelTile.Contents content;

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
                int ix = Random.Range(0, tileMap.width);
                int iy = Random.Range(0, tileMap.height);
                if (Mathf.Abs(posX - ix) > radiusToAvoid
                    || Mathf.Abs(posY - iy) > radiusToAvoid)
                {
                    LevelTile lt = tileMap[ix, iy];
                    if (lt.Available)
                    {
                        lt.Content = content;
                        break;//break the while loop
                    }
                }
            }
        }
    }

    public override void generatePostReveal(TileMap tileMap, LevelTile.Contents content)
    {
        throw new System.NotImplementedException();
    }

    protected bool outOfAreaToAvoid(TileMap tileMap, int posX, int posY, int avoidX, int avoidY)
    {
        return Mathf.Abs(posX - avoidX) > radiusToAvoid
            || Mathf.Abs(posY - avoidY) > radiusToAvoid;
    }
}
