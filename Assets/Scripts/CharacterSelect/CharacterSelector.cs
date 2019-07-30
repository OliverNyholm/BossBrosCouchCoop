using System.Collections;
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

    [Header("Text to show player name")]
    [SerializeField]
    private Text myNameText = null;

    [Header("Text to show class name")]
    [SerializeField]
    private Text myClassNameText = null;

    [Header("Text to show class description")]
    [SerializeField]
    private Text myDescriptionText = null;

    [Header("Text to show current insctructions")]
    [SerializeField]
    private Text myInstructionsText = null;

    public PlayerControls PlayerControls { get; set; }
    private CharacterSelectManager myManager;

    float myPreviousLeftAxis;
    float myPreviousRightAxis;

    public enum SelectionState
    {
        Leave,
        Class,
        Color,
        Ready
    }

    public SelectionState State { get; set; }

    private void Awake()
    {
        Hide();
    }

    private void Update()
    {
        Color color = myInstructionsText.color;
        color.a = Mathf.Abs(Mathf.Sin(Time.time));
        myInstructionsText.color = color;

        if (PlayerControls == null)
            return;

        if (PlayerControls.Action2.WasPressed || PlayerControls.Action3.WasPressed)
        {
            myManager.PlayerSetState(this, --State);
        }

        if (PlayerControls.Action1.WasPressed || PlayerControls.Start.WasPressed)
        {
            if (State == SelectionState.Ready)
                myManager.StartPlaying();
            else
                myManager.PlayerSetState(this, ++State);
        }

        if (State == SelectionState.Ready)
            return;

        if (myPreviousLeftAxis == 0.0f && PlayerControls.Left.RawValue > 0.0f)
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
        if (myPreviousRightAxis == 0.0f && PlayerControls.Right.RawValue > 0.0f)
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

        myPreviousLeftAxis = PlayerControls.Left.RawValue > 0.0f ? 1.0f : 0.0f;
        myPreviousRightAxis = PlayerControls.Right.RawValue > 0.0f ? 1.0f : 0.0f;
    }

    public void Show(PlayerControls aPlayerControls, string aName, CharacterSelectManager aManager)
    {
        PlayerControls = aPlayerControls;
        myManager = aManager;
        myNameText.text = aName;

        myAvatar.enabled = true;
        myClassIcon.enabled = true;
        myNameText.enabled = true;
        myClassNameText.enabled = true;
        myDescriptionText.enabled = true;

        State = SelectionState.Class;
    }

    public void Hide()
    {
        PlayerControls = null;
        myManager = null;

        myAvatar.enabled = false;
        myClassIcon.enabled = false;
        myNameText.enabled = false;
        myClassNameText.enabled = false;
        myDescriptionText.enabled = false;
    }

    public void SetColor(ColorScheme aColorScheme)
    {
        myNameText.color = aColorScheme.myColor;
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
        return myNameText.text;
    }
}
