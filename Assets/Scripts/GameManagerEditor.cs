using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor
{
    GameManager myGameManager;

    void Awake()
    {
        myGameManager = target as GameManager;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space();

        if (GUILayout.Button("Spawn another debug player"))
        {
            myGameManager.AddExtraDebugPlayer();
        }
    }
}