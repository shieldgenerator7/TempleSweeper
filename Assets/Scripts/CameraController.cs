using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{//2018-01-22: copied from Stonicorn.CameraController

    public float zoomSpeed = 0.5f;//how long it takes to fully change to a new zoom level
    public float firstScale = 3.0f;
    [Range(0, 1)]
    public float autoMoveThreshold = 0.2f;//what percentage of half the screen a tap needs to be in to auto-move the screen
    public float moveSpeed = 3;//how fast it moves when automoving
    public float autoMoveLockThreshold = 0.5f;//how far the camera has to be from its target to lock gesture control (implemented in GestureProfile)

    public CanvasScaler canvasScaler;
    private float scale = 1;//scale used to determine orthographicSize, independent of (landscape or portrait) orientation
    private Camera cam;
    private float zoomStartTime = 0.0f;//when the zoom last started
    private float startZoomScale;//the orthographicsize at the start and end of a zoom
    private Vector3 targetPosition;

    private int pixelWidth;
    private int pixelHeight;

    struct ScalePoint
    {
        private float scalePoint;
        private bool relative;//true if relative to player's range, false if absolute
        public ScalePoint(float scale, bool relative)
        {
            scalePoint = scale;
            this.relative = relative;
        }
        public float absoluteScalePoint()
        {
            if (relative)
            {
                return scalePoint * 3;
            }
            return scalePoint;
        }
    }
    List<ScalePoint> scalePoints = new List<ScalePoint>();
    int scalePointIndex = 0;//the index of the current scalePoint in scalePoints
    public static int SCALEPOINT_DEFAULT = 2;//the index of the default scalepoint

    // Use this for initialization
    void Start()
    {
        cam = GetComponent<Camera>();
        pinpoint();
        scale = cam.orthographicSize;
        //Initialize ScalePoints
        scalePoints.Add(new ScalePoint(1, false));
        scalePoints.Add(new ScalePoint(1, true));
        scalePoints.Add(new ScalePoint(2, true));
        scalePoints.Add(new ScalePoint(4, true));
        //Set the initialize scale point
        setScalePoint(1);
        scale = scalePoints[scalePointIndex].absoluteScalePoint();
    }

    void Update()
    {
        checkScreenSize();
        if (zoomStartTime != 0)
        {
            zoomToScalePoint();
        }
        if (transform.position != targetPosition)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }
    }

    public void zoomToScalePoint()
    {
        float absSP = scalePoints[scalePointIndex].absoluteScalePoint();
        scale = Mathf.Lerp(
            startZoomScale,
            absSP,
            (Time.time - zoomStartTime) / zoomSpeed);
        updateOrthographicSize();
        if (scale == absSP)
        {
            zoomStartTime = startZoomScale = 0.0f;
        }
    }

    public void setScalePoint(int scalePointIndex)
    {
        //Start the zoom-over-time process
        if (startZoomScale == 0)
        {
            startZoomScale = scalePoints[this.scalePointIndex].absoluteScalePoint();
        }
        else
        {
            startZoomScale = scale;
        }
        zoomStartTime = Time.time;
        //Set the new scale point index
        if (scalePointIndex < 0)
        {
            scalePointIndex = 0;
        }
        else if (scalePointIndex > scalePoints.Count - 1)
        {
            scalePointIndex = scalePoints.Count - 1;
        }
        this.scalePointIndex = scalePointIndex;
    }
    public void adjustScalePoint(int addend)
    {
        setScalePoint(scalePointIndex + addend);
    }
    public int getScalePointIndex()
    {
        return scalePointIndex;
    }
    public void updateOrthographicSize()
    {
        scale = firstScale;
        if (Screen.height > Screen.width)//portrait orientation
        {
            cam.orthographicSize = (scale * cam.pixelHeight) / cam.pixelWidth;
        }
        else
        {//landscape orientation
            cam.orthographicSize = scale;
        }
    }

    /// <summary>
    /// Tells the camera to stay in its new position
    /// </summary>
    public void pinpoint()
    {
        targetPosition = transform.position;
        onAutoMovementCanceled?.Invoke();
    }

    public delegate void OnAutoMovementCanceled();
    public OnAutoMovementCanceled onAutoMovementCanceled;

    /// <summary>
    /// Moves the camera to the given <see langword="GameObject"/>'s position
    /// </summary>
    /// <param name="go"></param>
    public void moveTo(GameObject go)
    {
        targetPosition = go.transform.position;
        targetPosition.z = transform.position.z;
    }

    public void moveTo(Vector2 pos)
    {
        targetPosition = pos;
        targetPosition.z = transform.position.z;
    }

    public bool AutoMoving
    {
        get { return (targetPosition - transform.position).sqrMagnitude > autoMoveLockThreshold * autoMoveLockThreshold; }
    }

    public void checkForAutomovement(Vector3 worldPos)
    {
        Vector2 screenPos = cam.WorldToScreenPoint(worldPos);
        //Make the threshold an even thickness all the way around
        float threshold = Mathf.Min(Screen.width * autoMoveThreshold, Screen.height * autoMoveThreshold);
        //Convert the position to the same corner
        if (screenPos.x > Screen.width / 2)
        {
            screenPos.x -= (screenPos.x - Screen.width / 2) * 2;
        }
        if (screenPos.y > Screen.height / 2)
        {
            screenPos.y -= (screenPos.y - Screen.height / 2) * 2;
        }
        //If the position is on the edge of the screen,
        if (screenPos.x < threshold || screenPos.y < threshold)
        {
            //Auto-move the camera
            worldPos.z = transform.position.z;
            targetPosition = worldPos;
            //Then Update() will update the camera's position each frame
            //for a smooth move effect
        }
    }

    /// <summary>
    /// Returns whether or not the given position is in the camera's view
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public bool inView(Vector2 position)
    {
        //2017-10-31: copied from an answer by Taylor-Libonati: http://answers.unity3d.com/questions/720447/if-game-object-is-in-cameras-field-of-view.html
        Vector3 screenPoint = cam.WorldToViewportPoint(position);
        return screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
    }

    void checkScreenSize()
    {
        int spw = Camera.main.scaledPixelWidth;
        int sph = Camera.main.scaledPixelHeight;
        if (pixelWidth != spw || pixelHeight != sph)
        {
            pixelWidth = spw;
            pixelHeight = sph;
            updateOrthographicSize();
            canvasScaler.matchWidthOrHeight = (pixelWidth > pixelHeight) ? 0 : 1;
            Managers.Display.updateScreenConstants(pixelWidth, pixelHeight);
            foreach (DisplayBar db in FindObjectsOfType<DisplayBar>())
            {
                for (int i = 0; i < 2; i++)
                {
                    db.reupdate();
                }
            }
            onScreenSizeChanged?.Invoke(pixelWidth, pixelHeight);
        }
    }
    public delegate void OnScreenSizeChanged(int width, int height);
    public event OnScreenSizeChanged onScreenSizeChanged;
}
