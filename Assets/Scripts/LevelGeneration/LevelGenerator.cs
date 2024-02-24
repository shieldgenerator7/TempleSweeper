using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LevelGenerator : ScriptableObject
{
    public abstract void generate(TileMap tileMap);

    public abstract void generatePostStart(TileMap tileMap, int posX, int posY);

    public abstract void generatePostReveal(TileMap tileMap, LevelTile.Contents content);

    public virtual void clearGeneratedObjects()
    {
        throw new System.NotImplementedException();
    }
}
