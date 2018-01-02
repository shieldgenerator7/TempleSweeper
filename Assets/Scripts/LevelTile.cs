using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTile : MonoBehaviour
{//2018-01-02: copied from WolfSim.LevelTile

    public enum TileType
    {
        EMPTY,
        MINE,
        TREASURE
    };

    public TileType tileType = TileType.EMPTY;

}
