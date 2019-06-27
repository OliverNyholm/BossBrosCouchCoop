using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public struct CharacterSelectData
{
    public PlayerControls myPlayerControls;
    public ClassData myClassData;
    public ColorScheme myColorScheme;
    public string myName;

    public CharacterSelectData(PlayerControls aPlayerControls, ClassData aClassData, ColorScheme aColorScheme, string aName)
    {
        myPlayerControls = aPlayerControls;
        myClassData = aClassData;
        myColorScheme = aColorScheme;
        myName = aName;
    }
}

public class CharacterGameData : MonoBehaviour
{
    private List<CharacterSelectData> mySelectedCharacters;

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

        mySelectedCharacters = new List<CharacterSelectData>();
    }

    public void AddPlayerData(PlayerControls aPlayerControls, ClassData aClassData, ColorScheme aColorScheme, string aName)
    {
        mySelectedCharacters.Add(new CharacterSelectData(aPlayerControls, aClassData, aColorScheme, aName));
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

    public List<CharacterSelectData> GetPlayerData()
    {
        return mySelectedCharacters;
    }          

    public void ClearPlayerData()
    {
        mySelectedCharacters.Clear();
    }
}
