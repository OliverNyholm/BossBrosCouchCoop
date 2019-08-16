using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(LevelProgression))]
public class LevelProgressionEditor : Editor
{
    int myLevelIndex = 1;
    bool myIsAvailable = false;
    LevelProgression myInstance;

    void Awake()
    {
        myInstance = target as LevelProgression;
    }

    public override void OnInspectorGUI()
    {

        if (GUILayout.Button("Check availability"))
        {
            Debug.Log("Level availability = " + myInstance.IsLevelAvailable(myLevelIndex));
        }
        if (GUILayout.Button("Set Progression at Index"))
        {
            myInstance.SetLevelProgression(myLevelIndex, myIsAvailable);
        }
        myLevelIndex = EditorGUILayout.IntField("Level Index:", myLevelIndex);
        myIsAvailable = EditorGUILayout.Toggle("Available:", myIsAvailable);

        EditorGUILayout.Space();

        DrawDefaultInspector();

        if (GUILayout.Button("Unlock all levels"))
        {
            myInstance.UnlockAllLevels();
        }
        if (GUILayout.Button("Reset Progression"))
        {
            myInstance.ResetProgression();
        }
    }
}
#endif