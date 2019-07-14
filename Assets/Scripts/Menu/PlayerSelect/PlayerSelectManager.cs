using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using InControl;

public class PlayerSelectManager : MonoBehaviour
{
    [Header("The HUD gameobjects for each player")]
    [SerializeField]
    private List<GameObject> myPlayers = new List<GameObject>();

    private CharacterGameData myCharacterGameData = null;

    private PlayerControls myKeyboardListener;
    private PlayerControls myJoystickListener;

    private bool myFirstUpdate;

    void Start()
    {
        for (int index = 0; index < myPlayers.Count; index++)
        {
            PlayerSelector PlayerSelector = myPlayers[index].GetComponent<PlayerSelector>();
            PlayerSetState(PlayerSelector, PlayerSelector.SelectionState.Back);
        }

        myKeyboardListener = PlayerControls.CreateWithKeyboardBindings();
        myJoystickListener = PlayerControls.CreateWithJoystickBindings();

        myCharacterGameData = FindObjectOfType<CharacterGameData>();
        List<PlayerSelectData> selectionData = myCharacterGameData.GetPlayerData();

        for (int index = 0; index < selectionData.Count; index++)
        {
            PlayerSelector selector = GetAvailablePlayerSelector();
            selector.Show(selectionData[index].myPlayerControls, this);
            PlayerSetState(selector, PlayerSelector.SelectionState.Name);
            selector.SetLetters(selectionData[index].myName);
        }
        myCharacterGameData.ClearPlayerData();
        myFirstUpdate = true;
    }

    private void OnDisable()
    {
        myJoystickListener.Destroy();
        if (IsKeyboardAvailable())
            myKeyboardListener.Destroy();
    }

    void Update()
    {
        if (myFirstUpdate)
        {
            myFirstUpdate = false;
            return;
        }


        if (JoinButtonWasPressedOnListener(myJoystickListener))
        {
            InputDevice inputDevice = InputManager.ActiveDevice;

            if (!IsInputDeviceAvailable(inputDevice))
                return;

            SetupPlayerSelector(GetAvailablePlayerSelector(), inputDevice);
        }
        if (myKeyboardListener != null && JoinButtonWasPressedOnListener(myKeyboardListener))
        {
            if (!IsKeyboardAvailable())
                return;

            SetupPlayerSelector(GetAvailablePlayerSelector(), null);
        }

        if (ExitButtonWasPressedOnListener(myJoystickListener))
        {
            InputDevice inputDevice = InputManager.ActiveDevice;
            if (IsInputDeviceAvailable(inputDevice))
            {
                myCharacterGameData.ClearPlayerData();
                SceneManager.LoadScene("Menu");
            }
        }
        if (ExitButtonWasPressedOnListener(myKeyboardListener))
        {
            if (IsKeyboardAvailable())
            {
                myCharacterGameData.ClearPlayerData();
                SceneManager.LoadScene("Menu");
            }
        }
    }

    bool IsInputDeviceAvailable(InputDevice aInputDevice)
    {
        for (int index = 0; index < myPlayers.Count; index++)
        {
            PlayerControls controls = myPlayers[index].GetComponent<PlayerSelector>().PlayerControls;

            if (controls != null && controls.Device == aInputDevice)
                return false;
        }

        return true;
    }

    bool IsKeyboardAvailable()
    {
        for (int index = 0; index < myPlayers.Count; index++)
        {
            PlayerControls controls = myPlayers[index].GetComponent<PlayerSelector>().PlayerControls;
            if (controls != null && controls.Device == myKeyboardListener.Device)
                return false;
        }

        return true;
    }

    bool JoinButtonWasPressedOnListener(PlayerControls aPlayerControls)
    {
        return aPlayerControls.Action1.WasPressed || aPlayerControls.Start.WasPressed;
    }

    bool ExitButtonWasPressedOnListener(PlayerControls aPlayerControls)
    {
        return aPlayerControls.Action2.WasPressed || aPlayerControls.Action3.WasPressed;
    }

