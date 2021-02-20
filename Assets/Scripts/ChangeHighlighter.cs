using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeHighlighter : MonoBehaviour
{
    public Color neutralColor = Color.white;
    public Color discoverColor = Color.yellow;
    public Color warnColor = new Color(255, 106, 0);
    public Color hitColor = Color.red;

    public float duration = 1;
    public float maxScale = 2;

    public enum ChangeType
    {
        NEUTRAL,//reveal non-trap
        DISCOVER,//reveal treasure or pick up a map
        WARN,//flag a tile
        HIT//reveal trap
    }

    public bool Running => gameObject.activeSelf;

    private float startTime = 0;
    private SpriteRenderer sr;

    // Start is called before the first frame update
    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void OnEnable()
    {
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        Color color = sr.color;
        color.a = 1 - ((Time.time - startTime) / duration);
        sr.color = color;
        transform.localScale = Vector3.one *
            (maxScale * (Time.time - startTime) / duration);
        if (Time.time > startTime + duration)
        {
            gameObject.SetActive(false);
        }
    }

    public void startEffect(Vector2 pos, ChangeType changeType)
    {
        Start();
        transform.position = pos;
        Color color;
        switch (changeType)
        {
            case ChangeType.NEUTRAL: color = neutralColor; break;
            case ChangeType.DISCOVER: color = discoverColor; break;
            case ChangeType.WARN: color = warnColor; break;
            case ChangeType.HIT: color = hitColor; break;
            default: throw new System.NotImplementedException();
        }
        color.a = 1;
        sr.color = color;
        transform.localScale = Vector3.one;
        gameObject.SetActive(true);
    }
}
