using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCharacter : MonoBehaviour
{

    private int health = 3;
    public int Health
    {
        get { return health; }
        set
        {
            //Set the new health value
            health = Mathf.Clamp(value, 0, startHealth);
            //Remove previous display objects
            foreach (GameObject go in GameObject.FindGameObjectsWithTag("HealthBar"))
            {
                Destroy(go);
            }
            //Create new ones to refresh the health bar
            healthImage.gameObject.SetActive(true);
            for (int i = 0; i < health; i++)
            {
                GameObject healthBar = Instantiate(healthImage.gameObject);
                healthBar.GetComponent<Image>().rectTransform.position =
                    (Vector2)healthImage.rectTransform.position + (healthBarImageSpacing * i);
                healthBar.transform.parent = healthImage.transform.parent;
            }
            healthImage.gameObject.SetActive(false);
        }
    }
    public int startHealth = 3;

    public int trophiesFound = 0;
    public int goalTrophyCount = 10;

    public Image healthImage;
    public Vector2 healthBarImageSpacing;

    private void Start()
    {
        reset();
    }

    /// <summary>
    /// Call this when the player reveals a mine
    /// </summary>
    /// <returns>True if he's still alive, false if game over</returns>
    public bool takeHit()
    {
        Health--;
        return alive();
    }
    public bool alive()
    {
        return Health > 0;
    }
    public bool findTrophy()
    {
        trophiesFound++;
        return goalAchieved();
    }
    public bool goalAchieved()
    {
        return trophiesFound >= goalTrophyCount;
    }
    public void reset()
    {
        Health = startHealth;
        trophiesFound = 0;
    }
}
