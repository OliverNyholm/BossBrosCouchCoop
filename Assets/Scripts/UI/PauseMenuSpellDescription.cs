using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuSpellDescription : MonoBehaviour
{
    [Header("The UI components to explain details about the spell")]
    [SerializeField]
    private Image mySpellImage = null;
    [SerializeField]
    private Text mySpellName = null;
    [SerializeField]
    private Text mySpellDescription = null;

    public void SetSpellDetails(Spell aSpell)
    {
        if(!aSpell)
        {
            mySpellImage.GetComponent<Image>().enabled = false;
            mySpellName.text = "No Spell";
            mySpellDescription.text = "";
            return;
        }

        mySpellImage.GetComponent<Image>().enabled = true;
        mySpellImage.sprite = aSpell.mySpellIcon;
        mySpellName.text = aSpell.name;
        mySpellDescription.text = aSpell.myTutorialInfo;
    }
}
