using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyController : MonoBehaviour
{
    [Range(1, 100)]
    public int delayBetweenTurns = 1;//how long until the next turn. 1 = essentially no delay

    private int lastTurnTime = 0;

    private Sprite trapSprite;

    // Start is called before the first frame update
    void Start()
    {
        Managers.Time.onTimePassed += checkForTurn;
        trapSprite = GetComponent<SpriteRenderer>().sprite;
        OccupiedTile.trapSprite = trapSprite;
    }

    private void checkForTurn(int currentTime)
    {
        if (currentTime >= lastTurnTime + delayBetweenTurns)
        {
            lastTurnTime = currentTime;
            takeTurn();
        }
    }

    protected abstract void takeTurn();

    /// <summary>
    /// The tile this entity is currently standing on
    /// </summary>
    public LevelTile OccupiedTile
    {
        get
        {
            return LevelManager.getTile(transform.position);
        }
    }

    protected void moveTo(Vector2 toPos)
    {
        move(transform.position, toPos);
    }

    /// <summary>
    /// Moves this entity from the original position to the destination position
    /// </summary>
    /// <param name="fromPos">The original position in World coordinates</param>
    /// <param name="toPos">The destination position in World coordinates</param>
    protected void move(Vector2 fromPos, Vector2 toPos)
    {
        LevelTile fromTile = LevelManager.getTile(fromPos);
        LevelTile toTile = LevelManager.getTile(toPos);
        //Move entity
        fromTile.tileType = LevelTile.TileType.EMPTY;
        transform.position = toPos;
        toTile.tileType = LevelTile.TileType.TRAP;
        toTile.trapSprite = trapSprite;
        //Hide tiles around fromPos and toPos
        List<LevelTile> tilesToHide = LevelManager.getSurroundingTiles(fromTile);
        tilesToHide.AddRange(LevelManager.getSurroundingTiles(toTile));
        tilesToHide.Add(fromTile);
        tilesToHide.Add(toTile);
        foreach(LevelTile lt in tilesToHide)
        {
            lt.Revealed = false;
        }
    }
}
