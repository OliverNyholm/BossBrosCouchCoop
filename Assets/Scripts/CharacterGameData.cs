using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public struct CharacterSelectData
{
    public InputDevice myInputDevice;
    public ClassData myClassData;
    public ColorScheme myColorScheme;

    public CharacterSelectData(InputDevice aInputDevice, ClassData aClassData, ColorScheme aColorScheme)
    {
        myInputDevice = aInputDevice;
        myClassData = aClassData;
        myColorScheme = aColorScheme;
    }
}

public class CharacterGameData : MonoBehaviour
{
    private List<CharacterSelectData> mySelectedCharacters;

    private void Awake()
    {
        DontDestroyOnLoad(this);

        mySelectedCharacters = new List<CharacterSelectData>();
    }

    public void AddPlayerData(InputDevice aInputDevice, ClassData aClassData, ColorScheme aColorScheme)
    {
        mySelectedCharacters.Add(new CharacterSelectData(aInputDevice, aClassData, aColorScheme));
    }

    public bool RemovePlayerData(InputDevice aInputDevice)
    {
        for (int index = 0; index < mySelectedCharacters.Count; index++)
        {
            if(mySelectedCharacters[index].myInputDevice == aInputDevice)
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
