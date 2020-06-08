using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TutorialHighlightManager : MonoBehaviour
{
    public List<TutorialHighlightCircle> myHighlightCircles;

    public void HighlightArea(Vector3 aLocation, Vector3 aTargetScale)
    {
        for (int index = 0; index < myHighlightCircles.Count; index++)
        {
            if (myHighlightCircles[index].myIsRunning)
                continue;

            myHighlightCircles[index].HighlightArea(aLocation, aTargetScale);
            break;
        }
    }
}

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