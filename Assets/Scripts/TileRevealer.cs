using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileRevealer : MonoBehaviour
{
    [Tooltip("How long between revealing tile layers")]
    public float revealDelay = 0.1f;

    private List<LevelTile> tilesToReveal = new List<LevelTile>();

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

    public void revealTilesAround(LevelTile lt)
    {
        tilesToReveal.Add(lt);
        enabled = true;
    }

    private void processReveal()
    {
        List<LevelTile> revealLater = new List<LevelTile>();
        foreach (LevelTile lt in tilesToReveal)
        {
            lt.Revealed = true;
            //Surrounding tiles
            List<LevelTile> surroundingTiles = Managers.Level.TileMap.getSurroundingLandTiles(lt.Position);
            bool emptyAllAround = !surroundingTiles.Any(lt => lt.Detectable);
            if (emptyAllAround)
            {
                foreach (LevelTile slt in surroundingTiles)
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
