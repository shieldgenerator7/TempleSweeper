using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumberDisplayer : MonoBehaviour
{
    public List<Sprite> numberSprites;
    public Color treasureColor = Color.yellow;
    public Color trapColor = Color.black;

    [Header("Wheels")]
    public Image wheelOverFlagged;
    public Image wheelFlagged;

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
        itemCount -= LevelManager.getAdjacentCount(levelTile, LevelTile.TileType.MAP);
        itemCount -= LevelManager.getAdjacentCount(levelTile, LevelTile.TileType.RESERVED);
        if (itemCount == 0)
        {
            Destroy(gameObject);
            return;
        }
        int flagCount = LevelManager.getAdjacentFlagCount(levelTile);
        displayNumber(itemCount, flagCount);
        if (LevelManager.getAdjacentCount(levelTile, LevelTile.TileType.TREASURE) > 0)
        {
            GetComponent<SpriteRenderer>().color = treasureColor;
        }
        else
        {
            GetComponent<SpriteRenderer>().color = trapColor;
        }
    }

    private void displayNumber(int objectCount, int flagCount)
    {
        //Set pie sprite
        if (objectCount >= 1)
        {
            GetComponent<SpriteRenderer>().sprite = numberSprites[objectCount - 1];
        }
        else
        {
            GetComponent<SpriteRenderer>().sprite = null;
        }
        //Set flagged wheel
        wheelFlagged.fillAmount = (float)flagCount / (float)objectCount;
        //Set over flagged wheel
        wheelOverFlagged.fillAmount = ((float)flagCount / (float)objectCount) - 1;
    }
}
