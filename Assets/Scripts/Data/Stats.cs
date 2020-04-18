﻿using System.Collections.Generic;
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
        Health health = GetComponent<Health>();
        if (health)
            health.EventOnHealthZero += OnDeath;
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

    public float CalculateBuffSmartDamage()
    {
        int damageBuffCount = 0;
        float score = 0.0f;
        foreach (SpellOverTime spell in mySpellOverTimeGOs)
        {
            if (!UtilityFunctions.HasSpellType(spell.mySpellOverTimeType, SpellOverTimeType.DOT))
                continue;

            score += spell.CalculateRemainingDamage() * 0.05f;
            const float madeUpMaxTime = 5.0f;
            score += Mathf.Abs(madeUpMaxTime - spell.TimeUntilNextTick()) + spell.GetTickValue() * 0.1f;

            damageBuffCount++;
        }

        return score + damageBuffCount;
    }

    protected virtual void OnDeath()
    {
        while (mySpellOverTimeGOs.Count > 0)
        {
            mySpellOverTimeGOs[mySpellOverTimeGOs.Count - 1].RemoveSpellOverTime();
        }
    }
}