using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

#if UNITY_EDITOR
[CustomEditor(typeof(TutorialHighlightManager))]
public class TutorialHighlightManagerEditor : Editor
{
    TutorialHighlightManager myManager;

    void Awake()
    {
        myManager = target as TutorialHighlightManager;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space();

        if (GUILayout.Button("Test"))
        {
            myManager.HighlightArea(new Vector3(-300, 0), Vector3.one);
        }
    }
}
#endif