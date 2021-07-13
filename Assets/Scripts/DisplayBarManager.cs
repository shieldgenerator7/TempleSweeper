using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayBarManager : MonoBehaviour
{
    public DisplayBar healthBar;
    public DisplayBar treasureBar;
    public DisplayBar crateBar;
    public DisplayBar mapBar;

    [Header("Buttons")]
    [SerializeField]
    private int desiredWidth = 1920;
    [SerializeField]
    private int desiredHeight = 1080;
    public float buttonSpacing = 55;
    private float originalSpacing;

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
        updateDisplayBars();

        //Screen size
        originalSpacing = buttonSpacing;
    }

    public void updateDisplayBars()
    {
        healthBar.updateDisplay(Managers.Player.Health);
        treasureBar.updateDisplay(Managers.Player.TrophiesFound);
        crateBar.updateDisplay(Managers.Player.goalTrophyCount);
        mapBar.updateDisplay(Managers.Player.MapFoundCount);
    }

    public void updateScreenConstants(int width, int height)
    {
        int dim;
        int desiredDim;
        if (width > height)
        {
            dim = width;
            desiredDim = desiredWidth;
        }
        else
        {
            dim = height;
            desiredDim = desiredHeight;
        }
        buttonSpacing = originalSpacing * dim / desiredDim;
    }
}
