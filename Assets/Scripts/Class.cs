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
    Taunt,
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

    [Header("Image to show on hud")]
    public Sprite mySprite;

    [SerializeField]
    private GameObject myAutoAttack;
    public GameObject[] mySpells;

    private GameObject myActionBar;
    private GameObject[] myActionButtons;

    public float[] myCooldownTimers;

    private float myResource;

    public Class()
    {
        const int mySpellSize = 8;
        mySpells = new GameObject[mySpellSize];
        myActionButtons = new GameObject[mySpellSize];
        myCooldownTimers = new float[mySpellSize];

        for (int index = 0; index < myCooldownTimers.Length; index++)
        {
            myCooldownTimers[index] = 0.0f;
        }
    }

    private void FindActionBar(Transform aUIParent)
    {
        myActionBar = aUIParent.Find("ActionBar").gameObject;
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
    public void SetupSpellHud(ActionClick anActionClickFunction, Transform aUIParent)
    {
        FindActionBar(aUIParent);

        for (int index = 0; index < myActionButtons.Length; index++)
        {
            int tempIndex = index;

            string name = "ActionButton" + (tempIndex + 1).ToString();
            for (int childIndex = 0; childIndex < myActionBar.transform.childCount; childIndex++)
            {
                if (myActionBar.transform.GetChild(childIndex).name == name)
                {
                    myActionButtons[index] = myActionBar.transform.GetChild(childIndex).gameObject;
                    myActionButtons[index].GetComponent<Image>().sprite = mySpells[index].GetComponent<Spell>().mySpellIcon;
                    break;
                }
            }

            myActionButtons[tempIndex].GetComponent<Button>().onClick.AddListener(delegate { anActionClickFunction(tempIndex); });
        }
    }
}
