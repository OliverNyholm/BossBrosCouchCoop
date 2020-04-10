using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIComponent : UIComponent
{
    private GameObject myActionBar;
    private GameObject[] myActionButtons;

    private SpellErrorHandler mySpellErrorHandler;

    void Start()
    {
        Transform uiHud = GameObject.Find("PlayerHud" + GetComponent<Player>().PlayerIndex).transform;
        SetupHud(uiHud);

        mySpellErrorHandler = uiHud.GetComponentInChildren<SpellErrorHandler>();
    }

    public override void SetupHud(Transform aUIParent)
    {
        base.SetupHud(aUIParent);

        Class playerClass = GetComponent<Class>();
        GetComponentInChildren<TargetProjector>().SetPlayerColor(myCharacterColor);

        myActionBar = aUIParent.Find("ActionBar").gameObject;
        myActionBar.GetComponent<CanvasGroup>().alpha = 1.0f;
        myActionButtons = new GameObject[playerClass.mySpellSize];
    }

    public void SetSpellCooldownText(int anIndex, float aDuration)
    {
        if (myActionButtons[anIndex] == null)
            return;

        if (aDuration > 0.0f)
        {
            myActionButtons[anIndex].GetComponentInChildren<Text>().text = aDuration.ToString("0.0");
        }
        else
        {
            myActionButtons[anIndex].GetComponent<ActionKey>().SetCooldown(0.0f);
        }
    }

    public void SpellPressed(int anIndex)
    {
        myActionButtons[anIndex].GetComponent<ActionKey>().SpellPressed();
    }

    public void SpellHeldDown(int anIndex)
    {
        myActionButtons[anIndex].GetComponent<ActionKey>().SpellHeldDown();
    }

    public void SpellReleased(int anIndex)
    {
        myActionButtons[anIndex].GetComponent<ActionKey>().SpellReleased();
    }

    public void HightlightHealTargeting(int anIndex, bool aShouldPulsate)
    {
        myActionButtons[anIndex].GetComponent<ActionKey>().SetPulsation(aShouldPulsate);
    }

    public void ToggleSpellInfo()
    {
        for (int index = 0; index < myActionButtons.Length; index++)
        {
            myActionButtons[index].GetComponent<ActionKey>().ToggleInfo();
        }
    }

    public void SetSpellHud(Spell aSpell, int anIndex)
    {
        myActionButtons[anIndex] = myActionBar.transform.GetChild(anIndex).gameObject;
        myActionButtons[anIndex].GetComponent<Image>().sprite = aSpell.mySpellIcon;
        myActionButtons[anIndex].GetComponent<ActionKey>().SetSpellInfo(aSpell.myQuickInfo);
    }

    public void HighlightSpellError(SpellErrorHandler.SpellError aSpellError)
    {
        mySpellErrorHandler.HighLightError(aSpellError);
    }
}
