using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDisplayer : MonoBehaviour
{
    public float displayScale = 3;
    public float scaleIncreaseDuration = 1;//seconds

    private Vector3 originalSize;
    private float currentScale = 1;
    private float scaleIncreaseRate;
    private float scalingStartTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        originalSize = transform.localScale;
        currentScale = 1;
        scaleIncreaseRate = displayScale / scaleIncreaseDuration;
        scalingStartTime = Time.time;
        //Make it show above everything else
        GetComponent<SpriteRenderer>().sortingOrder = 10;
        //Register with Level Manager
        LevelManager.FoundItem = this;
    }

    // Update is called once per frame
    void Update()
    {
        currentScale = Mathf.Lerp(
            1,
            displayScale,
            Mathf.Min(1, (Time.time - scalingStartTime) / scaleIncreaseDuration)
            );
        transform.localScale = originalSize * currentScale;
    }
}
