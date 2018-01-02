using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTile : MonoBehaviour
{//2018-01-02: copied from WolfSim.LevelTile

    public Sprite emptySprite;
    public Sprite trapSprite;
    public Sprite treasureSprite;

    public enum TileType
    {
        EMPTY,
        TRAP,
        TREASURE
    };

    public TileType tileType = TileType.EMPTY;
    public int indexX, indexY;//its index in the grid

    /// <summary>
    /// When flagged, a tile cannot be revealed
    /// </summary>
    public bool flagged = false;

    private bool revealed = false;

    /// <summary>
    /// Reveals the tile type of this tile
    /// </summary>
    public void reveal()
    {
        revealed = true;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        switch (tileType)
        {
            case TileType.EMPTY:
                sr.sprite = emptySprite;
                GetComponentInChildren<NumberDisplayer>().displayNumber(this);
                break;
            case TileType.TRAP:
                sr.sprite = trapSprite; break;
            case TileType.TREASURE:
                sr.sprite = treasureSprite; break;
        }
    }
    /// <summary>
    /// Whether this tile has revealed
    /// </summary>
    /// <returns></returns>
    public bool hasRevealed()
    {
        return revealed;
    }

    public void flag(bool isFlagged)
    {
        flagged = isFlagged;
        GetComponentInChildren<FlagDisplayer>().showFlag(flagged);
    }
}
