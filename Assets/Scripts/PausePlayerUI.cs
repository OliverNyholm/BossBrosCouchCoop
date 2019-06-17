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
    private GameObject mySpellHolder = null;

    public void SetClassDetails(Class aClass)
    {
        myClassIcon.sprite = aClass.mySprite;
        myClassName.text = aClass.myClassName;

        for (int index = 0; index < mySpellHolder.transform.childCount; index++)
        {
            SetSpellDetails(mySpellHolder.transform.GetChild(index).gameObject, aClass.mySpells[index].GetComponent<Spell>());
        }
    }

    void SetSpellDetails(GameObject aSpellDescription, Spell aSpell)
    {
        aSpellDescription.GetComponentInChildren<Image>().sprite = aSpell.mySpellIcon;
        aSpellDescription.GetComponentInChildren<Text>().text = aSpell.GetSpellDescription();
    }
}
