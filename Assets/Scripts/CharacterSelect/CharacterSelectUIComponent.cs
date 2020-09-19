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

    public void SetSpellHud(Spell aSpell, Color aClassColor, int anIndex)
    {
        myActionButtons[anIndex].GetComponent<Image>().sprite = aSpell.mySpellIcon;

        ActionKey actionKey = myActionButtons[anIndex].GetComponent<ActionKey>();
        actionKey.SetSpellName(aSpell.myName);
        actionKey.SetSpellInfo(aSpell.myTutorialInfo);
        actionKey.SetTextColor(aClassColor);
    }

    public void SetCharacterColor(Color aColor)
    {
        myCharacterColor = aColor;
    }
}
