using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugHelper : MonoBehaviour
{
    public LevelTile.TileType selectType;
    public Color highlightColor = Color.blue;

    /// <summary>
    /// Highlights all the tiles of the selected type
    /// </summary>
    public void highlightAll()
    {
        foreach (LevelTile lt in FindObjectsOfType<LevelTile>())
        {
            if (lt.tileType == selectType)
            {
                if (lt.cover)
                {
                    lt.cover.GetComponent<SpriteRenderer>().color = highlightColor;
                }
            }
        }
    }

    /// <summary>
    /// Reveals all the tiles of the selected type
    /// </summary>
    public void revealAll()
    {
        foreach (LevelTile lt in FindObjectsOfType<LevelTile>())
        {
            if (lt.tileType == selectType)
            {
                lt.Revealed = true;
            }
        }
    }
}
