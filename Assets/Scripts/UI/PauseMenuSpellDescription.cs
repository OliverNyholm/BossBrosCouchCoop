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
        mySpellImage.sprite = aSpell.mySpellIcon;
        mySpellName.text = aSpell.name;
        mySpellDescription.text = aSpell.myTutorialInfo;
    }
}
