using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
    [Header("Objects")]
    public GameObject startSpot;
    public static GameObject Start
    {
        get { return instance.startSpot; }
    }
    public GameObject endSpot;
    public static GameObject End
    {
        get { return instance.endSpot; }
    }

    private PlayerCharacter playerCharacter;
    public static PlayerCharacter Player
    {
        get
        {
            if (instance.playerCharacter == null)
            {
                instance.playerCharacter = FindObjectOfType<PlayerCharacter>();
            }
            return instance.playerCharacter;
        }
    }

    private CameraController cameraController;
    public static CameraController Camera
    {
        get
        {
            if (instance.cameraController == null)
            {
                instance.cameraController = FindObjectOfType<CameraController>();
            }
            return instance.cameraController;
        }
    }

    private LevelManager levelManager;
    public static LevelManager Level
    {
        get
        {
            if (instance.levelManager == null)
            {
                instance.levelManager = FindObjectOfType<LevelManager>();
            }
            return instance.levelManager;
        }
    }

    private GestureManager gestureManager;
    public static GestureManager Gesture
    {
        get
        {
            if (instance.gestureManager == null)
            {
                instance.gestureManager = FindObjectOfType<GestureManager>();
            }
            return instance.gestureManager;
        }
    }

    private EffectManager effectManager;
    public static EffectManager Effect
    {
        get
        {
            if (instance.effectManager == null)
            {
                instance.effectManager = FindObjectOfType<EffectManager>();
            }
            return instance.effectManager;
        }
    }

    private TileRevealer tileRevealer;
    public static TileRevealer TileRevealer
    {
        get
        {
            if (instance.tileRevealer == null)
            {
                instance.tileRevealer = FindObjectOfType<TileRevealer>();
            }
            return instance.tileRevealer;
        }
    }

    private DisplayBarManager displayBarManager;
    public static DisplayBarManager Display
    {
        get
        {
            if (instance.displayBarManager == null)
            {
                instance.displayBarManager = FindObjectOfType<DisplayBarManager>();
            }
            return instance.displayBarManager;
        }
    }

    private static Managers instance;
    private void Awake()
    {
        instance = this;
    }
}
