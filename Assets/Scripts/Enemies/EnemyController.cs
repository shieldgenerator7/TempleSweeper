using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyController : MonoBehaviour
{
    [Range(1, 100)]
    public int delayBetweenTurns = 1;//how long until the next turn. 1 = essentially no delay
    [Range(-1, 10)]
    /// <summary>
    /// When this enemy moves, the range around it that it obscures
    /// 1 = just the 8 squares around it, 0 = just itself, -1 = none
    /// </summary>
    public int obscureRange = 1;
    public LevelTile.TileType selfType = LevelTile.TileType.TRAP;

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
    protected void move(Vector2 fromPos, Vector2 toPos,
        LevelTile.TileType leaveBehindType = LevelTile.TileType.EMPTY)
    {
        LevelTile fromTile = LevelManager.getTile(fromPos);
        LevelTile toTile = LevelManager.getTile(toPos);

        //Move entity
        fromTile.tileType = leaveBehindType;
        transform.position = toPos;
        toTile.tileType = selfType;
        toTile.trapSprite = trapSprite;

        //Hide tiles around fromPos and toPos
        List<LevelTile> tilesToHide = new List<LevelTile>();
        if (obscureRange > 0)
        {
            tilesToHide.AddRange(LevelManager.getSurroundingTiles(fromTile, obscureRange));
            tilesToHide.AddRange(LevelManager.getSurroundingTiles(toTile, obscureRange));
        }
        if (obscureRange >= 0)
        {
            tilesToHide.Add(fromTile);
            tilesToHide.Add(toTile);
        }
        foreach (LevelTile lt in tilesToHide)
        {
            lt.Revealed = false;
        }

        //Update trap numbers of the tiles around from and to
        List<LevelTile> tilesToUpdate = new List<LevelTile>();
        tilesToUpdate.AddRange(LevelManager.getSurroundingTiles(fromTile));
        tilesToUpdate.AddRange(LevelManager.getSurroundingTiles(toTile));
        foreach (LevelTile lt in tilesToUpdate)
        {
            if (lt.Revealed)
            {
                lt.numberDisplayer.displayNumber();
            }
        }
    }

    /// <summary>
    /// Kill this enemy
    /// </summary>
    protected void kill()
    {
        gameObject.SetActive(false);
        Managers.Time.onTimePassed -= checkForTurn;
    }
}
