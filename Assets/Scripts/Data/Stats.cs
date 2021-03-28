using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour
{
    public float mySpeedMultiplier = 1.0f;
    public float myAttackSpeed = 1.0f;
    public float myDamageIncrease = 1.0f;
    public float myDamageMitigator = 1.0f;
    public float myAutoAttackDamageMitigator = 1.0f;
    public float myAutoAttackRange = 5;
    public float myNextSpellModifier = 1.0f;
    public int myAutoAttackDamage = 20;

    [System.Serializable]
    public struct RangeCylinder
    {
        public float myRadius;
        public float myHeight;
        public bool myDrawGizmos;

        public RangeCylinder(float aRadius, float aHeight)
        {
            myRadius = aRadius;
            myHeight = aHeight;
            myDrawGizmos = true;
        }
    };

    public RangeCylinder myRangeCylinder = new RangeCylinder(1.5f, 5.0f);

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
    public float NextSpellModifier { get { return myNextSpellModifier; } set { myNextSpellModifier = value; } }
    public float DamageIncrease { get { return myDamageIncrease; } set { myDamageIncrease = value; } }

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
    }

    public void RemoveSpellOverTime(SpellOverTime aSpell)
    {
        int index = FindIndexOfSpellOverTime(aSpell);
        if (index == -1)
            return;

        mySpellOverTimeGOs.RemoveAt(index);
    }

    public void RemoveSpellOverTimeStack(SpellOverTime aSpell)
    {
        int index = FindIndexOfSpellOverTime(aSpell);
        if (index == -1)
            return;

        mySpellOverTimeGOs[index].RemoveStack();
    }

    public SpellOverTime GetSpellOverTimeIfExists(SpellOverTime aSpellOverTime)
    {
        int index = FindIndexOfSpellOverTime(aSpellOverTime);
        if (index == -1)
            return null;

        return mySpellOverTimeGOs[index];
    }

    public bool HasSpellOverTime(SpellOverTime aSpell)
    {
        if (aSpell == null)
            return false;

        return FindIndexOfSpellOverTime(aSpell) != -1;
    }

    public bool HasMaxSpellOverTimeStackCount(SpellOverTime aSpell)
    {
        SpellOverTime spell = GetSpellOverTimeIfExists(aSpell);
        if (!spell)
            return false;

        return spell.HasMaxStackCount();
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

    public float GetAndResetSpellModifier()
    {
        float spellModifier = myNextSpellModifier;
        myNextSpellModifier = 1.0f;

        return spellModifier;
    }

    protected virtual void OnDeath()
    {
        while (mySpellOverTimeGOs.Count > 0)
        {
            mySpellOverTimeGOs[mySpellOverTimeGOs.Count - 1].RemoveSpellOverTime(true);
        }
    }

    public bool IsInRange(Vector3 aOtherPosition, float aRange)
    {
        float heightDifference = Mathf.Abs(transform.position.y - aOtherPosition.y);
        float maxHeightRange = aRange + myRangeCylinder.myHeight * 0.5f;
        if (heightDifference > maxHeightRange)
            return false;

        VectorXZ planarToSelf = new VectorXZ(transform.position - aOtherPosition);
        float distanceSqr = planarToSelf.LengthSqr();
        float maxRangeSqr = (aRange * aRange) + (myRangeCylinder.myRadius * myRangeCylinder.myRadius);
        if (distanceSqr > maxRangeSqr)
            return false;

        return true;
    }

    private void OnDrawGizmosSelected()
    {
        if (!myRangeCylinder.myDrawGizmos)
            return;

        const int circlePrecision = 10;
        float rotationPerIndex = 6.28f / circlePrecision;
        Vector3 previousTopPosition = Vector3.zero;
        Vector3 previousBottomPosition = Vector3.zero;
        Vector3 firstTopPosition = Vector3.zero;
        Vector3 firstBottomPosition = Vector3.zero;
        for (int index = 0; index < circlePrecision; index++)
        {
            Vector3 topPosition = transform.position;
            topPosition.y += myRangeCylinder.myHeight * 0.5f;

            float x = Mathf.Cos(rotationPerIndex * index);
            float z = Mathf.Sin(rotationPerIndex * index);

            topPosition.x += myRangeCylinder.myRadius * x;
            topPosition.z += myRangeCylinder.myRadius * z;

            Vector3 bottomPosition = topPosition;
            bottomPosition.y -= myRangeCylinder.myHeight;

            if (index == 0)
            {
                firstTopPosition = topPosition;
                firstBottomPosition = bottomPosition;
            }
            else
            {
                Gizmos.DrawLine(topPosition, previousTopPosition);
                Gizmos.DrawLine(bottomPosition, previousBottomPosition);
            }

            if(index == circlePrecision - 1)
            {
                Gizmos.DrawLine(topPosition, firstTopPosition);
                Gizmos.DrawLine(bottomPosition, firstBottomPosition);
            }

            Gizmos.DrawLine(topPosition, bottomPosition);

            previousTopPosition = topPosition;
            previousBottomPosition = bottomPosition;
        }
    }
}