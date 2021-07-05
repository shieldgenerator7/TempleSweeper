using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Updates the map line when tiles get revealed or when a new map segment gets found
/// </summary>
public class MapLineUpdater : MonoBehaviour
{
    public float speed = 1.0f;//the speed at which the map line grows when there's an update

    private MapLineGenerator currentMLG;
    private int targetCount = 0;
    private int currentCount = 0;//how many points have been fully revealed
    private Vector2 targetSize;//how long the segment should be when its fully drawn
    private bool okToMoveCamera = true;
    private bool okToMoveCameraEver = true;

    private List<SpriteRenderer> drawnLines = new List<SpriteRenderer>();

    public SpriteRenderer CurrentLineSR
    {
        get
        {
            if (drawnLines == null)
            {
                drawnLines = new List<SpriteRenderer>();
            }
            if (currentCount == 0 && drawnLines.Count == 0)
            {
                return null;
            }
            if (currentCount == targetCount)
            {
                return drawnLines[currentCount - 1];
            }
            return drawnLines[currentCount];
        }
    }

    public Vector2 LastRevealedSpot
    {
        get
        {
            return CurrentLineSR.transform.position + CurrentLineSR.transform.right * CurrentLineSR.size.x;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach (MapLineGenerator mlg in FindObjectsOfType<MapLineGenerator>())
        {
            mlg.onMapSegmentRevealed += checkReveal;
            mlg.onClearObjects += clearObjects;
        }
        drawnLines = new List<SpriteRenderer>();
        //hook up camera cancel automovement delegate
        Managers.Camera.onAutoMovementCanceled += stopMovingCamera;
    }

    private void OnEnable()
    {
        if (targetCount > 0)
        {
            if (drawnLines.Count < currentCount + 1)
            {
                createNewLine(drawnLines.Count);
            }
            okToMoveCamera = true;
            if (okToMoveCameraEver)
            {
                Managers.Camera.moveTo(drawnLines[currentCount].gameObject);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (targetCount > 0)
        {
            if (drawnLines.Count < currentCount + 1)
            {
                createNewLine(drawnLines.Count);
            }
            else if (CurrentLineSR.size.x < targetSize.x)
            {
                Vector2 size = CurrentLineSR.size;
                size.x = Mathf.MoveTowards(size.x, targetSize.x, speed * Time.deltaTime);
                Vector2 endPos = LastRevealedSpot;
                LevelTile lt = Managers.Level.getTile(endPos);
                if (!lt.Walkable || lt.Revealed)
                {
                    CurrentLineSR.size = size;
                    if (okToMoveCamera && okToMoveCameraEver)
                    {
                        Managers.Camera.moveTo(endPos);
                    }
                }
                else if (!lt.Revealed)
                {
                    okToMoveCamera = true;
                }
            }
            else if (currentCount < targetCount)
            {
                currentCount++;
            }
            if (currentCount == targetCount)
            {
                //Reveal the X
                if (currentCount == currentMLG.mapGenerator.amount)
                {
                    Managers.End.SetActive(true);
                }
                //Disable this updater
                this.enabled = false;
            }
        }
        else
        {
            this.enabled = false;
        }
    }

    public void checkReveal(MapLineGenerator mlg, int revealedCount)
    {
        currentMLG = mlg;
        targetCount = revealedCount;
        this.enabled = true;
    }

    public void createNewLine(int startIndex)
    {
        GameObject line = Instantiate(currentMLG.linePrefab);
        Vector2 startPos = currentMLG.getPosition(startIndex);
        Vector2 endPos = currentMLG.getPosition(startIndex + 1);
        line.transform.position = startPos;
        line.transform.right = (endPos - startPos);
        targetSize = new Vector2((startPos - endPos).magnitude, 1);
        SpriteRenderer lineSR = line.GetComponent<SpriteRenderer>();
        lineSR.size = new Vector2(0, 1);
        drawnLines.Add(lineSR);
    }

    public void clearObjects(MapLineGenerator mlg)
    {
        //Clear old path objects
        foreach (SpriteRenderer sr in drawnLines)
        {
            Destroy(sr.gameObject);
        }
        drawnLines.Clear();

        //Reset variables
        targetCount = 0;
        currentCount = 0;
        targetSize = new Vector2(0, 1);
    }

    public void stopMovingCamera()
    {
        okToMoveCamera = false;
        okToMoveCameraEver = false;
    }
}
