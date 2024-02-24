using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RiverGenerator", menuName = "Level/Generator/River")]
public class RiverGenerator : LevelGenerator
{//2019-06-17: copied from IslandGenerator
    public int streamCount = 2;
    public int fillInRiverCount = 4;
    [Header("River Bounds")]
    public int minX = 0;
    public int maxX = 47;
    public int minY = 9;
    public int maxY = 25;
    [Header("Allowed Directions")]
    public bool up = true;
    public bool down = true;
    public bool left = false;
    public bool right = true;

    public enum Edge
    {
        TOP,
        BOTTOM,
        LEFT,
        RIGHT
    }
    [Header("Start Edge")]
    public Edge startEdge;

    public override void generate(TileMap tileMap)
    {
        for (int n = 0; n < streamCount; n++)
        {
            int startX = minX;
            int startY = minY;
            int endX = -1;
            int endY = -1;
            switch (startEdge)
            {
                case Edge.TOP:
                    startY = maxY;
                    endY = minY;
                    startX = (int)Random.Range(minX, maxX + 1);
                    break;
                case Edge.BOTTOM:
                    startY = minY;
                    endY = maxY;
                    startX = (int)Random.Range(minX, maxX + 1);
                    break;
                case Edge.LEFT:
                    startX = minX;
                    endX = maxX;
                    startY = (int)Random.Range(minY, maxY + 1);
                    break;
                case Edge.RIGHT:
                    startX = maxX;
                    endX = minX;
                    startY = (int)Random.Range(minY, maxY + 1);
                    break;
            }
            int px = startX;
            int py = startY;
            tileMap[px, py].Walkable = false;
            while (px != endX && py != endY)
            {
                //Update px and/or py
                int diffX = Random.Range(
                    ((left) ? -1 : 0),
                    ((right) ? 1 : 0) + 1
                    );
                int diffY = Random.Range(
                    ((down) ? -1 : 0),
                    ((up) ? 1 : 0) + 1
                    );
                px += diffX;
                py += diffY;

                //Make this grid cell a river cell
                px = Mathf.Clamp(px, minX, maxX);
                py = Mathf.Clamp(py, minY, maxY);
                tileMap[px, py].Walkable = false;
            }
        }
        //Fill in holes
        Vector2Int pos = Vector2Int.zero;
        for (int x = minX - 1; x <= maxX + 1; x++)
        {
            for (int y = minY - 1; y <= maxY + 1; y++)
            {
                pos.x = x;
                pos.y = y;
                if (tileMap.inBounds(pos))
                {
                    //If it's not an empty spot,
                    if (tileMap[pos].Walkable)
                    {
                        //And it's surrounded on most sides
                        if (tileMap.landCount(pos, 1) < fillInRiverCount)
                        {
                            //Fill it in
                            tileMap[pos].Walkable = false;
                        }
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
