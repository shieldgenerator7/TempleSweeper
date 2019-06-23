using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DebugHelper))]
public class DebugHelperEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        DebugHelper dh = ((DebugHelper)target);
        if (GUILayout.Button("Highlight Tiles"))
        {
            dh.highlightAll();
        }
        if (GUILayout.Button("Reveal Tiles"))
        {
            dh.revealAll();
        }
    }
}
