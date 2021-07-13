using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayBarManager : MonoBehaviour
{
    public DisplayBar healthBar;
    public DisplayBar treasureBar;
    public DisplayBar crateBar;
    public DisplayBar mapBar;

    // Start is called before the first frame update
    void Start()
    {
        //Position bars
        crateBar.reposition(0);
        treasureBar.reposition(0);
        mapBar.reposition(1);
        //Register delegates
        //Update health bar
        Managers.Player.onHealthChanged += (health) => healthBar.updateDisplay(health);
        //Trophies found
        Managers.Player.onTrophiesFoundChanged += (trophiesFound) =>
        {
            treasureBar.updateDisplay(trophiesFound);
            crateBar.updateDisplay(Managers.Player.goalTrophyCount);
        };
        //Map found
        Managers.Player.onMapFoundCountChanged += (mapFoundCount) => mapBar.updateDisplay(mapFoundCount);
        //Call delegates
        Managers.Player.reset();
    }
}
