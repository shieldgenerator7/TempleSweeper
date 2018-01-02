using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumberDisplayer : MonoBehaviour
{
    public List<Sprite> numberSprites;
    public Color treasureColor = Color.yellow;
    public Color trapColor = Color.black;

    public void displayNumber(LevelTile lt)
    {
        displayNumber(LevelManager.getAdjacentCount(lt, LevelTile.TileType.EMPTY, true));
        if (LevelManager.getAdjacentCount(lt, LevelTile.TileType.TREASURE) > 0)
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
