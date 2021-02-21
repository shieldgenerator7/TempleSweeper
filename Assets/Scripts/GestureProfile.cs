using UnityEngine;
using System.Collections;

public class GestureProfile
{//2018-01-22: copied from Stonicorn.GestureProfile

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
        if (Managers.Camera.AutoMoving)
        {
            Managers.Camera.pinpoint();
            return;
        }
        Managers.Level.processTapGesture(curMPWorld);
        Managers.Camera.checkForAutomovement(curMPWorld);
    }
    public virtual void processHoldGesture(Vector3 curMPWorld, float holdTime, bool finished)
    {
        if (Managers.Camera.AutoMoving)
        {
            Managers.Camera.pinpoint();
            return;
        }
        Managers.Level.processHoldGesture(curMPWorld, finished);
        if (finished)
        {
            Managers.Camera.checkForAutomovement(curMPWorld);
        }
    }
    public void processDragGesture()
    {

    }
    public virtual void processPinchGesture(int adjustment)
    {
        Managers.Camera.adjustScalePoint(adjustment);
    }
    public virtual void processCursorMoveGesture(Vector3 curMPWorld, bool show)
    {
        Managers.Effect.hideCursor();
        if (show)
        {
            LevelTile lt = LevelManager.getTile(curMPWorld);
            if (!lt)
            {
                return;
            }
            if (!lt.Revealed || lt.DetectedAny
                || lt == Managers.Level.StartTile || (Managers.Player.completedMap() && lt == Managers.Level.XTile)
                || (lt.tileType == LevelTile.TileType.MAP && !lt.Activated)
                )
            {
                Managers.Effect.moveCursor(lt);
            }
        }
    }
}
