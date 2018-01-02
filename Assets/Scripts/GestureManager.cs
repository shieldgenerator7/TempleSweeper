using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestureManager : MonoBehaviour {

    public LevelManager lm;
    public Camera cam;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0))
        {
            lm.processTapGesture(cam.ScreenToWorldPoint(Input.mousePosition));
        }
        else if (Input.GetMouseButtonDown(1))
        {
            lm.processFlagGesture(cam.ScreenToWorldPoint(Input.mousePosition));
        }
	}
}
