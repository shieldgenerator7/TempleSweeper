using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfController : EnemyController
{
    public int range = 1;

    protected override void takeTurn()
    {
        if (LevelManager.getAdjacentCount(OccupiedTile, LevelTile.TileType.EMPTY) > 0)
        {
            //Try to find a spot to run to
            while (true)
            {
                int randX = Random.Range(-range, range + 1);
                int randY = Random.Range(-range, range + 1);
                Debug.Log("random: (" + randX + ", " + randY + ")");
                if (randX != 0 || randY != 0)
                {
                    int xIndex = LevelManager.getXIndex(transform.position);
                    xIndex += randX;
                    int yIndex = LevelManager.getYIndex(transform.position);
                    yIndex += randY;
                    LevelTile destTile = LevelManager.getTile(xIndex, yIndex);
                    if (destTile && destTile.tileType == LevelTile.TileType.EMPTY){
                        moveTo(LevelManager.getWorldPos(xIndex, yIndex));
                        break;
                    }
                }
            }
        }
    }
}
