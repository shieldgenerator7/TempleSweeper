using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileRevealer : MonoBehaviour
{
    [Tooltip("How long between revealing tile layers")]
    public float revealDelay = 0.1f;

    private List<LevelTileController> tilesToReveal = new List<LevelTileController>();

    private float lastRevealTime = 0;

    // Update is called once per frame
    void Update()
    {
        if (tilesToReveal.Count > 0)
        {
            if (Time.time > lastRevealTime + revealDelay)
            {
                lastRevealTime = Time.time;
                processReveal();
            }
        }
        else
        {
            enabled = false;
        }
    }

    public void revealTilesAround(LevelTileController lt)
    {
        tilesToReveal.Add(lt);
        enabled = true;
    }

    private void processReveal()
    {
        List<LevelTileController> revealLater = new List<LevelTileController>();
        foreach (LevelTileController lt in tilesToReveal)
        {
            lt.Revealed = true;
            //Surrounding tiles
            List<LevelTileController> surroundingTiles = LevelManager.getSurroundingTiles(lt);
            bool emptyAllAround = !surroundingTiles.Any(lt => !lt.Empty);
            if (emptyAllAround)
            {
                foreach (LevelTileController slt in surroundingTiles)
                {
                    if (!slt.Revealed && !slt.Flagged
                        && !tilesToReveal.Contains(slt) && !revealLater.Contains(slt)
                        )
                    {
                        revealLater.Add(slt);
                    }
                }
            }
        }
        tilesToReveal = revealLater;
    }
}
