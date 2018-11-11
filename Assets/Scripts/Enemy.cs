using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Enemy : NetworkBehaviour
{

    public string myName;
    public float myDamage;

    public GameObject[] mySpells;

    public float mySpeed;
    private float myAutoAttackCooldown;
    private float myAutoAttackCooldownReset = 1.0f;

    private Health myHealth;
    private Stats myStats;
    private List<BuffSpell> myBuffs;

    private Animator myAnimator;
    private NetworkAnimator myNetAnimator;

    [SyncVar]
    private bool myIsCasting;
    private Coroutine myCastingRoutine;

    [SyncVar]
    public GameObject myTarget;

    // Use this for initialization
    void Start()
    {
        if (!isServer)
            return;

        myHealth = GetComponent<Health>();
        myStats = GetComponent<Stats>();
        myBuffs = new List<BuffSpell>();

        myAnimator = GetComponent<Animator>();
        myNetAnimator = GetComponent<NetworkAnimator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isServer)
            return;

        HandleBuffs();
    }

    private void HandleBuffs()
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
                    myHealth.TakeDamage(hot.GetTickValue());
                    hot.ShouldDealTickSpellEffect = false;
                }
            }
            else if (myBuffs[index].GetBuff().mySpellType == SpellType.DOT)
            {
                BuffTickSpell dot = myBuffs[index] as BuffTickSpell;
                if (dot.ShouldDealTickSpellEffect)
                {
                    myHealth.TakeDamage(dot.GetTickValue());
                    dot.ShouldDealTickSpellEffect = false;
                }
            }
        }
    }

    public void AddBuff(BuffSpell aBuffSpell, Sprite aSpellIcon)
    {
        if (!hasAuthority)
            return;

        for (int index = 0; index < myBuffs.Count; index++)
        {
            if (myBuffs[index].GetParent() == aBuffSpell.GetParent() &&
                myBuffs[index].GetBuff() == aBuffSpell.GetBuff())
            {
                RemoveBuff(index);
                break;
            }
        }

        myBuffs.Add(aBuffSpell);
        aBuffSpell.GetBuff().ApplyBuff(ref myStats);

        if (aBuffSpell.GetBuff().myAttackSpeed != 0.0f)
        {
            float attackspeed = myAutoAttackCooldownReset / myStats.myAttackSpeed;

            float currentAnimationSpeed = 1.0f;
            if (currentAnimationSpeed > attackspeed)
            {
                myAnimator.SetFloat("AutoAttackSpeed", myAnimator.GetFloat("AutoAttackSpeed") + attackspeed / currentAnimationSpeed);
            }
        }
        else if (aBuffSpell.GetBuff().mySpeedMultiplier != 0.0f)
        {
            myAnimator.SetFloat("RunSpeed", myStats.mySpeedMultiplier);
        }
    }

    private void RemoveBuff(int anIndex)
    {
        myBuffs[anIndex].GetBuff().EndBuff(ref myStats);
        if (myBuffs[anIndex].GetBuff().mySpellType == SpellType.Shield)
        {
            myBuffs[anIndex].GetBuff().myDuration = 0.0f;
            myHealth.RemoveShield();
        }

        if (myBuffs[anIndex].GetBuff().myAttackSpeed != 0.0f)
        {
            float attackspeed = myAutoAttackCooldownReset / myStats.myAttackSpeed;

            float currentAnimationSpeed = 1.0f;
            if (currentAnimationSpeed > attackspeed)
            {
                myAnimator.SetFloat("AutoAttackSpeed", myAnimator.GetFloat("AutoAttackSpeed") + attackspeed / currentAnimationSpeed);
            }
            else
            {
                myAnimator.SetFloat("AutoAttackSpeed", 1.0f);
            }
        }
        else if (myBuffs[anIndex].GetBuff().mySpeedMultiplier != 0.0f)
        {
            myAnimator.SetFloat("RunSpeed", myStats.mySpeedMultiplier);
        }

        myBuffs.RemoveAt(anIndex);
    }

    public void RemoveBuffByName(string aName)
    {
        if (!hasAuthority)
            return;

        for (int index = 0; index < myBuffs.Count; index++)
        {
            if (myBuffs[index].GetName() == aName)
            {
                RemoveBuff(index);
                return;
            }
        }
    }

    public void InterruptSpellCast()
    {
        if (myIsCasting)
        {
            StopCasting();
        }
    }

    private void StopCasting()
    {
        if (myCastingRoutine != null)
            StopCoroutine(myCastingRoutine);
        myIsCasting = false;
    }
}
