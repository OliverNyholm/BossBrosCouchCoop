﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using InControl;

public class CharacterSelector : MonoBehaviour
{
    [Header("Image to show player color")]
    [SerializeField]
    private Image myAvatar = null;

    [Header("Image to show class icon")]
    [SerializeField]
    private Image myClassIcon = null;

    [Header("Text to show class color")]
    [SerializeField]
    private Text myColorText = null;

    [Header("Text to show class name")]
    [SerializeField]
    private Text myClassNameText = null;

    [Header("Text to show class description")]
    [SerializeField]
    private Text myDescriptionText = null;

    [Header("Text to show current insctructions")]
    [SerializeField]
    private Text myInstructionsText = null;

    public InputDevice Device { get; set; }
    private CharacterSelectManager myManager;

    float myPreviousLeftAxis;
    float myPreviousRightAxis;

    bool myIsInitialized;

    public enum SelectionState
    {
        Idle,
        Class,
        Color,
        Ready
    }

    public SelectionState State { get; set; }

    private void Start()
    {
        Hide();
    }

    private void Update()
    {
        Color color = myInstructionsText.color;
        color.a = Mathf.Abs(Mathf.Sin(Time.time));
        myInstructionsText.color = color;

        if (Device == null)
            return;

        if(myIsInitialized)
        {
            myIsInitialized = false;
            return;
        }

        if (Device.Action2.WasPressed)
        {
            myManager.PlayerSetState(this, --State);
            if (State == SelectionState.Idle)
                return;
        }

        if (Device.CommandWasPressed)
            myManager.StartPlaying();

        if (State == SelectionState.Ready)
            return;

        if (Device.Action1.WasPressed)
        {
            myManager.PlayerSetState(this, ++State);
        }


        if (myPreviousLeftAxis == 0.0f && Device.LeftStickLeft.RawValue > 0.0f)
        {
            switch (State)
            {
                case SelectionState.Class:
                    myManager.GetNextCharacter(this, -1);
                    break;
                case SelectionState.Color:
                    myManager.GetNextColor(this, -1);
                    break;
            }
        }
        if (myPreviousRightAxis == 0.0f && Device.LeftStickRight.RawValue > 0.0f)
        {
            switch (State)
            {
                case SelectionState.Class:
                    myManager.GetNextCharacter(this, 1);
                    break;
                case SelectionState.Color:
                    myManager.GetNextColor(this, 1);
                    break;
            }
        }

        myPreviousLeftAxis = Device.LeftStickLeft.RawValue > 0.0f ? 1.0f : 0.0f;
        myPreviousRightAxis = Device.LeftStickRight.RawValue > 0.0f ? 1.0f : 0.0f;
    }

    public void Show(InputDevice aInputDevice, CharacterSelectManager aManager)
    {
        Device = aInputDevice;
        myManager = aManager;

        myAvatar.enabled = true;
        myClassIcon.enabled = true;
        myColorText.enabled = true;
        myClassNameText.enabled = true;
        myDescriptionText.enabled = true;

        State = SelectionState.Class;

        myIsInitialized = true;
    }

    public void Hide()
    {
        Device = null;
        myManager = null;

        myAvatar.enabled = false;
        myClassIcon.enabled = false;
        myColorText.enabled = false;
        myClassNameText.enabled = false;
        myDescriptionText.enabled = false;
    }

    public void SetColor(ColorScheme aColorScheme)
    {
        myColorText.color = aColorScheme.myColor;
        myAvatar.sprite = aColorScheme.myAvatar;
    }

    public void SetClass(ClassData aClassData)
    {
        myClassIcon.sprite = aClassData.myIconSprite;
        myClassNameText.text = aClassData.myName;
        myDescriptionText.text = aClassData.myDescription;
    }

    public void SetInstructions(string aInstruction)
    {
        myInstructionsText.text = aInstruction;
    }

    public string GetName()
    {
        return myColorText.text;
    }
}