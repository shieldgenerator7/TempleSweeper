using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public GameObject changeHighlighterPrefab;

    private List<ChangeHighlighter> changeHighlighterPool = new List<ChangeHighlighter>();

    // Start is called before the first frame update
    void Start()
    {

    }

    public void highlightChange(LevelTile tile)
    {
        //Determine position
        Vector2 position = tile.transform.position;
        //Determine change type
        ChangeHighlighter.ChangeType changeType = ChangeHighlighter.ChangeType.NEUTRAL;
        if (tile.Flagged)
        {
            changeType = ChangeHighlighter.ChangeType.WARN;
        }
        else if (tile.Revealed)
        {
            switch (tile.tileType)
            {
                case LevelTile.TileType.MAP:
                    if (tile.Activated)
                    {
                        changeType = ChangeHighlighter.ChangeType.DISCOVER;
                    }
                    break;
                case LevelTile.TileType.TREASURE:
                    changeType = ChangeHighlighter.ChangeType.DISCOVER;
                    break;
                case LevelTile.TileType.TRAP:
                    changeType = ChangeHighlighter.ChangeType.HIT;
                    break;
                default: break;
            }
        }
        //Find change highlighter instance
        ChangeHighlighter highlighter;
        highlighter = changeHighlighterPool.FirstOrDefault(chl => !chl.Running);
        if (!highlighter)
        {
            GameObject newHighlighter = Instantiate(changeHighlighterPrefab);
            newHighlighter.SetActive(false);
            highlighter = newHighlighter.GetComponent<ChangeHighlighter>();
            changeHighlighterPool.Add(highlighter);
        }
        //Start effect
        highlighter.startEffect(position, changeType);
    }
}
