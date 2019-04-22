using UnityEngine;
using System.Collections;

public class GestureProfile
{//2018-01-22: copied from Stonicorn.GestureProfile

    protected Camera cam;
    protected CameraController cmaController;
    protected GestureManager gestureManager;
    protected LevelManager levelManager;

    public GestureProfile()
    {
        cam = Camera.main;
        cmaController = cam.GetComponent<CameraController>();
        gestureManager = GameObject.FindObjectOfType<GestureManager>();
        levelManager = GameObject.FindObjectOfType<LevelManager>();
    }
    /// <summary>
    /// Called when this profile is set to the current one
    /// </summary>
    public virtual void activate() { }
    /// <summary>
    /// Called when the GestureManager switches off this profile to a different one
    /// </summary>
    public virtual void deactivate() { }

    public virtual void processTapGesture(Vector3 curMPWorld)
    {
        if (cmaController.AutoMoving)
        {
            cmaController.pinpoint();
            return;
        }
        levelManager.processTapGesture(curMPWorld);
        cmaController.checkForAutomovement(curMPWorld);
    }
    public virtual void processHoldGesture(Vector3 curMPWorld, float holdTime, bool finished)
    {
        levelManager.processHoldGesture(curMPWorld, finished);
        if (finished)
        {
            cmaController.checkForAutomovement(curMPWorld);
        }
    }
    public void processDragGesture()
    {

    }
    public virtual void processPinchGesture(int adjustment)
    {
        cmaController.adjustScalePoint(adjustment);
    }
}
