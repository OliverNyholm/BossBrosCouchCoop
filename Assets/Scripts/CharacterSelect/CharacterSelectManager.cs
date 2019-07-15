﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
using UnityEngine.SceneManagement;

public class CharacterSelectManager : MonoBehaviour
{

    [Header("The HUD gameobjects for each player")]
    [SerializeField]
    private List<GameObject> myPlayers = new List<GameObject>();

    [Header("The classes to be selectable")]
    [SerializeField]
    private List<ClassData> myClassHuds = new List<ClassData>();

    [Header("The colors to be selectable")]
    [SerializeField]
    private List<ColorScheme> myColorSchemes = new List<ColorScheme>();

    private CharacterGameData myCharacterGameData = null;

    private List<int> myPlayerClassIndex;
    private List<int> myPlayerColorIndex;

    [Header("Level to load")]
    [SerializeField]
    private string myNextLevel = "Coop";

    void Start()
    {
        myPlayerClassIndex = new List<int>();
        myPlayerColorIndex = new List<int>();
        for (int index = 0; index < myPlayers.Count; index++)
        {
            CharacterSelector characterSelector = myPlayers[index].GetComponent<CharacterSelector>();
            PlayerSetState(characterSelector, CharacterSelector.SelectionState.Class);
            myPlayerClassIndex.Add(index);
            myPlayerColorIndex.Add(index);

            characterSelector.SetClass(myClassHuds[myPlayerClassIndex[index]]);
            characterSelector.SetColor(myColorSchemes[myPlayerColorIndex[index]]);
            myPlayers[index].SetActive(false);
        }

        myCharacterGameData = FindObjectOfType<CharacterGameData>();
        List<PlayerSelectData> characters = myCharacterGameData.GetPlayerData();
        for (int index = 0; index < characters.Count; index++)
        {
            SetupCharacterSelector(GetAvailableCharacterSelector(), characters[index]);
            myPlayers[index].SetActive(true);
        }

        if(characters.Count == 0)
        {
            PlayerSelectData selectData = new PlayerSelectData(PlayerControls.CreateWithKeyboardBindings(), null, null, "DebugPlayer");
            SetupCharacterSelector(GetAvailableCharacterSelector(), selectData);
            myPlayers[0].SetActive(true);

            myCharacterGameData.AddPlayerData(selectData.myPlayerControls, "DebugPlayer");
        }
        
        myNextLevel = myCharacterGameData.mySceneToLoad;
    }

    CharacterSelector GetAvailableCharacterSelector()
    {
        for (int index = 0; index < myPlayers.Count; index++)
        {
            if (myPlayers[index].GetComponent<CharacterSelector>().PlayerControls == null)
                return myPlayers[index].GetComponent<CharacterSelector>();
        }

        return null;
    }

    void SetupCharacterSelector(CharacterSelector aCharacterSelector, PlayerSelectData aCharacterSelectData)
    {
        if (!aCharacterSelector)
            return;

        aCharacterSelector.Show(aCharacterSelectData.myPlayerControls, aCharacterSelectData.myName, this);
        PlayerSetState(aCharacterSelector, CharacterSelector.SelectionState.Class);
    }

    public void GetNextCharacter(CharacterSelector aCharacterSelector, int aModifier)
    {
        for (int index = 0; index < myPlayers.Count; index++)
        {
            if (myPlayers[index].GetComponent<CharacterSelector>() == aCharacterSelector)
            {
                myPlayerClassIndex[index] = GetNextAvailableOption(CharacterSelector.SelectionState.Class, myClassHuds, myPlayerClassIndex, myPlayerClassIndex[index], aModifier);
                aCharacterSelector.SetClass(myClassHuds[myPlayerClassIndex[index]]);
            }
        }
    }

