using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour
{
    public float mySpeedMultiplier = 1.0f;
    public float myAttackSpeed = 1.0f;
    public float myDamageIncrease = 1.0f;
    public float myDamageMitigator = 1.0f;
    public float myAutoAttackRange = 5;
    public int myAutoAttackDamage = 20;

    public bool myCanBeStunned = true;
    private bool myIsStunned = false;
    private float myStunDuration = 0.0f;

    private List<SpellOverTime> mySpellOverTimeGOs;

    private void Awake()
    {
        mySpellOverTimeGOs = new List<SpellOverTime>(8);
        GetComponent<Health>().EventOnHealthZero += OnDeath;
    }

    public float AttackRange { get { return myAutoAttackRange; } }
    public bool IsStunned() { return myIsStunned; }
    public void SetStunned(float aDuration)
    {
        myStunDuration = aDuration;
        if (myStunDuration > 0.0f)
        {
            myIsStunned = true;
        }
        else
            myIsStunned = false;
    }

    public void Update()
    {
        if (myStunDuration > 0.0f)
        {
            myStunDuration -= Time.deltaTime;
            if (myStunDuration <= 0.0f)
                myIsStunned = false;
        }
    }

    public void AddSpellOverTime(SpellOverTime aSpell)
    {
        mySpellOverTimeGOs.Add(aSpell);
        GetComponent<UIComponent>().AddBuff(aSpell.mySpellIcon);
    }

    public void RemoveSpellOverTime(SpellOverTime aSpell)
    {
        int index = FindIndexOfSpellOverTime(aSpell);
        mySpellOverTimeGOs.RemoveAt(index);
        GetComponent<UIComponent>().RemoveBuff(index);
    }

    public void RemoveSpellOverTimeIfExists(SpellOverTime aSpell)
    {
        int index = FindIndexOfSpellOverTime(aSpell);
        if (index == -1)
            return;

        mySpellOverTimeGOs[index].RemoveSpellOverTime();
    }

    public bool HasSpellOverTime(SpellOverTime aSpell)
    {
        if (aSpell == null)
            return false;

        return FindIndexOfSpellOverTime(aSpell) != -1;
    }

    private int FindIndexOfSpellOverTime(SpellOverTime aSpell)
    {
        uint spellID = aSpell.GetComponent<UniqueID>().GetID();
        for (int index = 0; index < mySpellOverTimeGOs.Count; index++)
        {
            if (mySpellOverTimeGOs[index].GetComponent<UniqueID>().GetID() == spellID)
            {
                return index;
            }
        }

        return -1;
    }

    protected virtual void OnDeath()
    {
        while (mySpellOverTimeGOs.Count > 0)
        {
            RemoveSpellOverTime(mySpellOverTimeGOs[mySpellOverTimeGOs.Count - 1]);
        }
    }
}