using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [SerializeField]
    private int time = 0;
    public int Time
    {
        get { return time; }
        private set {
            time = value;
            onTimePassed?.Invoke(time);
        }
    }

    public void moveForward()
    {
        Time++;
    }

    public delegate void OnTimePassed(int currentTime);
    public OnTimePassed onTimePassed;

}
