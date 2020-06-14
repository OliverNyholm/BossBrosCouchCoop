using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using InControl;

[Serializable]
public class PlayerSelectData
{
    public PlayerControls myPlayerControls;
    public ClassData myClassData;
    public ColorScheme myColorScheme;
    public string myName;

    public PlayerSelectData(PlayerControls aPlayerControls, ClassData aClassData, ColorScheme aColorScheme, string aName)
    {
        myPlayerControls = aPlayerControls;
        myClassData = aClassData;
        myColorScheme = aColorScheme;
        myName = aName;
    }
}

public class CharacterGameData : MonoBehaviour
{
    private List<PlayerSelectData> mySelectedCharacters;

    public int myCurrentLevelIndex;
    public string mySceneToLoad;

    private static CharacterGameData ourCharacterGameDataInstance;
    private void Awake()
    {
        DontDestroyOnLoad(this);
        if (ourCharacterGameDataInstance)
            Destroy(gameObject);
        else
            ourCharacterGameDataInstance = this;

        mySelectedCharacters = new List<PlayerSelectData>();
    }

    public void AddPlayerData(PlayerControls aPlayerControls, string aName)
    {
        mySelectedCharacters.Add(new PlayerSelectData(aPlayerControls, null, null, aName));
    }

    public void AddCharacterData(PlayerControls aPlayerControls, ClassData aClassData, ColorScheme aColorScheme)
    {
        for (int index = 0; index < mySelectedCharacters.Count; index++)
        {
            if (mySelectedCharacters[index].myPlayerControls.Device == aPlayerControls.Device)
            {
                mySelectedCharacters[index].myClassData = aClassData;
                mySelectedCharacters[index].myColorScheme = aColorScheme;
                break;
            }
        }
    }

    public bool RemovePlayerData(PlayerControls aPlayerControls)
    {
        for (int index = 0; index < mySelectedCharacters.Count; index++)
        {
            if(mySelectedCharacters[index].myPlayerControls == aPlayerControls)
            {
                mySelectedCharacters.RemoveAt(index);
                return true;
            }
        }

        return false;
    }

    public void GiveClassAndColorsToPlayers(ClassData aClassData, List<ColorScheme> someColorSchemes)
    {
        for (int index = 0; index < mySelectedCharacters.Count; index++)
        {
            mySelectedCharacters[index].myClassData = aClassData;
            mySelectedCharacters[index].myColorScheme = someColorSchemes[index];
        }
    }

    public List<PlayerSelectData> GetPlayerData()
    {
        return mySelectedCharacters;
    }          

    public void ClearPlayerData()
    {
        mySelectedCharacters.Clear();
    }
}
