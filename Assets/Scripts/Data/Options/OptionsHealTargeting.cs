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
    private Sprite myControllerSprite = null;

    [SerializeField]
    private TextMeshProUGUI myOptionText = null;
    private string mySelectWithStickOnlyText = "Select Player With Joystick";
    private string mySelectWithStickAndAutoHealText = "Select Player With Joystick And Auto Heal";
    private string mySelectWithLookDirectionText = "Chose Player On Look Direction";

    private HealTargetingOption myHealTargetingOption;

    public override void OnSelected()
    {
        myOptionText.color = Color.yellow;
        UtilityFunctions.InvertTextZRotation(myOptionText);

        myImageToSetSpriteOn.sprite = myControllerSprite;
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

        SetOptionsText();
        SetData();
    }

    public override void InitData()
    {
        OptionsConfig options = OptionsConfig.Instance;
        if (!options)
            return;

        myHealTargetingOption = options.myOptionsData.myHealTargetingMode;
        SetOptionsText();
    }

    public override void SetData()
    {
        OptionsConfig options = OptionsConfig.Instance;
        if (!options)
            return;

        options.myOptionsData.myHealTargetingMode = myHealTargetingOption;
        options.InvokeOptionsChanged();
    }

    void SetOptionsText()
    {
        switch (myHealTargetingOption)
        {
            case HealTargetingOption.SelectWithStickOnly:
                myOptionText.text = mySelectWithStickOnlyText;
                break;
            case HealTargetingOption.SelectWithStickAndAutoHeal:
                myOptionText.text = mySelectWithStickAndAutoHealText;
                break;
            case HealTargetingOption.SelectWithLookDirection:
                myOptionText.text = mySelectWithLookDirectionText;
                break;
            default:
                break;
        }
    }
}
