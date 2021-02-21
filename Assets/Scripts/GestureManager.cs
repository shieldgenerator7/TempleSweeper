using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GestureManager : MonoBehaviour
{//2018-01-22: copied from Stonicorn.GestureManager

    //Settings
    public float dragThreshold = 50;//how far from the original mouse position the current position has to be to count as a drag
    public float holdThreshold = 0.1f;//how long the tap has to be held to count as a hold (in seconds)
    public float orthoZoomSpeed = 0.5f;

    //Gesture Profiles
    private GestureProfile currentGP;//the current gesture profile
    private Dictionary<string, GestureProfile> gestureProfiles = new Dictionary<string, GestureProfile>();//dict of valid gesture profiles

    //Gesture Event Methods
    public TapGesture tapGesture;

    //Original Positions
    private Vector3 origMP;//"original mouse position": the mouse position at the last mouse down (or tap down) event
    private Vector3 origMP2;//second orginal "mouse position" for second touch
    private Vector3 origCP;//"original camera position": the camera offset (relative to the player) at the last mouse down (or tap down) event
    private float origTime = 0f;//"original time": the clock time at the last mouse down (or tap down) event
    private int origScalePoint;//the original scale point of the camera
    //Current Positions
    private Vector3 curMP;//"current mouse position"
    private Vector3 curMP2;//"current mouse position" for second touch
    private Vector3 curMPWorld;//"current mouse position world" - the mouse coordinates in the world
    private float curTime = 0f;
    //Stats
    private int touchCount = 0;//how many touches to process, usually only 0 or 1, only 2 if zoom
    private float maxMouseMovement = 0f;//how far the mouse has moved since the last mouse down (or tap down) event
    private float holdTime = 0f;//how long the gesture has been held for
    private enum ClickState { Began, InProgress, Ended, None };
    private ClickState clickState = ClickState.None;
    //
    public int tapCount = 0;//how many taps have ever been made, including tap+holds that were sent back as taps
    //Flags
    public bool cameraDragInProgress = false;
    private bool isDrag = false;
    private bool isTapGesture = true;
    private bool isHoldGesture = false;
    public const float holdTimeScale = 0.5f;//how fast time moves during a hold gesture (1 = normal, 0.5 = half speed, 2 = double speed)
    public const float holdTimeScaleRecip = 1 / holdTimeScale;
    public bool isRightClick = false;


    // Use this for initialization
    void Start()
    {
        gestureProfiles.Add("Main", new GestureProfile());
        currentGP = gestureProfiles["Main"];

        Input.simulateMouseWithTouches = false;
    }

    // Update is called once per frame
    void Update()
    {
        //
        //Threshold updating
        //
        float newDT = Mathf.Min(Screen.width, Screen.height) / 20;
        if (dragThreshold != newDT)
        {
            dragThreshold = newDT;
        }
        //
        //Input scouting
        //
        if (Input.touchCount > 2)
        {
            touchCount = 0;
        }
        else if (Input.touchCount == 2)
        {
            touchCount = 2;
            if (Input.GetTouch(1).phase == TouchPhase.Began)
            {
                clickState = ClickState.Began;
                origMP2 = Input.GetTouch(1).position;
                origScalePoint = Managers.Camera.getScalePointIndex();
            }
            else if (Input.GetTouch(1).phase == TouchPhase.Ended)
            {
            }
            else
            {
                clickState = ClickState.InProgress;
                curMP2 = Input.GetTouch(1).position;
            }
        }
        else if (Input.touchCount == 1)
        {
            touchCount = 1;
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                clickState = ClickState.Began;
                origMP = Input.GetTouch(0).position;
            }
            else if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                clickState = ClickState.Ended;
            }
            else
            {
                clickState = ClickState.InProgress;
                curMP = Input.GetTouch(0).position;
            }
        }
        else if (Input.GetMouseButton(0))
        {
            touchCount = 1;
            if (Input.GetMouseButtonDown(0))
            {
                clickState = ClickState.Began;
                origMP = Input.mousePosition;
                isRightClick = false;
            }
            else
            {
                clickState = ClickState.InProgress;
                curMP = Input.mousePosition;
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            clickState = ClickState.Ended;
        }
        else if (Input.GetMouseButton(1))
        {
            touchCount = 1;
            if (Input.GetMouseButtonDown(1))
            {
                clickState = ClickState.Began;
                origMP = Input.mousePosition;
                isRightClick = true;
            }
            else
            {
                clickState = ClickState.InProgress;
                curMP = Input.mousePosition;
            }
        }
        else if (Input.GetMouseButtonUp(1))
        {
            clickState = ClickState.Ended;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            clickState = ClickState.InProgress;
        }
        else if (Input.touchCount == 0
            && !Input.GetMouseButton(0)
            && !Input.GetMouseButton(1))
        {
            touchCount = 0;
            clickState = ClickState.None;
        }

        //
        //Preliminary Processing
        //Stats are processed here
        //
        switch (clickState)
        {
            case ClickState.Began:
                if (touchCount < 2)
                {
                    curMP = origMP;
                    maxMouseMovement = 0;
                    origCP = Camera.main.transform.position;
                    origTime = Time.time;
                    curTime = origTime;
                }
                else if (touchCount == 2)
                {
                    curMP2 = origMP2;
                }
                break;
            case ClickState.Ended: //do the same thing you would for "in progress"
            case ClickState.InProgress:
                float mm = Vector3.Distance(curMP, origMP);
                if (mm > maxMouseMovement)
                {
                    maxMouseMovement = mm;
                }
                curTime = Time.time;
                holdTime = curTime - origTime;
                break;
            case ClickState.None: break;
            default:
                throw new System.Exception("Click State of wrong type, or type not processed! (Stat Processing) clickState: " + clickState);
        }
        curMPWorld = (Vector2)Camera.main.ScreenToWorldPoint(curMP);//cast to Vector2 to force z to 0

        if (Input.touchCount == 0 && Input.mousePresent)
        {
            currentGP.processCursorMoveGesture((Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition), true);
        }
        else
        {
            currentGP.processCursorMoveGesture(Vector3.zero, false);
        }

        //
        //Input Processing
        //
        if (touchCount == 1)
        {
            if (clickState == ClickState.Began)
            {
                if (touchCount < 2)
                {
                    //Set all flags = true
                    cameraDragInProgress = false;
                    isDrag = false;
                    isTapGesture = true;
                    isHoldGesture = false;
                }
            }
            else if (clickState == ClickState.InProgress)
            {
                if (maxMouseMovement > dragThreshold)
                {
                    if (!isHoldGesture)
                    {
                        isTapGesture = false;
                        isDrag = true;
                        cameraDragInProgress = true;
                    }
                }
                if (holdTime > holdThreshold
                    || isRightClick)
                {
                    if (!isDrag)
                    {
                        isTapGesture = false;
                        isHoldGesture = true;
                        Time.timeScale = holdTimeScale;
                    }
                }
                if (isDrag)
                {
                    //Check to make sure Merky doesn't get dragged off camera
                    Vector3 delta = Camera.main.ScreenToWorldPoint(origMP) - Camera.main.ScreenToWorldPoint(curMP);
                    Vector3 newPos = origCP + delta;
                    //Move the camera
                    Camera.main.transform.position = newPos;
                    Managers.Camera.pinpoint();
                }
                else if (isHoldGesture)
                {
                    currentGP.processHoldGesture(curMPWorld, holdTime, false);
                }
            }
            else if (clickState == ClickState.Ended)
            {
                if (isDrag)
                {
                    Managers.Camera.pinpoint();
                }
                else if (isHoldGesture)
                {
                    currentGP.processHoldGesture(curMPWorld, holdTime, true);
                }
                else if (isTapGesture)
                {
                    tapCount++;
                    currentGP.processTapGesture(curMPWorld);
                    tapGesture?.Invoke();
                }

                //Set all flags = false
                cameraDragInProgress = false;
                isDrag = false;
                isTapGesture = false;
                isHoldGesture = false;
                Time.timeScale = 1;
            }
            else
            {
                throw new System.Exception("Click State of wrong type, or type not processed! (Input Processing) clickState: " + clickState);
            }

        }
        else
        {//touchCount == 0 || touchCount >= 2
            if (clickState == ClickState.Began)
            {
            }
            else if (clickState == ClickState.InProgress)
            {
                //
                //Zoom Processing
                //
                //
                //Mouse Scrolling Zoom
                //
                if (Input.GetAxis("Mouse ScrollWheel") < 0)
                {
                    currentGP.processPinchGesture(1);
                }
                else if (Input.GetAxis("Mouse ScrollWheel") > 0)
                {
                    currentGP.processPinchGesture(-1);
                }
                //
                //Pinch Touch Zoom
                //2015-12-31 (1:23am): copied from https://unity3d.com/learn/tutorials/modules/beginner/platform-specific/pinch-zoom
                //

                // If there are two touches on the device...
                if (touchCount == 2)
                {
                    // Store both touches.
                    Touch touchZero = Input.GetTouch(0);
                    Touch touchOne = Input.GetTouch(1);

                    // Find the position in the previous frame of each touch.
                    Vector2 touchZeroPrevPos = origMP;
                    Vector2 touchOnePrevPos = origMP2;

                    // Find the magnitude of the vector (the distance) between the touches in each frame.
                    float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                    float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

                    // Find the difference in the distances between each frame.
                    int deltaMagnitudeQuo = (int)System.Math.Truncate(Mathf.Max(prevTouchDeltaMag, touchDeltaMag) / Mathf.Min(prevTouchDeltaMag, touchDeltaMag));
                    deltaMagnitudeQuo *= (int)Mathf.Sign(prevTouchDeltaMag - touchDeltaMag);

                    //Update the camera's scale point index
                    currentGP.processPinchGesture(origScalePoint + deltaMagnitudeQuo - Managers.Camera.getScalePointIndex());
                }
            }
            else if (clickState == ClickState.Ended)
            {
                origScalePoint = Managers.Camera.getScalePointIndex();
            }
        }

        //
        //Application closing
        //
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
    /// <summary>
    /// Switches the gesture profile to the profile with the given name
    /// </summary>
    /// <param name="gpName">The name of the GestureProfile</param>
    public void switchGestureProfile(string gpName)
    {
        //Deactivate current
        currentGP.deactivate();
        //Switch from current to new
        currentGP = gestureProfiles[gpName];
        //Activate new
        currentGP.activate();
    }

    /// <summary>
    /// Gets called when a tap gesture is processed
    /// </summary>
    public delegate void TapGesture();
}