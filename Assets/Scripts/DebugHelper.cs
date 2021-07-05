using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugHelper : MonoBehaviour
{
    public LevelTile.Contents selectContent;
    public Color highlightColor = Color.blue;

    /// <summary>
    /// Highlights all the tiles of the selected type
    /// </summary>
    public void highlightAll()
    {
        foreach (LevelTileController lt in FindObjectsOfType<LevelTileController>())
        {
            if (lt.LevelTile.Content == selectContent)
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
        foreach (LevelTile lt in Managers.Level.TileMap.getTiles(alt => alt.Content == selectContent))
        {
            if (lt.Content == selectContent)
            {
                lt.Revealed = true;
            }
        }
    }
}
