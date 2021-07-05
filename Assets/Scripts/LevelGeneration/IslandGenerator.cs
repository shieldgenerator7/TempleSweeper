using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandGenerator : LevelGenerator
{
    [Header("Settings")]
    public int landAmount = 16 * 30;
    [Range(1, 10)]
    public int maxLandDistance = 1;//how far a placed land can be from the nearest land
    [Range(1, 8)]
    public int fillInSideCount = 5;//how many surrounding tiles are required to fill in a hole

    public override void generate(TileMap tileMap)
    {
        Vector2 min, max;
        min = max = new Vector2(tileMap.width / 2, tileMap.height / 2);
        //Place the first one
        if (!tileMap[(int)min.x, (int)min.y].Walkable)
        {
            tileMap[(int)min.x, (int)min.y].Walkable = true;
        }
        //Place the rest of them
        for (int i = 0; i < landAmount - 1; i++)
        {
            bool placed = false;
            while (!placed)
            {
                //Randomize new position
                int randX = Random.Range((int)min.x - 1, (int)max.x + 2);
                int randY = Random.Range((int)min.y - 1, (int)max.y + 2);
                Vector2Int randPos = new Vector2Int(randX, randY);
                //If the position is valid,
                if (tileMap.inBounds(randPos))
                {
                    //If the spot is empty,
                    if (!tileMap[randPos].Walkable)
                    {
                        //And it's next to another land,                    
                        if (tileMap.containsLand(randPos, maxLandDistance))
                        {
                            //Place it here
                            tileMap[randX, randY].Walkable = true;
                            placed = true;
                            //Update min and max
                            min.x = Mathf.Min(randX, min.x);
                            min.y = Mathf.Min(randY, min.y);
                            max.x = Mathf.Max(randX, max.x);
                            max.y = Mathf.Max(randY, max.y);
                        }
                    }
                }
            }
        }
        //Fill in holes
        Vector2Int pos = Vector2Int.zero;
        for (int x = (int)min.x; x <= max.x; x++)
        {
            for (int y = (int)min.y; y <= max.y; y++)
            {
                pos.x = x;
                pos.y = y;
                //If it's an empty spot,
                if (!tileMap[pos].Walkable)
                {
                    //And it's surrounded on most sides
                    if (tileMap.landCount(pos, 1) > fillInSideCount)
                    {
                        //Fill it in
                        tileMap[pos].Walkable = true;
                    }
                }
            }
        }
    }

    public override void generatePostStart(TileMap tileMap, int posX, int posY)
    {
        throw new System.NotImplementedException();
    }
    public override void generatePostReveal(TileMap tileMap, LevelTile.Contents content)
    {
        throw new System.NotImplementedException();
    }
}
