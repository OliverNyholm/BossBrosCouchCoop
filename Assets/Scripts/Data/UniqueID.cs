using UnityEngine;
using UnityEditor;
using System.IO;

public class UniqueID : MonoBehaviour
{
    [SerializeField]
    private uint myID = uint.MaxValue;

#if UNITY_EDITOR 
    //ID to make sure the ID can't be changed from inspector.
    private uint myEditorID = uint.MaxValue;
    private bool myCanEditID = false;
#endif

    public uint GetID()
    {
        return myID;
    }

#if UNITY_EDITOR
    //Should not be called by other components. Only used for editor
    public void GenerateID()
    {
        myCanEditID = true;

        const string fileName = "highestUniqueID.txt";
        string fullPath = Application.dataPath + "/EditorData/" + fileName;
        StreamReader streamReader = new StreamReader(fullPath);
        if (uint.TryParse(streamReader.ReadToEnd(), out uint data))
        {
            myID = data + 1;
            myEditorID = myID;
        }
        else
        {
            Debug.LogError("Could read data in " + fileName + ". No unique ID generated.");
            streamReader.Close();
            return;
        }
        streamReader.Close();

        StreamWriter writer = new StreamWriter(fullPath, false);
        writer.Write(myID);
        writer.Close();

        myCanEditID = false;
    }
#endif

    private void OnValidate()
    {
#if UNITY_EDITOR
        if (!myCanEditID)
        {
            myID = myEditorID;
        }
        else
        {
            myEditorID = myID;
        }
#endif
    }
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
                myUniqueID.GenerateID();
            }

            EditorGUILayout.HelpBox("Component has not generated an actual unique ID yet!", MessageType.Warning);
        }
    }
}
#endif