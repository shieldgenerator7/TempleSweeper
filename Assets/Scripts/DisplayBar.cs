using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayBar : MonoBehaviour
{
    public Image seed;
    public string statTag;
    public Vector2 spacing = Vector2.zero;

    private void Start()
    {
        if (statTag == null || statTag == "")
        {
            statTag = seed.tag;
        }
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
        //Remove previous display objects
        foreach (GameObject go in GameObject.FindGameObjectsWithTag(statTag))
        {
            Destroy(go);
        }
        //Create new ones to refresh the health bar
        seed.gameObject.SetActive(true);
        for (int i = 0; i < statValue; i++)
        {
            GameObject displayBarSegment = Instantiate(seed.gameObject);
            displayBarSegment.GetComponent<Image>().rectTransform.position =
                (Vector2)seed.rectTransform.position + (spacing * i);
            displayBarSegment.transform.SetParent(seed.transform.parent);
        }
        seed.gameObject.SetActive(false);
    }

}