    public void GetNextColor(CharacterSelector aCharacterSelector, int aModifier)
    {
        for (int index = 0; index < myPlayers.Count; index++)
        {
            if (myPlayers[index].GetComponent<CharacterSelector>() == aCharacterSelector)
            {
                myPlayerColorIndex[index] = GetNextAvailableOption(CharacterSelector.SelectionState.Color, myColorSchemes, myPlayerColorIndex, myPlayerColorIndex[index], aModifier);

                aCharacterSelector.SetColor(myColorSchemes[myPlayerColorIndex[index]]);
            }
        }
    }

    public void PlayerSetState(CharacterSelector aCharacterSelector, CharacterSelector.SelectionState aState)
    {
        aCharacterSelector.State = aState;
        switch (aCharacterSelector.State)
        {
            case CharacterSelector.SelectionState.Leave:
                SceneManager.LoadScene("LevelSelect");
                break;
            case CharacterSelector.SelectionState.Class:
                aCharacterSelector.SetInstructions("Choose your class and press A when ready");
                break;
            case CharacterSelector.SelectionState.Color:
                aCharacterSelector.SetInstructions("Choose a color and press A when ready");
                RemoveSameSelections(aState);
                break;
            case CharacterSelector.SelectionState.Ready:
                aCharacterSelector.SetInstructions("Press Start when everyone is ready");
                RemoveSameSelections(aState);
                break;
            default:
                break;
        }
    }

    private void RemoveSameSelections(CharacterSelector.SelectionState aState)
    {
        for (int index = 0; index < myPlayers.Count; index++)
        {
            CharacterSelector characterSelector = myPlayers[index].GetComponent<CharacterSelector>();
            if (characterSelector.State >= aState)
                continue;

            if (!IsSelectionAvailable(characterSelector.State, myPlayerClassIndex, myPlayerClassIndex[index]))
            {
                myPlayerClassIndex[index] = GetNextAvailableOption(characterSelector.State, myClassHuds, myPlayerClassIndex, myPlayerClassIndex[index]);
                characterSelector.SetClass(myClassHuds[myPlayerClassIndex[index]]);
            }
            if (!IsSelectionAvailable(characterSelector.State, myPlayerColorIndex, myPlayerColorIndex[index]))
            {
                myPlayerColorIndex[index] = GetNextAvailableOption(characterSelector.State, myColorSchemes, myPlayerColorIndex, myPlayerColorIndex[index]);
                characterSelector.SetColor(myColorSchemes[myPlayerColorIndex[index]]);
            }
        }
    }

    private bool IsSelectionAvailable(CharacterSelector.SelectionState aState, List<int> aPlayerIndexList, int aIndex)
    {
        for (int index = 0; index < aPlayerIndexList.Count; index++)
        {
            if (myPlayers[index].GetComponent<CharacterSelector>().State > aState && aPlayerIndexList[index] == aIndex)
                return false;
        }

        return true;
    }

    private int GetNextAvailableOption<T>(CharacterSelector.SelectionState aState, List<T> aOptionList, List<int> aPlayerIndexList, int aIndex, int aDirection = 1)
    {
        aIndex += aDirection;
        for (int counter = 0; counter < aOptionList.Count; counter++)
        {
            if (aIndex >= aOptionList.Count)
                aIndex = 0;
            if (aIndex < 0)
                aIndex = aOptionList.Count - 1;

            if (IsSelectionAvailable(aState, aPlayerIndexList, aIndex))
                return aIndex;

            aIndex += aDirection;
        }

        return -1;
    }

    public void StartPlaying()
    {
        for (int index = 0; index < myPlayers.Count; index++)
        {
            PlayerControls playerControls = myPlayers[index].GetComponent<CharacterSelector>().PlayerControls;
            if (playerControls == null)
                continue;

            myCharacterGameData.AddCharacterData(playerControls, myClassHuds[myPlayerClassIndex[index]], myColorSchemes[myPlayerColorIndex[index]]);
        }

        SceneManager.LoadScene(myNextLevel);
    }
}
