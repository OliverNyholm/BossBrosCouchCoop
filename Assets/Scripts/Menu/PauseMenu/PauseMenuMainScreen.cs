using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class PauseMenuMainScreen : PauseMenuSubMenu
{
    [SerializeField]
    private List<UnityEvent> mySubMenuOnSelectedEvents = new List<UnityEvent>(6);
    [SerializeField]
    private List<TextMeshProUGUI> mySubmenuesTexts = new List<TextMeshProUGUI>(6);
    private int myCurrentSubmenuIndex = 0;

    private int myFontSizeIncreaseOnSelected = 6;

    protected override void Update()
    {
        base.Update();

        if (myPlayerControls == null || !IsOpen())
            return;

        if(myPlayerControls.Action1.WasReleased)
        {
            mySubMenuOnSelectedEvents[myCurrentSubmenuIndex].Invoke();
        }

        if(myPlayerControlsMovement.WasPressed(1))
            NextMenu(-1);
        if (myPlayerControlsMovement.WasPressed(2))
            NextMenu(1);
    }

    public override void Open()
    {
        base.Open();

        TextMeshProUGUI currentSubmenuText = mySubmenuesTexts[myCurrentSubmenuIndex];
        currentSubmenuText.color = Color.yellow;
        currentSubmenuText.fontSize += myFontSizeIncreaseOnSelected;
        UtilityFunctions.InvertTextZRotation(currentSubmenuText);
    }

    public override void Close()
    {
        TextMeshProUGUI currentSubmenuText = mySubmenuesTexts[myCurrentSubmenuIndex];
        currentSubmenuText.fontSize -= myFontSizeIncreaseOnSelected;
        UtilityFunctions.InvertTextZRotation(currentSubmenuText);
        base.Close();
    }

    private void NextMenu(int aDirection)
    {
        TextMeshProUGUI previousSubmenuText = mySubmenuesTexts[myCurrentSubmenuIndex];
        previousSubmenuText.color = Color.white;
        previousSubmenuText.fontSize -= myFontSizeIncreaseOnSelected;
        UtilityFunctions.InvertTextZRotation(previousSubmenuText);

        myCurrentSubmenuIndex += aDirection;

        if (myCurrentSubmenuIndex < 0)
            myCurrentSubmenuIndex = mySubMenuOnSelectedEvents.Count - 1;
        else if(myCurrentSubmenuIndex >= mySubMenuOnSelectedEvents.Count)
            myCurrentSubmenuIndex = 0;

        TextMeshProUGUI newSubmenuText = mySubmenuesTexts[myCurrentSubmenuIndex];
        newSubmenuText.color = Color.yellow;
        newSubmenuText.fontSize += myFontSizeIncreaseOnSelected;
        UtilityFunctions.InvertTextZRotation(newSubmenuText);
    }
}
