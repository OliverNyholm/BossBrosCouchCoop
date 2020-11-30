using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class OptionsConfig : MonoBehaviour
{
    private static OptionsConfig myInstance;
    private static bool myIsApplicationQuitting = false;

    public delegate void OptionsConfigChanged(OptionsConfig aOptionsConfig);
    public event OptionsConfigChanged EventOnOptionsChanged;

    [Serializable]
    public class OptionsData
    {
        public HealTargetingOption myHealTargetingMode = HealTargetingOption.SelectWithLeftStickOnly;
    }

    public OptionsData myOptionsData;

    public static OptionsConfig Instance
    {
        get
        {
            if (myIsApplicationQuitting)
            {
                return null;
            }

            if (myInstance == null)
            {
                GameObject gameObject = new GameObject();
                gameObject.name = "OptionsConfig Object";
                DontDestroyOnLoad(gameObject);

                myInstance = gameObject.AddComponent<OptionsConfig>();
                myInstance.ParseOptions();
            }
            return myInstance;
        }
    }

    private void Awake()
    {
        if (myInstance != null)
        {
            Debug.LogWarning("Second instance of MonoSingleton created. Automatic self-destruct triggered.");
            Destroy(gameObject);
        }
        else
        {
            myInstance = GetComponent<OptionsConfig>();
            myInstance.ParseOptions();
            DontDestroyOnLoad(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (myInstance == this)
        {
            myInstance = null;
        }
        myIsApplicationQuitting = true;
    }

    public void ParseOptions()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "Options.json");
        if (File.Exists(filePath))
        {
            StreamReader file = new StreamReader(filePath);

            string optionsData = file.ReadToEnd();
            myOptionsData = JsonUtility.FromJson<OptionsData>(optionsData);

            file.Close();
        }
        else
        {
            myOptionsData = new OptionsData();
            SaveOptions();
        }
    }

    public void SaveOptions()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "Options.json");
        StreamWriter file = new StreamWriter(filePath);

        string returnString = JsonUtility.ToJson(myOptionsData, true);
        file.WriteLine(returnString);

        file.Close();
    }
    
    public void InvokeOptionsChanged()
    {
        EventOnOptionsChanged?.Invoke(this);
    }
}

public enum HealTargetingOption : short
{
    NoHealTargeting,
    SelectWithLeftStickOnly,
    SelectWithLeftStickAndAutoHeal,
    SelectWithLookDirection,
    SelectWithRightStickOrKeyboard,
    Count
}
