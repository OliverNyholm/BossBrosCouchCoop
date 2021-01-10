using UnityEngine;
using UnityEditor;
using System.IO;

#if UNITY_EDITOR
using UnityEditor.SceneManagement;

[CustomEditor(typeof(UniqueID))]
public class UniqueIDEditor : Editor
{
    UniqueID myUniqueID;

    void Awake()
    {
        myUniqueID = target as UniqueID;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space();


        if (GUILayout.Button("Generate Unique ID"))
        {

            SerializedProperty id = serializedObject.FindProperty("myID");
            id.intValue = (int)myUniqueID.GenerateID();
            serializedObject.ApplyModifiedProperties();

            EditorUtility.SetDirty(myUniqueID);
            EditorSceneManager.MarkSceneDirty(myUniqueID.gameObject.scene);
        }

        if (myUniqueID.GetID() == uint.MaxValue)
        {
            EditorGUILayout.HelpBox("Component has not generated an actual unique ID yet!", MessageType.Warning);
        }
    }
}
#endif