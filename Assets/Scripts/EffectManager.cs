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

    public void highlightChange(LevelTile tile)
    {
        highlightEffect(tile, changeHighlighterPrefab, changeHighlighterPool);
    }
    public void highlightTile(LevelTile tile)
    {
        highlightEffect(tile, tileHighlighterPrefab, tileHighlighterPool);
    }
    private void highlightEffect(LevelTile tile, GameObject prefab, List<ChangeHighlighter> pool)
    {
        //Determine position
        Vector2 position = Managers.Level.getPosition(tile);
        //Determine change type
        ChangeHighlighter.ChangeType changeType = ChangeHighlighter.ChangeType.NEUTRAL;
        if (tile.Flagged)
        {
            changeType = ChangeHighlighter.ChangeType.WARN;
        }
        else if (tile.Revealed)
        {
            switch (tile.Content)
            {
                case LevelTile.Contents.MAP:
                    changeType = ChangeHighlighter.ChangeType.DISCOVER;
                    break;
                case LevelTile.Contents.TREASURE:
                    changeType = ChangeHighlighter.ChangeType.DISCOVER;
                    break;
                case LevelTile.Contents.TRAP:
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

    public void moveCursor(LevelTile tile)
    {
        Vector2 position = Managers.Level.getPosition(tile);
        cursorRevealed.transform.position = position;
        cursorHidden.transform.position = position;
        cursorRevealed.gameObject.SetActive(tile.Revealed);
        cursorHidden.gameObject.SetActive(!tile.Revealed);
    }
    public void hideCursor()
    {
        cursorRevealed.gameObject.SetActive(false);
        cursorHidden.gameObject.SetActive(false);
    }
}
