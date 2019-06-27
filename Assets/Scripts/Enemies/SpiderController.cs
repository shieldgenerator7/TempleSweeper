using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderController : EnemyController
{
    public int range = 1;

    public Sprite webSprite;

    protected override void takeTurn()
    {
        //Kill the spider if it gets revealed
        if (OccupiedTile.Revealed)
        {
            kill();
        }

        int validSpaceCount = LevelManager.getAdjacentCount(OccupiedTile, LevelTile.TileType.EMPTY);
        validSpaceCount += LevelManager.getAdjacentCount(OccupiedTile, LevelTile.TileType.TRAP);
        if (validSpaceCount > 0)
        {
            //Try to find a spot to run to
            while (true)
            {
                int randX = Random.Range(-range, range + 1);
                int randY = Random.Range(-range, range + 1);
                if (randX != 0 || randY != 0)
                {
                    int xIndex = LevelManager.getXIndex(transform.position);
                    xIndex += randX;
                    int yIndex = LevelManager.getYIndex(transform.position);
                    yIndex += randY;
                    LevelTile destTile = LevelManager.getTile(xIndex, yIndex);
                    if (destTile
                        && (destTile.tileType == LevelTile.TileType.EMPTY
                        || destTile.tileType == LevelTile.TileType.TRAP))
                    {
                        LevelTile fromTile = OccupiedTile;
                        move(
                            transform.position,
                            LevelManager.getWorldPos(xIndex, yIndex),
                            LevelTile.TileType.TRAP
                            );
                        //Lay a web trap
                        fromTile.trapSprite = webSprite;
                        break;
                    }
                }
            }
        }
    }
}
