using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionsHealTargeting : OptionsBase
{
    [SerializeField]
    private Image myImageToSetSpriteOn = null;

    [SerializeField]
    private Sprite mySelectWithJoystickSprite = null;
    [SerializeField]
    private Sprite mySelectWithStickAndAutoHealSprite = null;
    [SerializeField]
    private Material myLookAtTutorialGif = null;
    [SerializeField]
    private Sprite mySelectWithRightStickOrKeyboardSprite = null;

    [SerializeField]
    private TextMeshProUGUI myOptionText = null;
    private string mySelectWithJoystickOnlyText = "Select Player With Joystick";
    private string mySelectWithJoystickAndAutoHealText = "Select Player With Joystick And Auto Heal";
    private string mySelectWithLookDirectionText = "Chose Player On Look Direction And Auto Heal";
    private string mySelectWithRightStickOrKeyboardText = "Chose Player With Right Joystick Or Keyboard";

    private HealTargetingOption myHealTargetingOption;

    private void Awake()
    {
        myLookAtTutorialGif = new Material(myLookAtTutorialGif);
    }

    private void Update()
    {
        if (myImageToSetSpriteOn.material)
            myImageToSetSpriteOn.material.SetFloat("_unscaledTime", Time.unscaledTime);
    }

    public override void OnSelected()
    {
        myOptionText.color = Color.yellow;
        UtilityFunctions.InvertTextZRotation(myOptionText);
        SetOptionsText(true);
    }

    public override void OnDeselected()
    {
        myOptionText.color = Color.white;
        UtilityFunctions.InvertTextZRotation(myOptionText);
    }

    public override void NextOptions()
    {
        myHealTargetingOption++;
        if (myHealTargetingOption == HealTargetingOption.Count)
            myHealTargetingOption = 0;

        SetOptionsText(true);
        SetData();
    }

    public override void InitData()
    {
        OptionsConfig options = OptionsConfig.Instance;
        if (!options)
            return;

        myHealTargetingOption = options.myOptionsData.myHealTargetingMode;
        SetOptionsText(false);
    }

    public override void SetData()
    {
        OptionsConfig options = OptionsConfig.Instance;
        if (!options)
            return;

        options.myOptionsData.myHealTargetingMode = myHealTargetingOption;
        options.InvokeOptionsChanged();
    }

    void SetOptionsText(bool aShouldSetSprite)
    {
        switch (myHealTargetingOption)
        {
            case HealTargetingOption.SelectWithLeftStickOnly:
                myOptionText.text = mySelectWithJoystickOnlyText;
                if (aShouldSetSprite)
                { 
                    myImageToSetSpriteOn.sprite = mySelectWithJoystickSprite;
                    myImageToSetSpriteOn.material = null;
                }
                break;
            case HealTargetingOption.SelectWithLeftStickAndAutoHeal:
                myOptionText.text = mySelectWithJoystickAndAutoHealText;
                if (aShouldSetSprite)
                {
                    myImageToSetSpriteOn.sprite = mySelectWithStickAndAutoHealSprite;
                    myImageToSetSpriteOn.material = null;
                }
                break;
            case HealTargetingOption.SelectWithLookDirection:
                myOptionText.text = mySelectWithLookDirectionText;
                if (aShouldSetSprite)
                {
                    myImageToSetSpriteOn.sprite = null;
                    myImageToSetSpriteOn.material = myLookAtTutorialGif;
                    myImageToSetSpriteOn.material.SetFloat("_EnableTime", Time.time);
                }
                break;
            case HealTargetingOption.SelectWithRightStickOrKeyboard:
                myOptionText.text = mySelectWithRightStickOrKeyboardText;
                if (aShouldSetSprite)
                { 
                    myImageToSetSpriteOn.sprite = mySelectWithRightStickOrKeyboardSprite;
                    myImageToSetSpriteOn.material = null;
                }
                break;
            default:
                break;
        }
    }
}
