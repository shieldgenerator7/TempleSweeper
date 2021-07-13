﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayBar : MonoBehaviour
{
    public Image seed;
    [SerializeField]
    private Vector2 spacing = Vector2.zero;
    public Vector2 Spacing
    {
        get
        {
            if (spacing == Vector2.zero)
            {
                float buttonSpacing = Managers.Display.buttonSpacing;
                switch (direction)
                {
                    case Direction.LEFT:
                        spacing.x = -buttonSpacing;
                        break;
                    case Direction.RIGHT:
                        spacing.x = buttonSpacing;
                        break;
                    case Direction.UP:
                        spacing.y = buttonSpacing;
                        break;
                    case Direction.DOWN:
                        spacing.y = -buttonSpacing;
                        break;
                }
            }
            return spacing;
        }
    }
    public Vector2 PerpendicularSpacing
    {
        get
        {
            Vector2 spacing = Spacing;
            return new Vector2(spacing.y, spacing.x);
        }
    }
    public enum Direction
    {
        LEFT,
        RIGHT,
        UP,
        DOWN
    }
    public Direction direction;

    private List<Image> barIcons = new List<Image>();

    /// <summary>
    /// Updates the display to the stat value
    /// </summary>
    /// <param name="stat"></param>
    public void updateDisplay(int statValue)
    {
        //Clear the bar if the stat is zero
        if (statValue <= 0)
        {
            foreach (Image icon in barIcons)
            {
                Destroy(icon.gameObject);
            }
            barIcons.Clear();
            return;
        }
        //Create new ones to get up to the stat
        seed.gameObject.SetActive(true);
        while (statValue > barIcons.Count)
        {
            GameObject displayBarSegment = Instantiate(seed.gameObject);
            barIcons.Add(displayBarSegment.GetComponent<Image>());
        }
        seed.gameObject.SetActive(false);
        //Remove excess ones
        while (statValue < barIcons.Count)
        {
            Destroy(barIcons[0].gameObject);
            barIcons.RemoveAt(0);
        }
        //Position all the icons
        int i = 0;
        foreach (Image icon in barIcons)
        {
            icon.rectTransform.position =
                (Vector2)seed.rectTransform.position + (Spacing * i);
            icon.transform.SetParent(seed.transform.parent);
            i++;
        }
    }

    /// <summary>
    /// Place this display bar at its starting position
    /// </summary>
    /// <param name="row">Row 0 is the first row.</param>
    public void reposition(int row)
    {
        Rect rect = seed.rectTransform.rect;
        rect.y = seed.rectTransform.rect.height * row + 10;
        seed.rectTransform.anchoredPosition = new Vector2(
            seed.rectTransform.anchoredPosition.x,
            ((seed.rectTransform.rect.height * row) + (10 * (row + 1))) * -1
            );
    }

}
