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

    [SerializeField]
    private List<KeyboardKey> myFirstRowKeys = new List<KeyboardKey>();
    [SerializeField]
    private List<KeyboardKey> mySecondRowKeys = new List<KeyboardKey>();
    [SerializeField]
    private List<KeyboardKey> myThirdRowKeys = new List<KeyboardKey>();

    private List<List<KeyboardKey>> myKeys = new List<List<KeyboardKey>>(3);

    private bool myFirstUpdate;

    private void Awake()
    {
        for (int index = 0; index < myFirstRowKeys.Count; index++)
            myFirstRowKeys[index].SetColumnAndRow(index, 0);

        for (int index = 0; index < mySecondRowKeys.Count; index++)
            mySecondRowKeys[index].SetColumnAndRow(index, 1);

        for (int index = 0; index < myThirdRowKeys.Count; index++)
            myThirdRowKeys[index].SetColumnAndRow(index, 2);

        myKeys.Add(myFirstRowKeys);
        myKeys.Add(mySecondRowKeys);
        myKeys.Add(myThirdRowKeys);
    }

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
            PlayerSelector selector = GetAvailablePlayerSelector(out int availableIndex);
            selector.Show(selectionData[index].myPlayerControls, this, myKeys[0][0], availableIndex);
            PlayerSetState(selector, PlayerSelector.SelectionState.Name);
            selector.SetPlayerName(selectionData[index].myName);
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

            SetupPlayerSelector(GetAvailablePlayerSelector(out int availableIndex), inputDevice, availableIndex);
        }
        if (myKeyboardListener != null && JoinButtonWasPressedOnListener(myKeyboardListener))
        {
            if (!IsKeyboardAvailable())
                return;

            SetupPlayerSelector(GetAvailablePlayerSelector(out int availableIndex), null, availableIndex);
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
        return aPlayerControls.Jump.WasPressed;
    }

    bool ExitButtonWasPressedOnListener(PlayerControls aPlayerControls)
    {
        return aPlayerControls.TargetEnemy.WasPressed;
    }

    PlayerSelector GetAvailablePlayerSelector(out int anAvailableIndex)
    {
        anAvailableIndex = -1;
        for (int index = 0; index < myPlayers.Count; index++)
        {
            if (myPlayers[index].GetComponent<PlayerSelector>().PlayerControls == null)
            {
                anAvailableIndex = index;
                return myPlayers[index].GetComponent<PlayerSelector>();
            }
        }

        return null;
    }

    void SetupPlayerSelector(PlayerSelector aPlayerSelector, InputDevice aInputDevice, int aPlayerIndex)
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
            playerControls = myKeyboardListener;
        }

        aPlayerSelector.Show(playerControls, this, myKeys[0][0], aPlayerIndex);
        PlayerSetState(aPlayerSelector, PlayerSelector.SelectionState.Name);
    }

    public KeyboardKey GetKeyboardKey(KeyboardKey aCurrentKey, int aDirection)
    {
        int nextRow = aCurrentKey.GetRowIndex();
        int nextColumn = aCurrentKey.GetColumnIndex();
        switch (aDirection)
        {
            case 1: //up
                nextRow -= 1;
                break;
            case 2: //down
                nextRow += 1;
                break;
            case 3: //left
                nextColumn -= 1;
                break;
            case 4: //right
                nextColumn += 1;
                break;
            default:
                return null;
        }

        if (nextRow < 0 || nextRow >= myKeys.Count)
            return aCurrentKey;

        if (nextColumn >= myKeys[nextRow].Count)
            nextColumn = 0;
        else if (nextColumn == -1)
            nextColumn = myKeys[nextRow].Count - 1;

        return myKeys[nextRow][nextColumn];
    }

    public void PlayerSetState(PlayerSelector aPlayerSelector, PlayerSelector.SelectionState aState)
    {
        aPlayerSelector.State = aState;
        switch (aPlayerSelector.State)
        {
            case PlayerSelector.SelectionState.Back:
                aPlayerSelector.SetInstructions("Press Right Bumper to join.");
                aPlayerSelector.Hide();
                break;
            case PlayerSelector.SelectionState.Name:
                aPlayerSelector.SetInstructions("Choose your name by moving the stick and A/B. Press Right Bumper when ready.");
                break;
            case PlayerSelector.SelectionState.Ready:
                aPlayerSelector.SetInstructions("Press Right Bumper when everyone is ready");
                break;
            default:
                break;
        }
    }

    public void StartPlaying()
    {
        for (int index = 0; index < myPlayers.Count; index++)
        {
            PlayerSelector playerSelector = myPlayers[index].GetComponent<PlayerSelector>();
            if (playerSelector.PlayerControls == null)
                continue;

            if (playerSelector.State != PlayerSelector.SelectionState.Ready)
                return;
        }

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
