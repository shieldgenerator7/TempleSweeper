using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGenerator : LevelGenerator
{
    public int amount = 10;
    public int radiusToAvoid = 1;
    public LevelTile.TileType tileType;
    public GameObject specificTypePrefab;

    private List<EnemyController> spawnedEntities = new List<EnemyController>();

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
                int ix = Random.Range(0, gridWidth(tileMap));
                int iy = Random.Range(0, gridHeight(tileMap));
                if (Mathf.Abs(posX - ix) > radiusToAvoid
                    || Mathf.Abs(posY - iy) > radiusToAvoid)
                {
                    LevelTile lt = tileMap[ix, iy]?.GetComponent<LevelTile>();
                    if (lt && lt.Available)
                    {
                        generate(lt, tileType, specificTypePrefab);
                        break;//break the while loop
                    }
                }
            }
        }
    }

    public override void generatePostReveal(GameObject[,] tileMap, LevelTile.TileType tileType)
    {
        throw new System.NotImplementedException();
    }

    protected void generate(LevelTile lt, LevelTile.TileType tileType, GameObject typePrefab = null)
    {
        lt.tileType = tileType;
        if (typePrefab != null)
        {
            GameObject item = Instantiate(typePrefab);
            item.transform.position = lt.transform.position;
            spawnedEntities.Add(item.GetComponent<EnemyController>());
        }
    }

    protected bool outOfAreaToAvoid(GameObject[,] tileMap, int posX, int posY, int avoidX, int avoidY)
    {
        return Mathf.Abs(posX - avoidX) > radiusToAvoid
            || Mathf.Abs(posY - avoidY) > radiusToAvoid;
    }

    public override void clearGeneratedObjects()
    {
        foreach(EnemyController ec in spawnedEntities)
        {
            ec.retire();
        }
    }
}
