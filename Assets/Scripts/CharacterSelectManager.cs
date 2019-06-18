using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
using UnityEngine.SceneManagement;

public class CharacterSelectManager : MonoBehaviour
{
    [Header("The class that should store the players for all levels")]
    [SerializeField]
    private CharacterGameData myCharacterGameData;

    [Header("The HUD gameobjects for each player")]
    [SerializeField]
    private List<GameObject> myPlayers;

    [Header("The classes to be selectable")]
    [SerializeField]
    private List<ClassData> myClassHuds;

    [Header("The colors to be selectable")]
    [SerializeField]
    private List<ColorScheme> myColorSchemes;

    private List<int> myPlayerClassIndex;
    private List<int> myPlayerColorIndex;

    void Start()
    {
        myPlayerClassIndex = new List<int>();
        myPlayerColorIndex = new List<int>();
        for (int index = 0; index < myPlayers.Count; index++)
        {
            myPlayers[index].GetComponent<CharacterSelector>().SetInstructions("Press A to join game");
            myPlayerClassIndex.Add(index);
            myPlayerColorIndex.Add(index);
        }
    }

    void Update()
    {
        InputDevice activeDevice = InputManager.ActiveDevice;
        if (!activeDevice.Action1.WasPressed)
            return;

        if (!IsInputDeviceAvailable(activeDevice))
            return;

        SetupCharacterSelector(GetAvailableCharacterSelector(), activeDevice);
    }

    bool IsInputDeviceAvailable(InputDevice aInputDevice)
    {
        for (int index = 0; index < myPlayers.Count; index++)
        {
            if (myPlayers[index].GetComponent<CharacterSelector>().Device == aInputDevice)
                return false;
        }

        return true;
    }

    CharacterSelector GetAvailableCharacterSelector()
    {
        for (int index = 0; index < myPlayers.Count; index++)
        {
            if (myPlayers[index].GetComponent<CharacterSelector>().Device == null)
                return myPlayers[index].GetComponent<CharacterSelector>();
        }

        return null;
    }

    void SetupCharacterSelector(CharacterSelector aCharacterSelector, InputDevice aInputDevice)
    {
        if (!aCharacterSelector)
            return;

        aCharacterSelector.Show(aInputDevice, this);
        aCharacterSelector.SetInstructions("Choose your character and press A when ready");
    }

    public void GetNextCharacter(CharacterSelector aCharacterSelector, int aModifier)
    {
        for (int index = 0; index < myPlayers.Count; index++)
        {
            if (myPlayers[index].GetComponent<CharacterSelector>() == aCharacterSelector)
            {
                myPlayerClassIndex[index] += aModifier;
                if (myPlayerClassIndex[index] >= myClassHuds.Count)
                    myPlayerClassIndex[index] = 0;
                else if (myPlayerClassIndex[index] < 0)
                    myPlayerClassIndex[index] = myClassHuds.Count - 1;

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
                myPlayerColorIndex[index] += aModifier;
                if (myPlayerColorIndex[index] >= myColorSchemes.Count)
                    myPlayerColorIndex[index] = 0;
                else if (myPlayerColorIndex[index] < 0)
                    myPlayerColorIndex[index] = myColorSchemes.Count - 1;

                aCharacterSelector.SetColor(myColorSchemes[myPlayerColorIndex[index]]);
            }
        }
    }

    public void GetNextDescription(CharacterSelector aCharacterSelector)
    {
        switch (aCharacterSelector.State)
        {
            case CharacterSelector.SelectionState.Idle:
                aCharacterSelector.SetInstructions("Press A to join game");
                break;
            case CharacterSelector.SelectionState.Class:
                aCharacterSelector.SetInstructions("Choose your class and press A when ready");
                break;
            case CharacterSelector.SelectionState.Color:
                aCharacterSelector.SetInstructions("Choose a color and press A when ready");
                break;
            case CharacterSelector.SelectionState.Ready:
                aCharacterSelector.SetInstructions("Press Start when everyone is ready");
                break;
            default:
                break;
        }
    }

    public void StartPlaying()
    {
        for (int index = 0; index < myPlayers.Count; index++)
        {
            InputDevice inputDevice = myPlayers[index].GetComponent<CharacterSelector>().Device;
            if (inputDevice == null)
                continue;

            myCharacterGameData.AddPlayerData(inputDevice, myClassHuds[myPlayerClassIndex[index]], myColorSchemes[myPlayerColorIndex[index]]);
        }

        SceneManager.LoadScene("Coop");
    }
}
