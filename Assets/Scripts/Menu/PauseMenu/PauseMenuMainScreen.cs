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

        mySubmenuesTexts[myCurrentSubmenuIndex].color = Color.yellow;
        mySubmenuesTexts[myCurrentSubmenuIndex].fontSize += myFontSizeIncreaseOnSelected;
        InvertTextRotation();
    }

    public override void Close()
    {
        mySubmenuesTexts[myCurrentSubmenuIndex].fontSize -= myFontSizeIncreaseOnSelected;
        InvertTextRotation();
        base.Close();
    }

    private void NextMenu(int aDirection)
    {
        TextMeshProUGUI previousSubmenuText = mySubmenuesTexts[myCurrentSubmenuIndex];
        previousSubmenuText.color = Color.white;
        previousSubmenuText.fontSize -= myFontSizeIncreaseOnSelected;
        InvertTextRotation();

        myCurrentSubmenuIndex += aDirection;

        if (myCurrentSubmenuIndex < 0)
            myCurrentSubmenuIndex = mySubMenuOnSelectedEvents.Count - 1;
        else if(myCurrentSubmenuIndex >= mySubMenuOnSelectedEvents.Count)
            myCurrentSubmenuIndex = 0;

        TextMeshProUGUI newSubmenuText = mySubmenuesTexts[myCurrentSubmenuIndex];
        newSubmenuText.color = Color.yellow;
        newSubmenuText.fontSize += myFontSizeIncreaseOnSelected;
        InvertTextRotation();
    }

    private void InvertTextRotation()
    {
        TextMeshProUGUI currentSelectedSubMenuText = mySubmenuesTexts[myCurrentSubmenuIndex];

        Vector3 invertRotation = currentSelectedSubMenuText.transform.eulerAngles;
        invertRotation.z *= -1.0f;
        currentSelectedSubMenuText.transform.eulerAngles = invertRotation;
    }
}
