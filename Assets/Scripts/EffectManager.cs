using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public GameObject cursorRevealed;
    public GameObject cursorHidden;

    public GameObject changeHighlighterPrefab;
    private List<ChangeHighlighter> changeHighlighterPool = new List<ChangeHighlighter>();

    public GameObject tileHighlighterPrefab;
    private List<ChangeHighlighter> tileHighlighterPool = new List<ChangeHighlighter>();

    // Start is called before the first frame update
    void Start()
    {

    }

    public void highlightChange(LevelTileController tile)
    {
        highlightEffect(tile, changeHighlighterPrefab, changeHighlighterPool);
    }
    public void highlightTile(LevelTileController tile)
    {
        highlightEffect(tile, tileHighlighterPrefab, tileHighlighterPool);
    }
    private void highlightEffect(LevelTileController tile, GameObject prefab, List<ChangeHighlighter> pool)
    {
        //Determine position
        Vector2 position = tile.transform.position;
        //Determine change type
        ChangeHighlighter.ChangeType changeType = ChangeHighlighter.ChangeType.NEUTRAL;
        if (tile.LevelTile.Flagged)
        {
            changeType = ChangeHighlighter.ChangeType.WARN;
        }
        else if (tile.LevelTile.Revealed)
        {
            switch (tile.tileType)
            {
                case LevelTileController.TileType.MAP:
                    changeType = ChangeHighlighter.ChangeType.DISCOVER;
                    break;
                case LevelTileController.TileType.TREASURE:
                    changeType = ChangeHighlighter.ChangeType.DISCOVER;
                    break;
                case LevelTileController.TileType.TRAP:
                    changeType = ChangeHighlighter.ChangeType.HIT;
                    break;
                default: break;
            }
        }
        //Find change highlighter instance
        ChangeHighlighter highlighter;
        highlighter = pool.FirstOrDefault(chl => !chl.Running);
        if (!highlighter)
        {
            GameObject newHighlighter = Instantiate(prefab);
            newHighlighter.SetActive(false);
            highlighter = newHighlighter.GetComponent<ChangeHighlighter>();
            pool.Add(highlighter);
        }
        //Start effect
        highlighter.startEffect(position, changeType);
    }

    public void moveCursor(LevelTileController tile)
    {
        cursorRevealed.transform.position = tile.transform.position;
        cursorHidden.transform.position = tile.transform.position;
        cursorRevealed.gameObject.SetActive(tile.LevelTile.Revealed);
        cursorHidden.gameObject.SetActive(!tile.LevelTile.Revealed);
    }
    public void hideCursor()
    {
        cursorRevealed.gameObject.SetActive(false);
        cursorHidden.gameObject.SetActive(false);
    }
}
