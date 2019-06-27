using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfController : EnemyController
{
    public int range = 1;
    public int minTravelInDirection = 3;
    public int maxTravelInDirection = 5;

    private int turnsTraveledInDirection = 0;
    private int currentTravelLimit = 0;

    private int dirX = 1;
    private int dirY = 1;

    private bool revealStuns = true;

    protected override void takeTurn()
    {
        if (OccupiedTile.Revealed)
        {
            if (revealStuns)
            {
                revealStuns = false;
                //don't move if the player revealed its tile
                return;
            }
        }
        revealStuns = true;

        //Decide what to do
        if (LevelManager.getAdjacentCount(OccupiedTile, LevelTile.TileType.EMPTY) > 0)
        {
            if (turnsTraveledInDirection == currentTravelLimit)
            {
                chooseDirection();
            }
            //Try to find a spot to run to
            if (canMoveInDirection())
            {
                moveInDirection();
            }
            else
            {
                chooseDirection();
            }
        }
    }

    private void chooseDirection()
    {
        dirX = 0;
        dirY = 0;
        while (dirX == 0 && dirY == 0)
        {
            dirX = Random.Range(-range, range + 1);
            dirY = Random.Range(-range, range + 1);
        }
        currentTravelLimit = Random.Range(minTravelInDirection, maxTravelInDirection + 1);
        turnsTraveledInDirection = 0;
    }

    private bool canMoveInDirection()
    {
        int xIndex = LevelManager.getXIndex(transform.position);
        xIndex += dirX;
        int yIndex = LevelManager.getYIndex(transform.position);
        yIndex += dirY;
        LevelTile destTile = LevelManager.getTile(xIndex, yIndex);
        return destTile
            && destTile.tileType == LevelTile.TileType.EMPTY
            ;
    }

    private void moveInDirection()
    {
        int xIndex = LevelManager.getXIndex(transform.position);
        xIndex += dirX;
        int yIndex = LevelManager.getYIndex(transform.position);
        yIndex += dirY;
        moveTo(LevelManager.getWorldPos(xIndex, yIndex));
        //
        turnsTraveledInDirection++;
    }
}
