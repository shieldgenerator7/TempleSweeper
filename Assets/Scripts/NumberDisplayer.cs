using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumberDisplayer : MonoBehaviour
{
    public List<Sprite> numberSprites;
    public Color treasureColor = Color.yellow;
    public Color trapColor = Color.black;

    private LevelTile levelTile;

    public void displayNumber(LevelTile parent)
    {
        levelTile = parent;
        displayNumber();
    }
    public void displayNumber() { 
        if (levelTile == null)
        {
            return;
        }
        int itemCount = LevelManager.getAdjacentCount(levelTile, LevelTile.TileType.EMPTY, true);
        if (itemCount == 0)
        {
            Destroy(gameObject);
            return;
        }
        displayNumber(itemCount);
        if (LevelManager.getAdjacentCount(levelTile, LevelTile.TileType.TREASURE) > 0)
        {
            GetComponent<SpriteRenderer>().color = treasureColor;
        }
        else
        {
            GetComponent<SpriteRenderer>().color = trapColor;
        }
    }

    public void displayNumber(int count)
    {
        if (count >= 1)
        {
            GetComponent<SpriteRenderer>().sprite = numberSprites[count - 1];
        }
    }
}
