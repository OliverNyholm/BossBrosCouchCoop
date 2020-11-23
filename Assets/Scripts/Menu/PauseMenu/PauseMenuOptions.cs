using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PauseMenuOptions : PauseMenuSubMenu
{
    [SerializeField]
    private List<TextMeshProUGUI> myOptionsTexts = new List<TextMeshProUGUI>(2);

    [SerializeField]
    private List<OptionsBase> myOptions = new List<OptionsBase>(2);

    private int myCurrentOptionsIndex = 0;

    protected override void Update()
    {
        base.Update();

        if (myPlayerControls == null|| !IsOpen())
            return;

        if (myPlayerControls.Action1.WasPressed)
            myOptions[myCurrentOptionsIndex].NextOptions();

        if (BackPressed())
            myBaseMenu.CloseSubmenu();

        if (myPlayerControlsMovement.WasPressed(1))
            NextMenu(-1);
        if (myPlayerControlsMovement.WasPressed(2))
            NextMenu(1);
    }

    public override void Open()
    {
        base.Open();

        foreach (OptionsBase options in myOptions)
        {
            options.GetData();
            options.SetData();
        }

        myCurrentOptionsIndex = 0;

        TextMeshProUGUI myCurrentOptionsText = myOptionsTexts[myCurrentOptionsIndex];
        myCurrentOptionsText.color = Color.yellow;
        UtilityFunctions.InvertTextZRotation(myCurrentOptionsText);
        myOptions[myCurrentOptionsIndex].OnSelected();
    }

    public override void Close()
    {
        OptionsConfig optionsConfig = OptionsConfig.Instance;
        if (optionsConfig)
            optionsConfig.SaveOptions();

        TextMeshProUGUI myCurrentOptionsText = myOptionsTexts[myCurrentOptionsIndex];
        myCurrentOptionsText.color = Color.white;
        UtilityFunctions.InvertTextZRotation(myCurrentOptionsText);

        base.Close();
    }

    private void NextMenu(int aDirection)
    {
        TextMeshProUGUI previousSubmenuText = myOptionsTexts[myCurrentOptionsIndex];
        previousSubmenuText.color = Color.white;
        UtilityFunctions.InvertTextZRotation(previousSubmenuText);
        myOptions[myCurrentOptionsIndex].OnDeselected();

        myCurrentOptionsIndex += aDirection;

        if (myCurrentOptionsIndex < 0)
            myCurrentOptionsIndex = myOptionsTexts.Count - 1;
        else if (myCurrentOptionsIndex >= myOptionsTexts.Count)
            myCurrentOptionsIndex = 0;

        TextMeshProUGUI newSubmenuText = myOptionsTexts[myCurrentOptionsIndex];
        newSubmenuText.color = Color.yellow;
        UtilityFunctions.InvertTextZRotation(newSubmenuText);

        myOptions[myCurrentOptionsIndex].OnSelected();
    }
}
