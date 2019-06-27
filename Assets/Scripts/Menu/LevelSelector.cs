using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelector : MonoBehaviour
{
    [Header("All the levels to select from")]
    [SerializeField]
    private List<LevelInfo> myLevels = new List<LevelInfo>();
    private int myCurrentLevelIndex;

    [Header("The canvas to show info of levels with")]
    [SerializeField]
    private LevelSelectCanvas myLevelInfoCanvas = null;

    private LevelSelectCamera myCamera;

    private PlayerControls myKeyboardListener;
    private PlayerControls myJoystickListener;

    private bool myFirstUpdate;
    void Start()
    {
        myKeyboardListener = PlayerControls.CreateWithKeyboardBindings();
        myJoystickListener = PlayerControls.CreateWithJoystickBindings();

        myCamera = Camera.main.GetComponent<LevelSelectCamera>();
        myFirstUpdate = true;

        CharacterGameData gameData = FindObjectOfType<CharacterGameData>();
        myCurrentLevelIndex = gameData.myCurrentLevelIndex;
        myCamera.SetTargetPositionInstant(myLevels[myCurrentLevelIndex].transform);
        myLevelInfoCanvas.SetCanvasData(myLevels[myCurrentLevelIndex]);
    }

    // Update is called once per frame
    void Update()
    {
        if (myFirstUpdate)
        {
            myFirstUpdate = false;
            return;
        }

        if (WasRightClicked())
            NextLevel(1);
        if (WasLeftClicked())
            NextLevel(-1);

        if (WasStartClicked())
        {
            CharacterGameData gameData = FindObjectOfType<CharacterGameData>();
            gameData.mySceneToLoad = myLevels[myCurrentLevelIndex].mySceneNameToLoad;
            gameData.myCurrentLevelIndex = myCurrentLevelIndex;

            SceneManager.LoadScene("CharacterSelect");
        }

        if(WasBackClicked())
        {
            SceneManager.LoadScene("Menu");
        }
    }

    private void NextLevel(int aModifier)
    {        
        myCurrentLevelIndex += aModifier;
        if (myCurrentLevelIndex < 0 || myCurrentLevelIndex >= myLevels.Count)
        {
            myCurrentLevelIndex -= aModifier;
            return;
        }

        myCamera.SetTargetPosition(myLevels[myCurrentLevelIndex].transform);
        myLevelInfoCanvas.SetCanvasData(myLevels[myCurrentLevelIndex]);
    }

    private bool WasStartClicked()
    {
        if (myKeyboardListener.Start.WasPressed || myKeyboardListener.Action1.WasPressed || myJoystickListener.Action1.WasPressed || myJoystickListener.Start.WasPressed)
            return true;

        return false;
    }

    private bool WasBackClicked()
    {
        if (myKeyboardListener.Action2.WasPressed || myKeyboardListener.Action3.WasPressed || myJoystickListener.Action1.WasPressed || myJoystickListener.Action2.WasPressed)
            return true;

        return false;
    }

    private bool WasRightClicked()
    {
        if (myKeyboardListener.Right.WasPressed || myJoystickListener.Right.WasPressed)
            return true;

        return false;
    }

    private bool WasLeftClicked()
    {
        if (myKeyboardListener.Left.WasPressed || myJoystickListener.Left.WasPressed)
            return true;

        return false;
    }
}
