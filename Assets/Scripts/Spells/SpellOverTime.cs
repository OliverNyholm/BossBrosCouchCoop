using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpellOverTime : Spell
{
    [Tooltip("Set to true if the object is cast, and not spawned by another spell")]
    [HideInInspector] public bool myIsCastManually = false;

    [HideInInspector] public int myMaxStackCount = 1;
    [HideInInspector] public bool myLoseStacksOneByOne = false;
    private int myCurrentStackCount = 0;

    [HideInInspector] public float myDuration = 6;
    [HideInInspector] public SpellOverTimeType mySpellOverTimeType;
    //Buffs
    [HideInInspector] public float mySpeedMultiplier = 0;
    [HideInInspector] public float myAttackSpeed = 0;
    [HideInInspector] public float myDamageIncrease = 0;
    [HideInInspector] public float myDamageMitigator = 0;
    //Shield
    [HideInInspector] public int myShieldValue = 0;
    private int myCurrentShieldValue;
    //HoT and DoT
    [HideInInspector] public int myNumberOfTicks = 3;
    private int myTickDamage;
    private float myInterval;
    private float myIntervalTimer;

    private float myLifeTimeLeft;
    private bool myHasReachedTarget;

    private GameObject myBuffWidget = null;

    protected override void Awake()
    {
        base.Awake();
        myReturnToPoolWhenReachedTarget = false;
    }

    public override void Reset()
    {
        base.Reset();
        ResetBuff();
        myHasReachedTarget = false;
        myCurrentStackCount = 0;
        myBuffWidget = null;

        if (UtilityFunctions.HasSpellType(mySpellOverTimeType, SpellOverTimeType.DOT | SpellOverTimeType.HOT))
        {
            myTickDamage = myDamage / myNumberOfTicks;
            myInterval = myDuration / myNumberOfTicks;
            myIntervalTimer = 0.0f;
        }
    }

    private void ResetBuff()
    {
        myLifeTimeLeft = myDuration;
        myCurrentShieldValue = myShieldValue;
    }

    protected override void Update()
    {
        if (!myHasReachedTarget)
        {
            base.Update();
            return;
        }

        if (myLifeTimeLeft >= 0)
        {
            myLifeTimeLeft -= Time.deltaTime;
            if (myLifeTimeLeft <= 0.0f)
            {
                myLifeTimeLeft = myDuration;
                RemoveStack();
                return;
            }
        }

        if (UtilityFunctions.HasSpellType(mySpellOverTimeType, SpellOverTimeType.Shield))
        {
            if (myCurrentShieldValue <= 0)
            {
                RemoveStack();
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

        SpellOverTime existingBuff = stats.GetSpellOverTimeIfExists(this);
        if (existingBuff)
        {
            existingBuff.AddAnotherStack();
            ReturnToPool();
        }
        else
        {
            myCurrentStackCount = 1;
            ApplyBuffEffects(stats);
            stats.AddSpellOverTime(this);

            UIComponent uiComponent = myTarget.GetComponent<UIComponent>();
            if (uiComponent)
                myBuffWidget = uiComponent.AddBuffAndGetUIRef(this);
        }
    }

    public void AddAnotherStack()
    {
        Stats stats = myTarget.GetComponent<Stats>();
        RemoveBuffEffects(stats);
        myCurrentStackCount = Mathf.Min(myCurrentStackCount + 1, myMaxStackCount);
        ResetBuff();
        ApplyBuffEffects(stats);

        SetStackCountOnUI();
    }

    private void ApplyBuffEffects(Stats aStats)
    {
        if (UtilityFunctions.HasSpellType(mySpellOverTimeType, SpellOverTimeType.Stats))
            SetStatsEffect(aStats, myCurrentStackCount);

        if (myCurrentStackCount == 1 || IsStackable()) //We can add shield if this was the first or if we can stack them
        {
            if (UtilityFunctions.HasSpellType(mySpellOverTimeType, SpellOverTimeType.Shield))
                myTarget.GetComponent<Health>().AddShield(this);
        }
    }

    private void RemoveBuffEffects(Stats aStats)
    {
        if (UtilityFunctions.HasSpellType(mySpellOverTimeType, SpellOverTimeType.Stats))
            SetStatsEffect(aStats, -myCurrentStackCount);

        if (UtilityFunctions.HasSpellType(mySpellOverTimeType, SpellOverTimeType.Shield))
            myTarget.GetComponent<Health>().RemoveShield(this);

        //Hmm... this is questionable
        //if (UtilityFunctions.HasSpellType(mySpellOverTimeType, SpellOverTimeType.DOT | SpellOverTimeType.HOT))
        //{
        //    if (!myTarget.GetComponent<Health>().IsDead())
        //        DealTickEffect();
        //}
    }

    private void RemoveStack()
    {
        if (!myLoseStacksOneByOne || myCurrentStackCount == 1)
        {
            RemoveSpellOverTime(true);
        }
        else
        {
            Stats stats = myTarget.GetComponent<Stats>();
            RemoveBuffEffects(stats);
            myCurrentStackCount = Mathf.Max(myCurrentStackCount - 1, 0);
            ResetBuff();
            ApplyBuffEffects(stats);

            SetStackCountOnUI();
        }
    }

    public void RemoveSpellOverTime(bool aForceRemove)
    {
        if (myCurrentStackCount - 1 == 0 || aForceRemove)
        {
            Stats stats = myTarget.GetComponent<Stats>();
            RemoveBuffEffects(stats);
            stats.RemoveSpellOverTime(this);

            UIComponent uiComponent = myTarget.GetComponent<UIComponent>();
            if (uiComponent && myBuffWidget)
                uiComponent.RemoveBuff(myBuffWidget);

            ReturnToPool();            
        }
        else
        {
            RemoveStack();
        }
    }

    private void SetStatsEffect(Stats aStats, int aStatMultiplier)
    {
        aStats.mySpeedMultiplier += mySpeedMultiplier * aStatMultiplier;
        aStats.myAttackSpeed += myAttackSpeed * aStatMultiplier;
        aStats.myDamageIncrease += myDamageIncrease * aStatMultiplier;
        aStats.myDamageMitigator += myDamageMitigator * aStatMultiplier;

        if (myAttackSpeed != 0.0f)
        {
            float attackspeed = myTarget.GetComponent<CastingComponent>().myAutoAttackCooldownReset / aStats.myAttackSpeed;

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

        NPCMovementComponent npcMovementComponent = aStats.GetComponent<NPCMovementComponent>();
        if (npcMovementComponent)
            npcMovementComponent.ModifySpeed(aStats.mySpeedMultiplier);
    }

    private void SetStackCountOnUI()
    {
        if (myBuffWidget)
            myBuffWidget.GetComponentInChildren<TextMeshProUGUI>().text = myCurrentStackCount > 1 ? myCurrentStackCount.ToString() : "";
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
        int ticksLeft = (int)((myDuration - myLifeTimeLeft) / myIntervalTimer);
        return ticksLeft * myTickDamage;
    }

    protected virtual void DealTickEffect()
    {
        if (UtilityFunctions.HasSpellType(mySpellOverTimeType, SpellOverTimeType.DOT))
        {
            myTarget.GetComponent<Health>().TakeDamage(myTickDamage, myParent.GetComponent<UIComponent>().myCharacterColor, myTarget.transform.position);
            if (myParent.GetComponent<Player>() != null)
                PostMaster.Instance.PostMessage(new Message(MessageCategory.DamageDealt, new Vector2(myParent.GetInstanceID(), myTickDamage)));
        }
        else
        {
            myTarget.GetComponent<Health>().GainHealth(myTickDamage);
        }
    }

    public int GetTickValue()
    {
        return myTickDamage;
    }

    public float TimeUntilNextTick()
    {
        return myInterval - myIntervalTimer;
    }

    public override bool IsCastOnFriends()
    {
        if (base.IsCastOnFriends())
            return true;

        return UtilityFunctions.HasSpellType(mySpellOverTimeType, SpellOverTimeType.HOT | SpellOverTimeType.Shield | SpellOverTimeType.Stats);
    }

    public bool IsStackable()
    {
        return myMaxStackCount > 1;
    }

    public int GetStackCount()
    {
        return myCurrentStackCount;
    }
}
