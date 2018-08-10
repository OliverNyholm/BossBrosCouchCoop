using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum SpellType
{
    Damage,
    DOT,
    Heal,
    HOT,
    Shield,
    Interrupt,
    Silence,
    Slow,
    Buff,
    Ressurect
}


public class Class : MonoBehaviour
{

    public string myClassName;
    public GameObject[] mySpells;

    private GameObject myActionBar;
    private GameObject[] myActionButtons;

    public float[] myCooldownTimers;

    private float myResource;

    public Class()
    {
        mySpells = new GameObject[8];
        myActionButtons = new GameObject[8];
        myCooldownTimers = new float[8];

        for (int index = 0; index < myCooldownTimers.Length; index++)
        {
            myCooldownTimers[index] = 0.0f;
        }
    }

    private void FindActionBar()
    {
        myActionBar = GameObject.Find("ActionBar");
    }

    private void Update()
    {
        for (int index = 0; index < myCooldownTimers.Length; index++)
        {
            if (myCooldownTimers[index] > 0.0f)
            {
                myCooldownTimers[index] -= Time.deltaTime;
                if (myCooldownTimers[index] > 0.0f)
                {
                    myActionButtons[index].GetComponentInChildren<Text>().text = myCooldownTimers[index].ToString("0.0");
                }
                else
                {
                    myActionButtons[index].GetComponentInChildren<Text>().text = "";
                    myActionButtons[index].GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                }
            }
        }
    }

    public bool IsSpellOnCooldown(int anIndex)
    {
        if (myCooldownTimers[anIndex] > 0.0f)
            return true;

        return false;
    }

    public GameObject GetSpell(int anIndex)
    {
        return mySpells[anIndex];
    }

    public void SetSpellOnCooldown(int anIndex)
    {
        myCooldownTimers[anIndex] = mySpells[anIndex].GetComponent<Spell>().myCooldown;

        myActionButtons[anIndex].GetComponentInChildren<Text>().text = myCooldownTimers[anIndex].ToString("0.0");
        myActionButtons[anIndex].GetComponent<Image>().color = new Color(0.8f, 0.3f, 0.3f, 0.5f);
    }

    public delegate void ActionClick(int anIndex);
    public void SetupSpellHud(ActionClick anActionClickFunction)
    {
        FindActionBar();

        for (int index = 0; index < myActionButtons.Length; index++)
        {
            int tempIndex = index;

            string name = "ActionButton" + (tempIndex + 1).ToString();
            for (int childIndex = 0; childIndex < myActionBar.transform.childCount; childIndex++)
            {
                if (myActionBar.transform.GetChild(childIndex).name == name)
                {
                    myActionButtons[index] = myActionBar.transform.GetChild(childIndex).gameObject;
                    break;
                }
            }

            myActionButtons[tempIndex].GetComponent<Button>().onClick.AddListener(delegate { anActionClickFunction(tempIndex); });
        }
    }
}
