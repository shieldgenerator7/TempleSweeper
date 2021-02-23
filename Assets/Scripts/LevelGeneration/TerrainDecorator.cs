using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TerrainDecorator : LevelGenerator
{
    public Terrain terrain;
    public int count = -1;//-1 to paint all land

    public override void generate(LevelTile[,] tileMap)
    {
        List<LevelTile> tiles = Managers.Level.getAllTiles(lt => lt.Walkable);
        if (count < 0)
        {
            tiles.ForEach(lt => lt.terrain = terrain);
        }
        else
        {
            for (int i = 0; i < count; i++)
            {
                int index = Random.Range(0, tiles.Count);
                tiles[index].terrain = terrain;
                tiles.RemoveAt(index);
            }
        }
    }

    public override void generatePostReveal(LevelTile[,] tileMap, LevelTile.Contents content)
    {
        throw new System.NotImplementedException();
    }

    public override void generatePostStart(LevelTile[,] tileMap, int posX, int posY)
    {
        throw new System.NotImplementedException();
    }
}
