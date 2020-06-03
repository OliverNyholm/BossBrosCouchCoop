using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Class : MonoBehaviour
{
    public string myClassName;
    public ClassRole myClassRole;

    [Header("Image to show on hud")]
    public Sprite mySprite;

    public GameObject[] mySpells;
    public float[] myCooldownTimers;

    [HideInInspector]
    public int mySpellSize = 4;

    private PlayerUIComponent myUIComponent;

    public enum ClassRole
    {
        MeleeDps,
        RangedDps,
        Healer,
        Tank
    }

    public void Awake()
    {
        myUIComponent = GetComponent<PlayerUIComponent>();
    }

    public void Start()
    {
        PoolManager poolManager = PoolManager.Instance;

        for (int index = 0; index < mySpells.Length; index++)
        {
            if (mySpells[index] == null)
            {
                myUIComponent.SetSpellHud(null, index);
                continue;
            }

            Spell spell = mySpells[index].GetComponent<Spell>();
            spell.CreatePooledObjects(poolManager, 3);

            myUIComponent.SetSpellHud(spell, index);
        }
    }

    private void Update()
    {
        for (int index = 0; index < myCooldownTimers.Length; index++)
        {
            if (myCooldownTimers[index] > 0.0f)
            {
                myCooldownTimers[index] -= Time.deltaTime;
                myUIComponent.SetSpellCooldownText(index, myCooldownTimers[index]);
            }
        }
    }

    public bool IsSpellCastOnFriends(int anIndex)
    {
        return mySpells[anIndex].GetComponent<Spell>().IsCastOnFriends();
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
        myUIComponent.SetSpellCooldownText(anIndex, myCooldownTimers[anIndex]);
    }

    public bool HasSpell(int aIndex)
    {
        return mySpells[aIndex] != null;
    }

    public void ReplaceSpell(GameObject aSpell, int aSpellIndex)
    {
        mySpells[aSpellIndex] = aSpell;
        myCooldownTimers[aSpellIndex] = 0.0f;
        myUIComponent.SetSpellCooldownText(aSpellIndex, 0.0f);

        if (aSpell != null)
            myUIComponent.SetSpellHud(aSpell.GetComponent<Spell>(), aSpellIndex);
        else
            myUIComponent.SetSpellHud(null, aSpellIndex);
    }
}
