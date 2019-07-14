using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [Header("The Play Button")]
    [SerializeField]
    private Button myPlayButton = null;

    [Header("The Controls Button")]
    [SerializeField]
    private Button myControlsButton = null;

    [Header("The Quit Button")]
    [SerializeField]
    private Button myQuitButton = null;

    [Header("The Controls Image")]
    [SerializeField]
    private Image myControlsImage = null;

    private List<Button> myButtons;
    private int myCurrentButtonIndex;

    private PlayerControls myKeyboardListener;
    private PlayerControls myJoystickListener;

    private bool myFirstUpdate;

    void Start()
    {
        myButtons = new List<Button>
        {
            myPlayButton,
            myControlsButton,
            myQuitButton
        };

        myCurrentButtonIndex = myButtons.Count - 1;
        NextButton(1);
        myKeyboardListener = PlayerControls.CreateWithKeyboardBindings();
        myJoystickListener = PlayerControls.CreateWithJoystickBindings();

        myFirstUpdate = true;
    }

    private void OnDestroy()
    {
        myJoystickListener.Destroy();
        myKeyboardListener.Destroy();
    }

    private void Update()
    {
        if(myFirstUpdate)
        {
            myFirstUpdate = false;
            return;
        }

        if (WasUpClicked())
            NextButton(-1);
        if (WasDownClicked())
            NextButton(1);

        if (WasStartClicked())
            myButtons[myCurrentButtonIndex].onClick.Invoke();
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("PlayerSelect");
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
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

    private bool WasStartClicked()
    {
        if (myKeyboardListener.Start.WasPressed || myKeyboardListener.Action1.WasPressed || myJoystickListener.Action1.WasPressed || myJoystickListener.Start.WasPressed)
            return true;

        return false;
    }

    private bool WasUpClicked()
    {
        if (myKeyboardListener.Up.WasPressed || myJoystickListener.Up.WasPressed)
            return true;

        return false;
    }

    private bool WasDownClicked()
    {
        if (myKeyboardListener.Down.WasPressed || myJoystickListener.Down.WasPressed)
            return true;

        return false;
    }
}
