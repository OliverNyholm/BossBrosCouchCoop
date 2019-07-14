using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSelector : MonoBehaviour
{
    [Header("Text to show name")]
    [SerializeField]
    private Text myNameText = null;

    [Header("Text to show current insctructions")]
    [SerializeField]
    private Text myInstructionsText = null;

    public PlayerControls PlayerControls { get; set; }
    private PlayerSelectManager myManager;

    private float myInitializeTimer;
    private float myHoldDownTimer;

    private readonly string myAlphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZÅÄÖ-1234567890\'_";
    private char[] myLetters;
    private int myCharPosition;
    private int myCharIndex;
    private const int myMaxNameLength = 12;


    public enum SelectionState
    {
        Back,
        Name,
        Ready
    }

    public SelectionState State { get; set; }

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

            myCharPosition = 0;
            myCharIndex = myAlphabet.IndexOf(myLetters[myCharPosition]);
            SetLetter(0);
        }

        myInitializeTimer = 0.3f;
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

        if (PlayerControls.Start.WasPressed)
        {
            if (State == SelectionState.Ready)
                myManager.StartPlaying();
            else
                myManager.PlayerSetState(this, ++State);
        }

        if (PlayerControls.Action2.WasPressed && State == SelectionState.Ready)
        {
            myManager.PlayerSetState(this, --State);
            return;
        }

        if (State == SelectionState.Ready)
            return;

        if (PlayerControls.Right.WasPressed)
        {
            SetLetter(1);
            myHoldDownTimer = 0.5f;
        }
        if (PlayerControls.Left.WasPressed)
        {
            SetLetter(-1);
            myHoldDownTimer = 0.5f;
        }

        if (PlayerControls.Right.IsPressed)
        {
            myHoldDownTimer -= Time.deltaTime;
            if (myHoldDownTimer <= 0.0f)
            {
                SetLetter(1);
                myHoldDownTimer = 0.1f;
            }
        }
        if (PlayerControls.Left.IsPressed)
        {
            myHoldDownTimer -= Time.deltaTime;
            if (myHoldDownTimer <= 0.0f)
            {
                SetLetter(-1);
                myHoldDownTimer = 0.1f;
            }
        }

        if (PlayerControls.Action1.WasPressed)
        {
            if (HasCreatedDoubleBlankspace())
                return;

            myCharPosition++;
            if (myCharPosition >= myMaxNameLength)
                myCharPosition = myMaxNameLength - 1;

            myCharIndex = myAlphabet.IndexOf(myLetters[myCharPosition]);
            SetLetter(0);
        }
        if (PlayerControls.Action2.WasPressed)
        {
            myCharPosition--;
            if (myCharPosition < 0)
            {
                myCharPosition = 0;
                myManager.PlayerSetState(this, --State);
                return;
            }

            myCharIndex = myAlphabet.IndexOf(myLetters[myCharPosition]);
            SetLetter(0);
        }
    }

    public void Show(PlayerControls aPlayerControls, PlayerSelectManager aManager)
    {
        PlayerControls = aPlayerControls;
        myManager = aManager;

        myNameText.enabled = true;

        State = SelectionState.Name;

        myInitializeTimer = 0.3f;
        myCharPosition = 0;
    }

    public void Hide()
    {
        PlayerControls = null;
        myManager = null;

        myNameText.enabled = false;
    }

    public void SetInstructions(string aInstruction)
    {
        myInstructionsText.text = aInstruction;
    }

    public string GetName()
    {
        myNameText.text = myNameText.text.Replace('_', ' ');
        myNameText.text = myNameText.text.Replace("<color=red>", "");
        myNameText.text = myNameText.text.Replace("</color>", "");

        return myNameText.text;
    }

    private void SetLetter(int aMoveDirection)
    {
        myCharIndex += aMoveDirection;
        if (myCharIndex < 0)
            myCharIndex = myAlphabet.Length - 1;
        if (myCharIndex >= myAlphabet.Length)
            myCharIndex = 0;

        myLetters[myCharPosition] = myAlphabet[myCharIndex];
        myNameText.text = new string(myLetters);

        //Replaces all underlines with blankspaces
        myNameText.text = myNameText.text.Replace('_', ' ');

        //Replaces current positons blankspace with an underline so players know their selected position
        if (myNameText.text.Length > myCharPosition && myNameText.text[myCharPosition] == ' ')
        {
            myNameText.text = myNameText.text.Insert(myCharPosition + 1, "_");
            myNameText.text = myNameText.text.Remove(myCharPosition, 1);
        }

        //Remove all double blankspaces
        while (myNameText.text.Contains("  "))
            myNameText.text = myNameText.text.Replace("  ", " ");

        if (myCharPosition == myNameText.text.Length)
            myNameText.text += "</color>";
        else
            myNameText.text = myNameText.text.Insert(myCharPosition + 1, "</color>");

        myNameText.text = myNameText.text.Insert(myCharPosition, "<color=red>");
    }

    public bool HasCreatedDoubleBlankspace()
    {
        if (myCharPosition == 0)
            return false;

        if (myCharPosition == myMaxNameLength - 1)
            return false;

        if (myLetters[myCharPosition - 1] == '_' && myLetters[myCharPosition] == '_')
            return true;

        return false;
    }

    public void SetLetters(string aName)
    {
        myLetters = new char[myMaxNameLength];
        for (int index = 0; index < myMaxNameLength; index++)
        {
            myLetters[index] = '_';
        }

        for (int index = 0; index < aName.Length; index++)
        {
            myLetters[index] = aName[index];
        }

        myCharPosition = aName.Length - 2;
        if (myCharPosition < 0)
            myCharPosition = 0;
        myCharIndex = myAlphabet.IndexOf(myLetters[myCharPosition]);
        SetLetter(0);
    }
}
