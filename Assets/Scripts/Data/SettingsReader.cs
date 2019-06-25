using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SettingsReader : MonoBehaviour
{
    [System.Serializable]
    private class PlayerSettings
    {
        float MasterVolume;
    }

    private void Awake()
    {
    }

    private void ReadJson()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "Settings.json");
        if(File.Exists(filePath))
        {
            string dataAsJson = File.ReadAllText(filePath);
            PlayerSettings settings = JsonUtility.FromJson<PlayerSettings>(dataAsJson);
        }
    }
}
