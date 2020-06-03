using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PausePlayerUI : MonoBehaviour
{
    [Header("Class Icon to display")]
    [SerializeField]
    private Image myClassIcon = null;

    [Header("Class Name to display")]
    [SerializeField]
    private Text myClassName = null;

    [Header("Gameobject that holds spells")]
    [SerializeField]
    private List<PauseMenuSpellDescription> mySpellDescriptions = new List<PauseMenuSpellDescription>();

    public void SetClassDetails(Class aClass)
    {
        myClassIcon.sprite = aClass.mySprite;
        myClassName.text = aClass.myClassName;

        for (int index = 0; index < mySpellDescriptions.Count; index++)
        {
            GameObject spell = aClass.mySpells[index];
            if (!spell)
                continue;

            mySpellDescriptions[index].SetSpellDetails(spell.GetComponent<Spell>());
        }
    }
}
