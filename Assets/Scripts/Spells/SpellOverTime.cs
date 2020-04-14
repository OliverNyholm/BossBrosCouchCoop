using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellOverTime : Spell
{
    [Tooltip("Set to true if the object is cast, and not spawned by another spell")]
    public bool myIsCastManually = false;

    public float myDuration = 6;
    public SpellOverTimeType mySpellOverTimeType;
    //Buffs
    public float mySpeedMultiplier = 0;
    public float myAttackSpeed = 0;
    public float myDamageIncrease = 0;
    public float myDamageMitigator = 0;
    //Shield
    public int myShieldValue = 0;
    private int myCurrentShieldValue;
    //HoT and DoT
    public int myNumberOfTicks = 3;
    private int myNumberOfTicksDealt;
    private int myTickDamage;
    private float myInterval;
    private float myIntervalTimer;

    private float myLifeTime;
    private bool myHasReachedTarget;

    private void Awake()
    {
        myReturnToPoolWhenReachedTarget = false;
    }

    public override void Reset()
    {
        base.Reset();
        myLifeTime = myDuration;
        myCurrentShieldValue = myShieldValue;
        myHasReachedTarget = false;

        if (UtilityFunctions.HasSpellType(mySpellOverTimeType, SpellOverTimeType.DOT | SpellOverTimeType.HOT))
        {
            myNumberOfTicksDealt = 0;
            myTickDamage = myDamage / myNumberOfTicks;
            myInterval = myDuration / myNumberOfTicks;
            myIntervalTimer = 0.0f;
        }
    }

    protected override void Update()
    {
        if (!myHasReachedTarget)
        {
            base.Update();
            return;
        }

        myLifeTime -= Time.deltaTime;
        if(myLifeTime <= 0.0f)
        {
            RemoveSpellOverTime();
            return;
        }

        if (UtilityFunctions.HasSpellType(mySpellOverTimeType, SpellOverTimeType.Shield))
        {
            if (myCurrentShieldValue <= 0)
            {
                RemoveSpellOverTime();
                return;
            }
        }

        if (UtilityFunctions.HasSpellType(mySpellOverTimeType, SpellOverTimeType.DOT | SpellOverTimeType.HOT))
        {
            myIntervalTimer += Time.deltaTime;

            if (myIntervalTimer >= myInterval)
            {
                myIntervalTimer -= myInterval;
                DealTickEffect();
            }
        }
    }

    protected override void OnReachTarget()
    {
        base.OnReachTarget();

        transform.parent = myTarget.transform;

        myHasReachedTarget = true;
        SetOverTimeEffects();
    }

    private void SetOverTimeEffects()
    {
        Stats stats = myTarget.GetComponent<Stats>();
        stats.RemoveSpellOverTimeIfExists(this);

        stats.AddSpellOverTime(this);

        if(UtilityFunctions.HasSpellType(mySpellOverTimeType, SpellOverTimeType.Stats))
            SetStatsEffect(stats, true);

        if (UtilityFunctions.HasSpellType(mySpellOverTimeType, SpellOverTimeType.Shield))
            myTarget.GetComponent<Health>().AddShield(this);
    }

    private void RemoveOverTimeEffects()
    {
        Stats stats = myTarget.GetComponent<Stats>();
        stats.RemoveSpellOverTime(this);

        if (UtilityFunctions.HasSpellType(mySpellOverTimeType, SpellOverTimeType.Stats))
            SetStatsEffect(stats, false);

        if (UtilityFunctions.HasSpellType(mySpellOverTimeType, SpellOverTimeType.Shield))
            myTarget.GetComponent<Health>().RemoveShield(this);

        if (UtilityFunctions.HasSpellType(mySpellOverTimeType, SpellOverTimeType.DOT | SpellOverTimeType.HOT))
        {
            if (!myTarget.GetComponent<Health>().IsDead() && myNumberOfTicksDealt < myNumberOfTicks)
                DealTickEffect();
        }
    }

    public void RemoveSpellOverTime()
    {
        RemoveOverTimeEffects();
        ReturnToPool();
    }

    private void SetStatsEffect(Stats aStats, bool anIsApply)
    {
        int applyModifier = anIsApply ? 1 : -1; //If not applying -> removing, therefore negating the value from what it was before.
        aStats.mySpeedMultiplier += mySpeedMultiplier * applyModifier;
        aStats.myAttackSpeed += myAttackSpeed * applyModifier;
        aStats.myDamageIncrease += myDamageIncrease * applyModifier;
        aStats.myDamageMitigator += myDamageMitigator * applyModifier;

        if (myAttackSpeed != 0.0f)
        {
            float attackspeed = GetComponent<CastingComponent>().myAutoAttackCooldownReset / aStats.myAttackSpeed;

            float currentAnimationSpeed = 1.0f;
            if (currentAnimationSpeed > attackspeed)
            {
                AnimatorWrapper animatorWrapper = myTarget.GetComponent<AnimatorWrapper>();
                if (animatorWrapper)
                    animatorWrapper.SetFloat(AnimationVariable.AutoAttackSpeed, animatorWrapper.GetFloat(AnimationVariable.AutoAttackSpeed) + attackspeed / currentAnimationSpeed);
            }
        }
        else if (mySpeedMultiplier != 0.0f)
        {
                AnimatorWrapper animatorWrapper = myTarget.GetComponent<AnimatorWrapper>();
            if (animatorWrapper)
                animatorWrapper.SetFloat(AnimationVariable.RunSpeed, aStats.mySpeedMultiplier);
        }
    }

    public int SoakDamage(int aDamage)
    {
        myCurrentShieldValue -= aDamage;

        if (myCurrentShieldValue > 0)
            return 0;

        return Mathf.Abs(myCurrentShieldValue);
    }

    public int GetRemainingShieldHealth()
    {
        return myCurrentShieldValue;
    }

    public int CalculateRemainingDamage()
    {
        int ticksLeft = (int)((myBuff.myDuration - myDuration) / myIntervalTimer);
        return ticksLeft * myTickDamage;
    }

    private void DealTickEffect()
    {
        myNumberOfTicksDealt++;
        if (UtilityFunctions.HasSpellType(mySpellOverTimeType, SpellOverTimeType.DOT))
        {
            myTarget.GetComponent<Health>().TakeDamage(myTickDamage, myParent.GetComponent<UIComponent>().myCharacterColor);
            if (myParent.GetComponent<Player>() != null)
                PostMaster.Instance.PostMessage(new Message(MessageCategory.DamageDealt, new Vector2(myParent.GetInstanceID(), myTickDamage)));
        }
        else
        {
            myTarget.GetComponent<Health>().GainHealth(myTickDamage);
        }
    }
}
