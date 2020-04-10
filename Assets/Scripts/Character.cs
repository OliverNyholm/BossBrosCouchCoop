using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    public float myBaseSpeed;
    public float myJumpSpeed;
    public float myGravity;

    protected Animator myAnimator;

    protected Health myHealth;
    protected Resource myResource;
    protected Stats myStats;
    protected Class myClass;


    protected List<BuffSpell> myBuffs;

    protected float myStunDuration;

    protected virtual void Awake()
    {
        if (!myAnimator)
            SetComponents();

    }

    private void SetComponents()
    {
        myAnimator = GetComponent<Animator>();

        myHealth = GetComponent<Health>();
        myResource = GetComponent<Resource>();
        myStats = GetComponent<Stats>();
        myClass = GetComponent<Class>();

        myBuffs = new List<BuffSpell>();

        myHealth.EventOnHealthZero += OnDeath;
    }

    protected abstract bool IsMoving();

    protected virtual void Update()
    {
        HandleBuffs();

        //Gör en kommentar ändå, puss på dig Oliver <3. Saknar dig.
        if (myAnimator)
            myAnimator.SetLayerWeight(1, 1);
    }

    public bool CanRaycastToObject(GameObject anObject)
    {
        Vector3 hardcodedEyePosition = new Vector3(0.0f, 0.7f, 0.0f);
        Vector3 infrontOfPlayer = (transform.position + hardcodedEyePosition) + transform.forward;
        Vector3 direction = (anObject.transform.position + hardcodedEyePosition) - infrontOfPlayer;

        Ray ray = new Ray(infrontOfPlayer, direction);
        float distance = Vector3.Distance(infrontOfPlayer, anObject.transform.position + hardcodedEyePosition);
        LayerMask layerMask = LayerMask.GetMask("Terrain");

        if (Physics.Raycast(ray, distance, layerMask))
        {
            return false;
        }

        return true;
    }

    protected void HandleBuffs()
    {
        for (int index = 0; index < myBuffs.Count; index++)
        {
            myBuffs[index].Tick();
            if (myBuffs[index].IsFinished())
            {
                RemoveBuff(index);
            }
            else if (myBuffs[index].GetBuff().mySpellType == SpellType.HOT)
            {
                BuffTickSpell hot = myBuffs[index] as BuffTickSpell;
                if (hot.ShouldDealTickSpellEffect)
                {
                    myHealth.GainHealth(hot.GetTickValue());
                    hot.ShouldDealTickSpellEffect = false;
                }
            }
            else if (myBuffs[index].GetBuff().mySpellType == SpellType.DOT)
            {
                BuffTickSpell dot = myBuffs[index] as BuffTickSpell;
                if (dot.ShouldDealTickSpellEffect)
                {
                    myHealth.TakeDamage(dot.GetTickValue(), dot.GetParent().GetComponent<UIComponent>().myCharacterColor);
                    if (dot.GetParent().tag == "Player")
                        PostMaster.Instance.PostMessage(new Message(MessageCategory.DamageDealt, new Vector2(dot.GetParent().GetInstanceID(), dot.GetTickValue())));

                    dot.ShouldDealTickSpellEffect = false;
                }
            }
        }
    }

    public void AddBuff(BuffSpell aBuffSpell, Sprite aSpellIcon)
    {
        CheckAlreadyHasThatBuff(aBuffSpell, true);

        myBuffs.Add(aBuffSpell);
        aBuffSpell.GetBuff().ApplyBuff(ref myStats);
        GetComponent<UIComponent>().AddBuff(aSpellIcon);

        if (aBuffSpell.GetBuff().myAttackSpeed != 0.0f)
        {
            float attackspeed = GetComponent<CastingComponent>().myAutoAttackCooldownReset / myStats.myAttackSpeed;

            float currentAnimationSpeed = 1.0f;
            if (currentAnimationSpeed > attackspeed)
            {
                if (myAnimator)
                    myAnimator.SetFloat("AutoAttackSpeed", myAnimator.GetFloat("AutoAttackSpeed") + attackspeed / currentAnimationSpeed);
            }
        }
        else if (aBuffSpell.GetBuff().mySpeedMultiplier != 0.0f)
        {
            if (myAnimator)
                myAnimator.SetFloat("RunSpeed", myStats.mySpeedMultiplier);
        }
    }

    private void RemoveBuff(int anIndex)
    {
        myBuffs[anIndex].GetBuff().EndBuff(ref myStats);
        if (myBuffs[anIndex].GetBuff().mySpellType == SpellType.Shield)
        {
            //Ugly hack to set shield to 0, which will remove it at myHealth.RemoveShield()
            (myBuffs[anIndex] as BuffShieldSpell).SoakDamage((myBuffs[anIndex] as BuffShieldSpell).GetRemainingShieldHealth());
            myHealth.RemoveShield();
        }

        if (myBuffs[anIndex].GetBuff().myAttackSpeed != 0.0f)
        {
            float attackspeed = GetComponent<CastingComponent>().myAutoAttackCooldownReset / myStats.myAttackSpeed;

            float currentAnimationSpeed = 1.0f;
            if (currentAnimationSpeed > attackspeed)
            {
                if (myAnimator)
                    myAnimator.SetFloat("AutoAttackSpeed", myAnimator.GetFloat("AutoAttackSpeed") + attackspeed / currentAnimationSpeed);
            }
            else
            {
                if (myAnimator)
                    myAnimator.SetFloat("AutoAttackSpeed", 1.0f);
            }
        }
        else if (myBuffs[anIndex].GetBuff().mySpeedMultiplier != 0.0f)
        {
            if (myAnimator)
                myAnimator.SetFloat("RunSpeed", myStats.mySpeedMultiplier);
        }

        myBuffs.RemoveAt(anIndex);
        GetComponent<UIComponent>().RemoveBuff(anIndex);
    }

    public void RemoveBuffByName(string aName)
    {
        for (int index = 0; index < myBuffs.Count; index++)
        {
            if (myBuffs[index].GetName() == aName) //Make unique id for each spell and remove by id intead
            {
                RemoveBuff(index);
                return;
            }
        }
    }

    public bool CheckAlreadyHasThatBuff(BuffSpell aBuffSpell, bool aShouldRemoveOnDuplicate = false)
    {
        for (int index = 0; index < myBuffs.Count; index++)
        {
            if (myBuffs[index].GetParent() == aBuffSpell.GetParent() &&
                myBuffs[index].GetBuff() == aBuffSpell.GetBuff())
            {
                if (aShouldRemoveOnDuplicate)
                    RemoveBuff(index);
                return true;
            }
        }

        return false;
    }

    protected virtual void OnDeath()
    {
        while (myBuffs.Count > 0)
        {
            RemoveBuff(myBuffs.Count - 1);
        }

        AnimatorWrapper animatorWrapper = GetComponent<AnimatorWrapper>();
        if (animatorWrapper)
            animatorWrapper.SetTrigger(AnimationVariable.Death);
    }
}