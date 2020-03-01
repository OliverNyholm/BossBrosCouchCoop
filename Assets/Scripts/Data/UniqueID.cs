using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;

[DisallowMultipleComponent]
public class UniqueID : MonoBehaviour
{
    [SerializeField]
    private uint myID = uint.MaxValue;

    public uint GetID()
    {
        return myID;
    }

#if UNITY_EDITOR
    //Should not be called by other components. Only used for editor
    public uint GenerateID()
    {
        uint returnValue = uint.MaxValue;

        const string fileName = "highestUniqueID.txt";
        string fullPath = Application.dataPath + "/EditorData/" + fileName;
        StreamReader streamReader = new StreamReader(fullPath);
        if (uint.TryParse(streamReader.ReadToEnd(), out uint data))
        {
            returnValue = data + 1;
        }
        else
        {
            Debug.LogError("Could read data in " + fileName + ". No unique ID generated.");
            streamReader.Close();
            return returnValue;
        }
        streamReader.Close();

        StreamWriter writer = new StreamWriter(fullPath, false);
        writer.Write(returnValue);
        writer.Close();

        return returnValue;
    }
#endif
}

#if UNITY_EDITOR
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

        if (myUniqueID.GetID() == uint.MaxValue)
        {
            if (GUILayout.Button("Generate Unique ID"))
            {                

                SerializedProperty id = serializedObject.FindProperty("myID");
                id.intValue = (int)myUniqueID.GenerateID();
                serializedObject.ApplyModifiedProperties();

                EditorUtility.SetDirty(myUniqueID);
                EditorSceneManager.MarkSceneDirty(myUniqueID.gameObject.scene);
            }

            EditorGUILayout.HelpBox("Component has not generated an actual unique ID yet!", MessageType.Warning);
        }
    }
}
#endif