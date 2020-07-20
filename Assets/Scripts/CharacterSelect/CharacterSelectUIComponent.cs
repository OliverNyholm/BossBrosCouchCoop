using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectUIComponent : PlayerUIComponent
{
    private CharacterSelector myCharacterSelector;

    protected override void Awake()
    {
        myActionButtons = new GameObject[GetComponent<Class>().mySpellSize];
    }        

    public void SetCharacterSelector(CharacterSelector aCharacterSelector)
    {
        myCharacterSelector = aCharacterSelector;

        List<GameObject> spells = myCharacterSelector.GetSpells();
        for (int index = 0; index < myActionButtons.Length; index++)
        {
            myActionButtons[index] = spells[index];
        }
    }

    private void Start()
    {
        myCastbar = GetComponentInChildren<Castbar>();
        myCastbar.GetComponent<Canvas>().worldCamera = Camera.main;
    }

    public override void SetSpellHud(Spell aSpell, int anIndex, bool aShouldHighlightSpell = false)
    {
        ActionKey actionKey = myActionButtons[anIndex].GetComponent<ActionKey>();
        if (aSpell == null)
        {
            myActionButtons[anIndex].GetComponent<Image>().sprite = myNoSpellSprite;
            actionKey.SetSpellName("None");
            actionKey.SetSpellInfo("No spell on this button");
            //myActionButtons[anIndex].GetComponent<ActionKey>().SetSpellInfo("");
        }
        else
        {
            myActionButtons[anIndex].GetComponent<Image>().sprite = aSpell.mySpellIcon;
            actionKey.SetSpellName(aSpell.myName);
            actionKey.SetSpellInfo(aSpell.myTutorialInfo);
            //myActionButtons[anIndex].GetComponent<ActionKey>().SetSpellInfo(aSpell.myQuickInfo);
        }
    }
}
