using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(DebugManager))]
public class DebugManagerEditor : Editor
{
    DebugManager myDebugManager;

    void Awake()
    {
        myDebugManager = target as DebugManager;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space();

        if (GUILayout.Button("Kill All"))
        {
            myDebugManager.KillAll();
        }
    }
}
#endif