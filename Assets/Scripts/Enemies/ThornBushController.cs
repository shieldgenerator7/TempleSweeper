using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThornBushController : EnemyController
{
    public Sprite bushSprite;
    /// <summary>
    /// How many times this bush
    /// is allowed to try to place another bush before quitting.
    /// Used to prevent infinite loops.
    /// </summary>
    public int maxTries = 100;

    //Min and Max area of effect in World coordinates
    private Vector2 minArea;
    private Vector2 maxArea;

    protected override void awaken()
    {
        OccupiedTile.trapSprite = bushSprite;
        minArea = maxArea = transform.position;
    }

    protected override void takeTurn()
    {
        int tries = 0;
        //Place a random bush
        while (true && tries < maxTries)
        {
            tries++;

            //Find a random location
            float randX = Random.Range(minArea.x - 1, maxArea.x + 1);
            randX = Mathf.Round(randX);
            float randY = Random.Range(minArea.y - 1, maxArea.y + 1);
            randY = Mathf.Round(randY);
            LevelTile lt = LevelManager.getTile(new Vector2(randX, randY));
            if (lt &&
                (lt.tileType == LevelTile.TileType.EMPTY
                || lt.tileType == LevelTile.TileType.TRAP))
            {
                randX = lt.transform.position.x;
                randY = lt.transform.position.y;
                //Check to make sure this location is next to another bush
                bool nextToOtherBush = false;
                foreach (LevelTile other in LevelManager.getSurroundingTilesManhattan(lt, 1))
                {
                    if (other.trapSprite == bushSprite)
                    {
                        nextToOtherBush = true;
                        break;
                    }
                }
                if (nextToOtherBush)
                {
                    lt.trapSprite = bushSprite;
                    add(lt);

                    //Expand area of effect
                    minArea.x = Mathf.Min(minArea.x, randX);
                    minArea.y = Mathf.Min(minArea.y, randY);
                    maxArea.x = Mathf.Max(maxArea.x, randX);
                    maxArea.y = Mathf.Max(maxArea.y, randY);

                    //break out of while loop
                    break;
                }
            }
        }
    }
}
