using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTileController : MonoBehaviour
{//2018-01-02: copied from WolfSim.LevelTile

    public GameObject cover;
    public SpriteRenderer contentsSR;
    public NumberDisplayer numberDisplayer;
    public Sprite trapSprite;
    public Sprite treasureSprite;
    public Sprite mapSprite;

    private LevelTile levelTile;
    public LevelTile LevelTile
    {
        get => levelTile;
        set => levelTile = value;
    }

    private void Start()
    {
        levelTile.onFlaggedChanged += updateFlagged;
        levelTile.onRevealedChanged += updateRevealed;
    }

    private void updateFlagged(bool flagged)
    {
        GetComponentInChildren<FlagDisplayer>().showFlag(flagged);
    }

    private void updateRevealed(bool revealed)
    {
        if (revealed)
        {
            //Destroy the cover
            Destroy(cover);
            //Show the contents
            switch (levelTile.Content)
            {
                case LevelTile.Contents.NONE:
                    numberDisplayer.displayNumber(this);
                    break;
                case LevelTile.Contents.TRAP:
                    contentsSR.sprite = trapSprite;
                    contentsSR.gameObject.AddComponent<ItemDisplayer>();
                    levelTile.Content = LevelTile.Contents.NONE;
                    break;
                case LevelTile.Contents.TREASURE:
                    contentsSR.sprite = treasureSprite;
                    contentsSR.gameObject.AddComponent<ItemDisplayer>();
                    levelTile.Content = LevelTile.Contents.NONE;
                    break;
                case LevelTile.Contents.MAP:
                    contentsSR.sprite = mapSprite;
                    break;
            }
            Managers.Effect.highlightTile(levelTile);
        }
        else
        {
            throw new System.InvalidOperationException("Cannot unreveal tile!");
        }
    }
}
