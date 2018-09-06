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
    Ressurect,
    Special
}

[System.Flags]
public enum SpellTarget
{
    Friend = 1 << 1,
    Enemy = 1 << 2,
    Anyone = Friend | Enemy
}


public class Class : MonoBehaviour
{

    public string myClassName;

    [SerializeField]
    private GameObject myAutoAttack;
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
        myActionBar.GetComponent<CanvasGroup>().alpha = 1.0f;
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

    public GameObject GetAutoAttack()
    {
        return myAutoAttack;
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
                    myActionButtons[index].GetComponent<Image>().sprite = mySpells[index].GetComponent<Spell>().myCastbarIcon;
                    myActionButtons[index].GetComponent<ActionKey>().SetDescription(GetSpellDescription(mySpells[index].GetComponent<Spell>()));
                    break;
                }
            }

            myActionButtons[tempIndex].GetComponent<Button>().onClick.AddListener(delegate { anActionClickFunction(tempIndex); });
        }
    }

    private string GetSpellDescription(Spell aSpell)
    {
        string target = "Cast spell on ";
        if (aSpell.myIsOnlySelfCast)
            target += "self ";
        else
            target += aSpell.GetSpellTarget().ToString() + " ";

        string detail = "to ";
        switch (aSpell.mySpellType)
        {
            case SpellType.Damage:
                detail += "deal " + aSpell.myDamage + " damage. ";
                break;
            case SpellType.Heal:
                break;
            case SpellType.Interrupt:
                break;
            case SpellType.Silence:
                break;
            case SpellType.Slow:
                break;
            case SpellType.Buff:
                if (aSpell.myBuff.mySpellType == SpellType.DOT)
                    detail += "deal " + (aSpell.myBuff as TickBuff).myTotalDamage + " damage over " + (aSpell.myBuff as TickBuff).myDuration.ToString("0") + " seconds. ";
                break;
            case SpellType.Ressurect:
                break;
            case SpellType.Special:
                detail += "deal something... ";
                break;
            default:
                detail += "deal something... ";
                break;
        }

        string cost = "Costs " + aSpell.myResourceCost + " to cast spell. ";

        string castTime = "Spell ";
        if (aSpell.myCastTime <= 0.0f)
        {
            castTime += "is instant cast.";
        }
        else
        {
            castTime += "takes " + aSpell.myCastTime + " to cast. ";
            if (aSpell.myIsCastableWhileMoving)
                castTime += "Is castable while moving.";
            else
                castTime += "Is not castable while moving";
        }

        return target + detail + cost + castTime;
    }
}
