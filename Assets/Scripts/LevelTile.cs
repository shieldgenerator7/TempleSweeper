using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelTile
{
    public enum Contents
    {
        NONE,
        TRAP,
        TREASURE,
        MAP
    }
    private Contents contents = Contents.NONE;
    public Contents Content
    {
        get => (Walkable)
            ? contents
            : Contents.NONE;
        set
        {
            if (!Locked)
            {
                contents = value;
                if (contents != Contents.NONE)
                {
                    Walkable = true;
                }
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

    public int x { get => Position.x; }
    public int y { get => Position.y; }
    public readonly Vector2Int Position;

    public Terrain terrain;

    public LevelTile(int x, int y)
    {
        this.Position = new Vector2Int(x, y);
        this.contents = Contents.NONE;
        this.Locked = false;
        this.Walkable = false;
    }

    /// <summary>
    /// True if this tile has a flag on it
    /// </summary>
    public bool Flagged
    {
        get => (Walkable)
            ? flagged
            : false;
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
        get => (Walkable)
            ? revealed
            : true;
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

    public bool Available
        => contents == Contents.NONE && !Locked && Walkable;

    public bool Detectable
        => Content == Contents.TRAP
        || Content == Contents.TREASURE;
}
