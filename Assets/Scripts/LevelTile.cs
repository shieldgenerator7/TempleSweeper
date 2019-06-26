using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTile : MonoBehaviour
{//2018-01-02: copied from WolfSim.LevelTile

    public GameObject cover;
    public SpriteRenderer contentsSR;
    public NumberDisplayer numberDisplayer;
    public Sprite trapSprite;
    public Sprite treasureSprite;
    public Sprite mapSprite;

    public enum TileType
    {
        EMPTY,
        TRAP,
        TREASURE,
        MAP,
        RESERVED//empty, but not able to be assigned to anything
    };

    public TileType tileType = TileType.EMPTY;
    public int indexX, indexY;//its index in the grid

    /// <summary>
    /// When flagged, a tile cannot be revealed
    /// </summary>
    private bool flagged = false;
    public bool Flagged
    {
        get { return flagged; }
        set
        {
            flagged = value;
            GetComponentInChildren<FlagDisplayer>().showFlag(flagged);
        }
    }

    private bool revealed = false;
    public bool Revealed
    {
        get { return revealed; }
        set
        {
            revealed = value;
            if (revealed)
            {
                //Destroy the cover
                Destroy(cover);
                //Show the contents
                switch (tileType)
                {
                    case TileType.RESERVED:
                    case TileType.EMPTY:
                        numberDisplayer.displayNumber(this);
                        break;
                    case TileType.TRAP:
                        contentsSR.sprite = trapSprite;
                        contentsSR.gameObject.AddComponent<ItemDisplayer>();
                        tileType = TileType.EMPTY;
                        break;
                    case TileType.TREASURE:
                        contentsSR.sprite = treasureSprite;
                        contentsSR.gameObject.AddComponent<ItemDisplayer>();
                        tileType = TileType.EMPTY;
                        break;
                    case TileType.MAP:
                        contentsSR.sprite = mapSprite;
                        break;
                }
            }
            else
            {
                throw new System.InvalidOperationException("Cannot unreveal tile!");
            }
        }
    }

    private bool activated = false;
    public bool Activated
    {
        get { return activated; }
        set
        {
            activated = value;
            if (activated)
            {
                if (tileType != TileType.MAP)
                {
                    throw new System.InvalidOperationException("Cannot activate non-map tile! tile type: " + tileType);
                }
                Managers.Player.MapFoundCount++;
                contentsSR.gameObject.AddComponent<ItemDisplayer>();
            }
            else
            {
                throw new System.InvalidOperationException("Cannot unactivate tile!");
            }
        }
    }

    public bool Empty
    {
        get
        {
            return empty(tileType);
        }
    }

    public static bool empty(TileType type)
    {
        return type == TileType.EMPTY
            || type == TileType.RESERVED
            || type == TileType.MAP;
    }

    public bool Available
    {
        get
        {
            return available(tileType);
        }
    }

    public static bool available(TileType type)
    {
        return type == TileType.EMPTY;
    }
}
