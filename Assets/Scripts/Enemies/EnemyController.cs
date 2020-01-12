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

    public enum Response
    {
        NOTHING,//the enemy does not respond
        STUN,//the enemy becomes stunned
        DIE//the enemy dies
    }
    public Response onFlagResponse;
    public Response onRevealResponse;

    // Start is called before the first frame update
    void Start()
    {
        Managers.Time.onTimePassed += checkForTurn;
        Managers.Level.onTileRevealed += OnReveal;
        Managers.Level.onTileFlagged += OnFlag;
        trapSprite = GetComponent<SpriteRenderer>().sprite;
        OccupiedTile.trapSprite = trapSprite;
        awaken();
    }

    protected virtual void awaken() { }

    private void checkForTurn(int currentTime)
    {
        if (selfType == LevelTile.TileType.TRAP
            && obscureRange >= 0)
        {
            if (OccupiedTile.Revealed)
            {
                OccupiedTile.Revealed = false;
                LevelManager.updateSurroundingTiles(OccupiedTile);
            }
        }
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
        LevelManager.hideSurroundingTiles(fromTile, obscureRange);
        LevelManager.hideSurroundingTiles(toTile, obscureRange);

        //Update trap numbers of the tiles around from and to
        LevelManager.updateSurroundingTiles(fromTile);
        LevelManager.updateSurroundingTiles(toTile);
    }

    /// <summary>
    /// Adds a type to the given position
    /// </summary>
    /// <param name="addPos">The position to add something to in World coordinates</param>
    /// <param name="addType">The type to add</param>
    protected void add(Vector2 addPos,
        LevelTile.TileType addType = LevelTile.TileType.TRAP)
    {
        LevelTile addTile = LevelManager.getTile(addPos);
        add(addTile, addType);
    }
    protected void add(LevelTile addTile,
        LevelTile.TileType addType = LevelTile.TileType.TRAP)
    {
        addTile.tileType = addType;
        LevelManager.hideSurroundingTiles(addTile, obscureRange);
        LevelManager.updateSurroundingTiles(addTile);
    }

    /// <summary>
    /// Kill this enemy
    /// </summary>
    protected void kill()
    {
        gameObject.SetActive(false);
        Managers.Time.onTimePassed -= checkForTurn;
        Managers.Level.onTileRevealed -= OnReveal;
        Managers.Level.onTileFlagged -= OnFlag;
    }

    public void retire()
    {
        kill();
        Destroy(gameObject);
    }

    public void OnFlag(LevelTile lt)
    {
        if ((Vector2)lt.transform.position == (Vector2)transform.position)
        {
            OnResponse(onFlagResponse);
        }
    }

    public void OnReveal(LevelTile lt)
    {
        if ((Vector2)lt.transform.position == (Vector2)transform.position)
        {
            OnResponse(onRevealResponse);
        }
    }

    private void OnResponse(Response response)
    {
        switch (response)
        {
            case Response.NOTHING:
                break;
            case Response.STUN:
                break;
            case Response.DIE:
                kill();
                break;
            default:
                throw new UnityException("Response option not processed! response: " + response);
        }
    }
}
