using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayBar : MonoBehaviour
{
    public Image seed;
    public Vector2 spacing = Vector2.zero;

    private List<Image> barIcons = new List<Image>();

    private void Start()
    {
        if (spacing == Vector2.zero)
        {
            spacing.x = seed.rectTransform.rect.width;
            spacing.y = 0;
        }
    }

    /// <summary>
    /// Updates the display to the stat value
    /// </summary>
    /// <param name="stat"></param>
    public void updateDisplay(int statValue)
    {
        //Clear the bar if the stat is zero
        if (statValue <= 0) {
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
                (Vector2)seed.rectTransform.position + (spacing * i);
            icon.transform.SetParent(seed.transform.parent);
            i++;
        }
    }

}