    PlayerSelector GetAvailablePlayerSelector()
    {
        for (int index = 0; index < myPlayers.Count; index++)
        {
            if (myPlayers[index].GetComponent<PlayerSelector>().PlayerControls == null)
                return myPlayers[index].GetComponent<PlayerSelector>();
        }

        return null;
    }

    void SetupPlayerSelector(PlayerSelector aPlayerSelector, InputDevice aInputDevice)
    {
        if (!aPlayerSelector)
            return;

        PlayerControls playerControls = null;// = myKeyboardListener;
        if (aInputDevice != null)
        {
            playerControls = PlayerControls.CreateWithJoystickBindings();
            playerControls.Device = aInputDevice;
        }
        else
        {
            //playerControls = PlayerControls.CreateWithJoystickBindings();
            playerControls = myKeyboardListener;
        }

        aPlayerSelector.Show(playerControls, this);
        PlayerSetState(aPlayerSelector, PlayerSelector.SelectionState.Name);
    }

    public void PlayerSetState(PlayerSelector aPlayerSelector, PlayerSelector.SelectionState aState)
    {
        aPlayerSelector.State = aState;
        switch (aPlayerSelector.State)
        {
            case PlayerSelector.SelectionState.Back:
                aPlayerSelector.SetInstructions("Press A to join.");
                aPlayerSelector.Hide();
                break;
            case PlayerSelector.SelectionState.Name:
                aPlayerSelector.SetInstructions("Choose your name by moving left/right and A/B. Press Start when ready.");
                break;
            case PlayerSelector.SelectionState.Ready:
                aPlayerSelector.SetInstructions("Press Start when everyone is ready");
                RemoveSameSelections(aState);
                break;
            default:
                break;
        }
    }

    private void RemoveSameSelections(PlayerSelector.SelectionState aState)
    {
        for (int index = 0; index < myPlayers.Count; index++)
        {
            PlayerSelector playerSelector = myPlayers[index].GetComponent<PlayerSelector>();
            if (playerSelector.State >= aState)
                continue;

            //if (!IsSelectionAvailable(playerSelector.State, myPlayerClassIndex, myPlayerClassIndex[index]))
            //{
            //    myPlayerClassIndex[index] = GetNextAvailableOption(playerSelector.State, myClassHuds, myPlayerClassIndex, myPlayerClassIndex[index]);
            //    playerSelector.SetClass(myClassHuds[myPlayerClassIndex[index]]);
            //}
            //if (!IsSelectionAvailable(playerSelector.State, myPlayerColorIndex, myPlayerColorIndex[index]))
            //{
            //    myPlayerColorIndex[index] = GetNextAvailableOption(playerSelector.State, myColorSchemes, myPlayerColorIndex, myPlayerColorIndex[index]);
            //    playerSelector.SetColor(myColorSchemes[myPlayerColorIndex[index]]);
            //}
        }
    }

    private bool IsSelectionAvailable(PlayerSelector.SelectionState aState, List<int> aPlayerIndexList, int aIndex)
    {
        for (int index = 0; index < aPlayerIndexList.Count; index++)
        {
            if (myPlayers[index].GetComponent<PlayerSelector>().State > aState && aPlayerIndexList[index] == aIndex)
                return false;
        }

        return true;
    }

    private int GetNextAvailableOption<T>(PlayerSelector.SelectionState aState, List<T> aOptionList, List<int> aPlayerIndexList, int aIndex, int aDirection = 1)
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
            PlayerControls playerControls = myPlayers[index].GetComponent<PlayerSelector>().PlayerControls;
            if (playerControls == null)
                continue;

            myCharacterGameData.AddPlayerData(playerControls, myPlayers[index].GetComponent<PlayerSelector>().GetName());
        }

        SceneManager.LoadScene("LevelSelect");
    }
}
