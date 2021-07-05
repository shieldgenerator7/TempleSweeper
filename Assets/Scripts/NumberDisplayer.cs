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

    [SerializeField]
    private int detectedCount;
    [SerializeField]
    private int flagCount;

    private LevelTileController levelTile;

    public void displayNumber(LevelTileController parent)
    {
        levelTile = parent;
        displayNumber();
    }
    public void displayNumber()
    {
        if (!levelTile || !levelTile.LevelTile.Walkable)
        {
            return;
        }
        gameObject.SetActive(true);
        detectedCount = Managers.Level.TileMap.getDetectedCount(levelTile.LevelTile.Position);
        if (detectedCount == 0)
        {
            gameObject.SetActive(false);
            return;
        }
        flagCount = Managers.Level.TileMap.getAdjacentFlagCount(levelTile.LevelTile.Position);
        displayNumber(detectedCount, flagCount);
        if (Managers.Level.TileMap.getAdjacentCount(levelTile.LevelTile.Position, LevelTile.Contents.TREASURE) > 0)
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
