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

    /// <summary>
    /// Reveals the tile type of this tile
    /// </summary>
    public void reveal()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        switch (tileType)
        {
            case TileType.EMPTY:
                sr.sprite = emptySprite; break;
            case TileType.TRAP:
                sr.sprite = trapSprite; break;
            case TileType.TREASURE:
                sr.sprite = treasureSprite; break;
        }
    }
}
