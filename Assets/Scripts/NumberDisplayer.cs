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
    public Image wheelPresent;

    private LevelTile levelTile;
    private SpriteRenderer sr;

    public void displayNumber(LevelTile parent)
    {
        levelTile = parent;
        sr = GetComponent<SpriteRenderer>();
        displayNumber();
    }
    public void displayNumber()
    {
        if (levelTile == null)
        {
            return;
        }
        gameObject.SetActive(true);
        if (levelTile.tileType != LevelTile.TileType.EMPTY
            && levelTile.tileType != LevelTile.TileType.RESERVED)
        {
            sr.color = Color.white;
            wheelFlagged.fillAmount = 0;
            wheelOverFlagged.fillAmount = 0;
            return;
        }
        int itemCount = LevelManager.getAdjacentCount(levelTile, LevelTile.TileType.EMPTY, true);
        itemCount -= LevelManager.getAdjacentCount(levelTile, LevelTile.TileType.MAP);
        itemCount -= LevelManager.getAdjacentCount(levelTile, LevelTile.TileType.RESERVED);
        if (itemCount == 0)
        {
            gameObject.SetActive(false);
            return;
        }
        int flagCount = LevelManager.getAdjacentFlagCount(levelTile);
        displayNumber(itemCount, flagCount);
        if (LevelManager.getAdjacentCount(levelTile, LevelTile.TileType.TREASURE) > 0)
        {
            sr.color = treasureColor;
        }
        else
        {
            sr.color = trapColor;
        }
    }

    private void displayNumber(int objectCount, int flagCount)
    {
        //Set pie sprite
        if (objectCount >= 1)
        {
            sr.sprite = numberSprites[objectCount - 1];
        }
        else
        {
            sr.sprite = null;
        }
        //Set flagged wheel
        wheelFlagged.fillAmount = (float)flagCount / (float)objectCount;
        //Set over flagged wheel
        wheelOverFlagged.fillAmount = ((float)flagCount / (float)objectCount) - 1;
    }
}
