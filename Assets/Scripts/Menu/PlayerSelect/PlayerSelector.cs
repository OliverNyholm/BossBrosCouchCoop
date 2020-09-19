using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerSelector : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI myNameText = null;

    [SerializeField]
    private TextMeshProUGUI myInstructionsText = null;

    [SerializeField]
    private GameObject mySelectionHighlight = null;

    public PlayerControls PlayerControls { get; set; }
    private PlayerSelectManager myManager;

    private KeyboardKey myHoveredKey = null;

    private const int myMaxNameLength = 12;
    private char[] myLetters;

    private float myInitializeTimer = 0.3f;

    private Vector4 myPreviousControllerLeftAxis;

    public enum SelectionState
    {
        Back,
        Name,
        Ready
    }

    public SelectionState State { get; set; }

    private void Awake()
    {
        mySelectionHighlight.GetComponent<Image>().material = new Material(mySelectionHighlight.GetComponent<Image>().material);
        mySelectionHighlight.SetActive(false);
    }

    private void Start()
    {
        if (myLetters == null)
        {
            Hide();
            myLetters = new char[myMaxNameLength];
            for (int index = 0; index < myMaxNameLength; index++)
            {
                myLetters[index] = '_';
            }
        }
    }

    private void Update()
    {
        Color color = myInstructionsText.color;
        color.a = Mathf.Abs(Mathf.Sin(Time.time));
        myInstructionsText.color = color;

        if (PlayerControls == null)
            return;

        if (myInitializeTimer > 0.0f)
        {
            myInitializeTimer -= Time.deltaTime;
            return;
        }

        if (PlayerControls.Jump.WasPressed)
        {
            if (State == SelectionState.Ready)
                myManager.StartPlaying();
            else
                myManager.PlayerSetState(this, ++State);
        }

        if (PlayerControls.TargetEnemy.WasPressed)
        {
            myManager.PlayerSetState(this, --State);
            return;
        }

        if (State == SelectionState.Ready)
            return;

        CheckInput();
    }

    public void Show(PlayerControls aPlayerControls, PlayerSelectManager aManager, KeyboardKey aKeyboardKey, int aPlayerIndex)
    {
        PlayerControls = aPlayerControls;
        myManager = aManager;
        myHoveredKey = aKeyboardKey;

        myNameText.enabled = true;
        myNameText.text = "";
        mySelectionHighlight.SetActive(true);
        mySelectionHighlight.GetComponent<Image>().material.SetInt("_PlayerIndex", aPlayerIndex);

        SetHighlightPosition();

        myInitializeTimer = 0.3f;
        State = SelectionState.Name;
    }

    public void Hide()
    {
        PlayerControls = null;
        myManager = null;

        mySelectionHighlight.SetActive(false);

        myNameText.enabled = false;
    }

    public void SetInstructions(string aInstruction)
    {
        myInstructionsText.text = aInstruction;
    }

    public string GetName()
    {
        return myNameText.text;
    }

    public void SetPlayerName(string aString)
    {
        myNameText.text = aString;
    }

    private void SetHighlightPosition()
    {
        RectTransform keyTransform = myHoveredKey.transform.GetComponent<RectTransform>();
        RectTransform selectionTransform = mySelectionHighlight.GetComponent<RectTransform>();
        selectionTransform.anchorMin = keyTransform.anchorMin;
        selectionTransform.anchorMax = keyTransform.anchorMax;
    }

    private void CheckInput()
    {
        if (PlayerControls.myIsController)
            CheckControllerInput();
        else
            CheckKeyboardInput();
    }

    private void CheckControllerInput()
    {
        const float controllerThreshold = 0.4f;
        if (myPreviousControllerLeftAxis[0] == 0.0f && PlayerControls.Up.RawValue > controllerThreshold)
            GetNextKey(1);
        if (myPreviousControllerLeftAxis[1] == 0.0f && PlayerControls.Down.RawValue > controllerThreshold)
            GetNextKey(2);
        if (myPreviousControllerLeftAxis[2] == 0.0f && PlayerControls.Left.RawValue > controllerThreshold)
            GetNextKey(3);
        if (myPreviousControllerLeftAxis[3] == 0.0f && PlayerControls.Right.RawValue > controllerThreshold)
            GetNextKey(4);

        myPreviousControllerLeftAxis[0] = PlayerControls.Up.RawValue > controllerThreshold ? 1.0f : 0.0f;
        myPreviousControllerLeftAxis[1] = PlayerControls.Down.RawValue > controllerThreshold ? 1.0f : 0.0f;
        myPreviousControllerLeftAxis[2] = PlayerControls.Left.RawValue > controllerThreshold ? 1.0f : 0.0f;
        myPreviousControllerLeftAxis[3] = PlayerControls.Right.RawValue > controllerThreshold ? 1.0f : 0.0f;

        if (PlayerControls.Action1.WasPressed)
        {
            AddLetter();
        }
        if (PlayerControls.Action2.WasPressed)
        {
            RemoveLetter();
        }
    }

    private void CheckKeyboardInput()
    {        
        if (PlayerControls.Up.WasPressed)
        {
            GetNextKey(1);
        }
        if (PlayerControls.Down.WasPressed)
        {
            GetNextKey(2);
        }
        if (PlayerControls.Left.WasPressed)
        {
            GetNextKey(3);
        }
        if (PlayerControls.Right.WasPressed)
        {
            GetNextKey(4);
        }

        if (PlayerControls.Action1.WasPressed)
        {
            AddLetter();
        }
        if (PlayerControls.Action2.WasPressed)
        {
            RemoveLetter();
        }
    }

    private void GetNextKey(int aDirection)
    {
        myHoveredKey = myManager.GetKeyboardKey(myHoveredKey, aDirection);
        SetHighlightPosition();
    }
    private void AddLetter()
    {
        if (myNameText.text.Length == myMaxNameLength)
            return;

        char newChar = myHoveredKey.GetLetter();
        if (newChar.Equals(' ') && myNameText.text[myNameText.text.Length - 1].Equals(' ')) //Don't add multiple spaces
            return;

        myNameText.text = myNameText.text + newChar;
    }

    private void RemoveLetter()
    {
        if (myNameText.text.Length > 1)
            myNameText.text = myNameText.text.Substring(0, myNameText.text.Length - 1);
        else
            myNameText.text = "";
    }
}
