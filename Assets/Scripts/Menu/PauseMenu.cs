using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool ourIsGamePaused = false;

    [Header("The parent gameobject to pause menu")]
    [SerializeField]
    private GameObject myPauseObject = null;

    [Header("The Play Button")]
    [SerializeField]
    private Button myResumeButton = null;

    [Header("The Controls Button")]
    [SerializeField]
    private Button myControlsButton = null;

    [Header("The Quit Button")]
    [SerializeField]
    private Button myCharacterSelectButton = null;

    [Header("The Controls Image")]
    [SerializeField]
    private Image myControlsImage = null;

    private List<Button> myButtons;
    private int myCurrentButtonIndex;

    private PlayerControls myKeyboardListener;
    private PlayerControls myJoystickListener;

    private void Start()
    {
        myButtons = new List<Button>
        {
            myCharacterSelectButton,
            myResumeButton,
            myControlsButton
        };

        myKeyboardListener = PlayerControls.CreateWithKeyboardBindings();
        myJoystickListener = PlayerControls.CreateWithJoystickBindings();

        myCurrentButtonIndex = 0;
        NextButton(1);
    }

    void Update()
    {
        if (WasPausePressed())
        {
            ourIsGamePaused = !ourIsGamePaused;
            if (ourIsGamePaused)
                Pause();
            else
                Resume();
        }

        if (!ourIsGamePaused)
            return;

        if (WasRightPressed())
            NextButton(1);
        if (WasLeftPressed())
            NextButton(-1);

        if (WasAction1Released())
            myButtons[myCurrentButtonIndex].onClick.Invoke();
    }

    private void Pause()
    {
        Time.timeScale = 0.0f;
        myPauseObject.SetActive(true);
        ourIsGamePaused = true;

        List<GameObject> players = FindObjectOfType<TargetHandler>().GetAllPlayers();

        const int playerMax = 4;
        for (int index = 0; index < playerMax; index++)
        {
            if (players.Count <= index)
            {
                myPauseObject.transform.GetChild(index).gameObject.SetActive(false);
            }
            else
            {
                myPauseObject.transform.GetChild(index).gameObject.SetActive(true);
                myPauseObject.transform.GetChild(index).GetComponent<PausePlayerUI>().SetClassDetails(players[index].GetComponent<Class>());
            }
        }
    }

    public void Resume()
    {
        Time.timeScale = 1.0f;
        ourIsGamePaused = false;
        myPauseObject.SetActive(false);
    }

    public void LoadCharacterSelect()
    {
        ourIsGamePaused = false;
        Resume();
        SceneManager.LoadScene("CharacterSelect");
    }

    public void ToggleControls()
    {
        myControlsImage.enabled = !myControlsImage.enabled;
    }

    private void NextButton(int aModifier)
    {
        myControlsImage.enabled = false;

        myButtons[myCurrentButtonIndex].image.color = myButtons[myCurrentButtonIndex].colors.normalColor;
        myCurrentButtonIndex += aModifier;
        if (myCurrentButtonIndex < 0)
            myCurrentButtonIndex = myButtons.Count - 1;
        if (myCurrentButtonIndex >= myButtons.Count)
            myCurrentButtonIndex = 0;

        myButtons[myCurrentButtonIndex].image.color = myButtons[myCurrentButtonIndex].colors.selectedColor;
    }

    private bool WasAction1Released()
    {
        if (myKeyboardListener.Action1.WasReleased || myJoystickListener.Action1.WasReleased)
            return true;

        return false;
    }

    private bool WasRightPressed()
    {
        if (myKeyboardListener.Right.WasPressed || myJoystickListener.Right.WasPressed)
            return true;

        return false;
    }

    private bool WasLeftPressed()
    {
        if (myKeyboardListener.Left.WasPressed || myJoystickListener.Left.WasPressed)
            return true;

        return false;
    }

    private bool WasPausePressed()
    {
        if (myKeyboardListener.Start.WasPressed || myKeyboardListener.Pause.WasPressed || myJoystickListener.Pause.WasPressed || myJoystickListener.Start.WasPressed)
            return true;

        return false;
    }
}
