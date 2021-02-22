using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelTile
{
    public enum Contents
    {
        NONE,
        TREASURE,
        MAP,
        TRAP
    }
    private Contents contents = Contents.NONE;
    public Contents Content
    {
        get => contents;
        set
        {
            if (!Locked)
            {
                contents = value;
            }
        }
    }

    /// <summary>
    /// True if its contents are non-editable
    /// </summary>
    public bool Locked = false;

    /// <summary>
    /// True if it is land, false if it is water
    /// </summary>
    public bool Walkable = false;

    public LevelTile()
    {
        contents = Contents.NONE;
        Locked = false;
        Walkable = false;
    }
    public LevelTile(Contents content, bool locked = false)
    {
        this.contents = content;
        this.Locked = locked;
        this.Walkable = true;
    }

    /// <summary>
    /// True if this tile has a flag on it
    /// </summary>
    public bool Flagged
    {
        get => flagged;
        set
        {
            if (!revealed)
            {
                flagged = value;
                onFlaggedChanged?.Invoke(flagged);
            }
        }
    }
    private bool flagged = false;
    public delegate void OnFlaggedChanged(bool flagged);
    public OnFlaggedChanged onFlaggedChanged;

    /// <summary>
    /// True if this tile has been revealed
    /// </summary>
    public bool Revealed
    {
        get => revealed;
        set
        {
            if (!flagged)
            {
                revealed = value;
                onRevealedChanged?.Invoke(revealed);
            }
        }
    }
    private bool revealed = false;
    public delegate void OnRevealedChanged(bool revealed);
    public OnRevealedChanged onRevealedChanged;
}
