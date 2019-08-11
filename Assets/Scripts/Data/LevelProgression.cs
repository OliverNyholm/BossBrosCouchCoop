using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class LevelProgression : MonoBehaviour
{
    public int myLevelProgression;
    public List<int> myLevelsToUnlock = new List<int>();

    private static LevelProgression ourInstance;
    private void Awake()
    {
        DontDestroyOnLoad(this);
        if (ourInstance)
            Destroy(gameObject);
        else
            ourInstance = this;

        ReadBinaryFile();
    }

    public bool IsLevelAvailable(int aLevelIndex)
    {
        int bitValue = 1 << aLevelIndex;

        return (myLevelProgression & bitValue) == bitValue;
    }

    public void SetLevelProgression(int aLevelIndex, bool aIsAvailable)
    {
        int bitValue = 1 << aLevelIndex;

        myLevelProgression &= ~bitValue;
        myLevelProgression |= bitValue * (aIsAvailable ? 1 : 0);

        WriteBinaryFile();
        Debug.Log(myLevelProgression);
    }

    public void SetLevelsToUnlock(List<int> aLevelsToUnlock)
    {
        myLevelsToUnlock = new List<int>(aLevelsToUnlock);
    }

    public void UnlockLevels()
    {
        for (int index = 0; index < myLevelsToUnlock.Count; index++)
        {
            SetLevelProgression(myLevelsToUnlock[index], true);
        }
    }

    public void ResetProgression()
    {
        myLevelProgression = 1;
        WriteBinaryFile();
    }

    public void UnlockAllLevels()
    {
        myLevelProgression = int.MaxValue;
        WriteBinaryFile();
    }

    private void ReadBinaryFile()
    {
        Stream stream = new FileStream(Application.streamingAssetsPath + "/progression.bin", FileMode.Open);
        BinaryReader br = new BinaryReader(stream);
        myLevelProgression = br.ReadInt32();

        br.Close();
        stream.Close();
    }

    private void WriteBinaryFile()
    {
        string destination = Application.streamingAssetsPath + "/progression.bin";
        FileStream file;

        if (File.Exists(destination))
            file = File.OpenWrite(destination);
        else
            file = File.Create(destination);

        BinaryWriter binaryWriter = new BinaryWriter(file);
        binaryWriter.Write(myLevelProgression);

        binaryWriter.Close();
        file.Close();
    }
}

//[CustomEditor(typeof(LevelProgression))]
//public class LevelProgressionEditor : Editor
//{
//    int myLevelIndex = 1;
//    bool myIsAvailable = false;
//    LevelProgression myInstance;

//    void Awake()
//    {
//        myInstance = target as LevelProgression;
//    }

//    public override void OnInspectorGUI()
//    {

//        if (GUILayout.Button("Check availability"))
//        {
//            Debug.Log("Level availability = " + myInstance.IsLevelAvailable(myLevelIndex));
//        }
//        if (GUILayout.Button("Set Progression at Index"))
//        {
//            myInstance.SetLevelProgression(myLevelIndex, myIsAvailable);
//        }
//        myLevelIndex = EditorGUILayout.IntField("Level Index:", myLevelIndex);
//        myIsAvailable = EditorGUILayout.Toggle("Available:", myIsAvailable);

//        EditorGUILayout.Space();

//        DrawDefaultInspector();

//        if (GUILayout.Button("Unlock all levels"))
//        {
//            myInstance.UnlockAllLevels();
//        }
//        if (GUILayout.Button("Reset Progression"))
//        {
//            myInstance.ResetProgression();
//        }
//    }
//}
