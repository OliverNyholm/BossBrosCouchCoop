using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCastingComponent : CastingComponent
{
    protected PlayerControls myPlayerControls;

    protected PlayerTargetingComponent myTargetingComponent;
    protected PlayerUIComponent myUIComponent;
    private PlayerControlsVibrationManager myPlayerControlsVibrationsManager;
    private PlayerMovementComponent myMovementComponent;
    protected Class myClass;

    private Vector3 myHealTargetingReleaseDirection;
    private float myStartTimeOfHoldingKeyDown = 0;
    private bool myIsFriendlySpellKeyHeldDown;
    private bool myHasChangedMovementDirectionAfterCastingManualHeal;
    private int myFriendlySpellKeyHeldDownIndex = 0;

    private float myCastingWhileMovingBufferTimestamp;

    private bool myShouldAutoAttack;
    private bool myKeepLookingAtTargetWhileCasting = false;

    public delegate void EventOnSpellSpawned(GameObject aPlayer, GameObject aSpell, int aSpellIndex);

    protected override void Awake()
    {
        base.Awake();
        myClass = GetComponent<Class>();
        myTargetingComponent = GetComponent<PlayerTargetingComponent>();
        myMovementComponent = GetComponent<PlayerMovementComponent>();
        myUIComponent = GetComponent<PlayerUIComponent>();
        myPlayerControlsVibrationsManager = GetComponent<PlayerControlsVibrationManager>();

        myHasChangedMovementDirectionAfterCastingManualHeal = true;
    }

    private void Update()
    {
        if (myHealth.IsDead())
            return;

        DetectSpellInput();

        if (!myHasChangedMovementDirectionAfterCastingManualHeal)
            CheckHasChangedLookDirectionAfterManualHeal();

        if (ShouldHealTargetBeEnabled() && !myTargetingComponent.IsManualHealTargeting())
            myTargetingComponent.EnableManualHealTargeting(myFriendlySpellKeyHeldDownIndex);

        if (myShouldAutoAttack)
            AutoAttack();
    }

    public void SetPlayerController(PlayerControls aPlayerControls)
    {
        myPlayerControls = aPlayerControls;
        myPlayerControlsVibrationsManager.SetPlayerControls(aPlayerControls);
    }

    public void SetShouldAutoAttack(bool aValue)
    {
        myShouldAutoAttack = aValue;
    }

    private void AutoAttack()
    {
        if (myAutoAttackCooldown > 0.0f)
        {
            myAutoAttackCooldown -= Time.deltaTime * GetComponent<Stats>().myAttackSpeed;
            return;
        }

        if (!IsAbleToAutoAttack())
        {
            return;
        }

        myAnimatorWrapper.SetTrigger(SpellAnimationType.AutoAttack);
        myAutoAttackCooldown = myAutoAttackCooldownReset;

        myTargetingComponent.SpellTarget = myTargetingComponent.Target;
        SpawnSpell(null, myTargetingComponent.SpellTarget.transform.position, true);
    }

    private void DetectSpellInput()
    {
        if (Time.timeScale <= 0.0f)
            return;

        if (myPlayerControls.Action1.WasPressed)
            CheckSpellToCast(0);
        else if (myPlayerControls.Action2.WasPressed)
            CheckSpellToCast(1);
        else if (myPlayerControls.Action3.WasPressed)
            CheckSpellToCast(2);
        else if (myPlayerControls.Action4.WasPressed)
            CheckSpellToCast(3);

        //if (!myIsFriendlySpellKeyHeldDown)
        //    return;
        //
        //if (myPlayerControls.Action1.WasReleased)
        //    CastFriendlySpell(0);
        //else if (myPlayerControls.Action2.WasReleased)
        //    CastFriendlySpell(1);
        //else if (myPlayerControls.Action3.WasReleased)
        //    CastFriendlySpell(2);
        //else if (myPlayerControls.Action4.WasReleased)
        //    CastFriendlySpell(3);
    }

    public void CheckSpellToCast(int aKeyIndex)
    {
        if (!myClass.HasSpell(aKeyIndex))
            return;

        Spell spell = myClass.GetSpell(aKeyIndex).GetComponent<Spell>();
        if (spell.myIsOnlySelfCast)
        {
            CastSpell(aKeyIndex, true);
            return;
        }
        else
        {
            myTargetingComponent.FindSpellTarget(spell);
            CastSpell(aKeyIndex, true);
            return;
        }


        //if (!myClass.IsSpellCastOnFriends(aKeyIndex))
        //{
        //    if (myTargetingComponent.Target == null)
        //        myTargetingComponent.DetermineNewEnemyTarget();
        //
        //    CastSpell(aKeyIndex, true);
        //    return;
        //}
        //else if (myClass.GetSpell(aKeyIndex).GetComponent<Spell>().myIsOnlySelfCast)
        //{
        //    CastSpell(aKeyIndex, true);
        //    return;
        //}

        //Debug.Log("we can remove this now :)");
        //myUIComponent.SpellHeldDown(aKeyIndex);
        //myStartTimeOfHoldingKeyDown = Time.time;
        //myIsFriendlySpellKeyHeldDown = true;
        //myFriendlySpellKeyHeldDownIndex = aKeyIndex;
    }

    private bool ShouldHealTargetBeEnabled()
    {
        return myIsFriendlySpellKeyHeldDown && Time.time - myStartTimeOfHoldingKeyDown > myTargetingComponent.GetSmartTargetHoldDownMaxDuration();
    }

    public bool HasSameLookDirectionAfterReleasingManualHeal()
    {
        return !myHasChangedMovementDirectionAfterCastingManualHeal;
    }

    private void CheckHasChangedLookDirectionAfterManualHeal()
    {
        if (myPlayerControls.Movement == Vector3.zero)
            myHasChangedMovementDirectionAfterCastingManualHeal = true;

        if (Vector3.Dot(myPlayerControls.Movement, myHealTargetingReleaseDirection) < 0.7f)
            myHasChangedMovementDirectionAfterCastingManualHeal = true;
    }

    public void CastFriendlySpell(int aKeyIndex)
    {
        if(myClass.GetSpell(aKeyIndex).GetComponent<Spell>().myIsOnlySelfCast)
        {
            myTargetingComponent.SpellTarget = gameObject;
        }
        else if(myTargetingComponent.IsSmartHealingAvailable())
        {
            if (Time.time - myStartTimeOfHoldingKeyDown < myTargetingComponent.GetSmartTargetHoldDownMaxDuration())
            {
                myTargetingComponent.SetTargetWithLowestHealthAndWithoutBuff(myClass.GetSpell(aKeyIndex).GetComponent<Spell>());
                CastSpell(aKeyIndex, true);
            }
            else
            {
                if (myTargetingComponent.Target != gameObject)
                    myKeepLookingAtTargetWhileCasting = true;

                CastSpell(aKeyIndex, false);
            }
        }
        else
        {
            CastSpell(aKeyIndex, false);
        }


        myHealTargetingReleaseDirection = myPlayerControls.Movement;
        myIsFriendlySpellKeyHeldDown = false;
        myTargetingComponent.DisableManualHealTargeting(myFriendlySpellKeyHeldDownIndex);
    }

    public void CastSpell(int aKeyIndex, bool isPressed)
    {
        if (isPressed)
            myUIComponent.SpellPressed(aKeyIndex);
        else
            myUIComponent.SpellReleased(aKeyIndex);

        if (myClass.IsSpellOnCooldown(aKeyIndex))
        {
            ShowError(SpellErrorHandler.SpellError.Cooldown);
            return;
        }

        GameObject spell = myClass.GetSpell(aKeyIndex);
        Spell spellScript = spell.GetComponent<Spell>();

        if (GetComponent<Health>().IsDead())
            return;

        if (!spellScript.myIsOnlySelfCast)
            myTargetingComponent.SpellTarget = myTargetingComponent.Target;

        if (!IsAbleToCastSpell(spellScript))
            return;

        myAnimatorWrapper.SetTrigger(spellScript.GetAnimationType());

        if (spellScript.myCastTime <= 0.0f)
        {
            SpawnSpell(spellScript, GetSpellSpawnPosition(spellScript));
            myClass.SetSpellOnCooldown(aKeyIndex);
            //GetComponent<Resource>().LoseResource(spellScript.myResourceCost);
            myAnimatorWrapper.SetTrigger(spellScript.myAnimationType);
            return;
        }

        myAnimatorWrapper.SetBool(AnimationVariable.IsCasting, true);
        myHasChangedMovementDirectionAfterCastingManualHeal = false;

        if (spellScript.myIsCastableWhileMoving)
        {
            mySpellCastingMovementSpeedReducement = spellScript.mySpeedWhileCastingReducement;
            myStats.mySpeedMultiplier -= mySpellCastingMovementSpeedReducement;
        }
        else
        {
            mySpellCastingMovementSpeedReducement = 0.0f;
        }

        myUIComponent.SetCastbarStartValues(spellScript);
        myTargetingComponent.SetSpellTarget(myTargetingComponent.Target);
        myCastingRoutine = StartCoroutine(CastbarProgress(aKeyIndex));
    }

    public IEnumerator CastbarProgress(int aKeyIndex)
    {
        GameObject spell = myClass.GetSpell(aKeyIndex);
        Spell spellScript = spell.GetComponent<Spell>();

        GetComponent<AudioSource>().clip = spellScript.GetSpellSFX().myCastSound;
        GetComponent<AudioSource>().Play();

        myIsCasting = true;
        float castSpeed = spellScript.myCastTime / myStats.myAttackSpeed;
        float rate = 1.0f / castSpeed;
        float progress = 0.0f;

        while (progress <= 1.0f)
        {
            myUIComponent.SetCastbarValues(Mathf.Lerp(0, 1, progress), (castSpeed - (progress * castSpeed)).ToString("0.0"));

            progress += rate * Time.deltaTime;

            if (InterruptDueToMovement(spellScript) || Input.GetKeyDown(KeyCode.Escape))
            {
                //myCastbar.SetCastTimeText("Cancelled");
                myAnimatorWrapper.SetTrigger(AnimationVariable.CastingCancelled);
                StopCasting(true);
                yield break;
            }

            if (myKeepLookingAtTargetWhileCasting && myTargetingComponent.SpellTarget)
                transform.LookAt(myTargetingComponent.SpellTarget.transform, Vector3.up);

            yield return null;
        }

        StopCasting(false);

        if (IsAbleToCastSpell(spellScript))
        {
            SpawnSpell(spellScript, GetSpellSpawnPosition(spellScript));
            myClass.SetSpellOnCooldown(aKeyIndex);
            //GetComponent<Resource>().LoseResource(spellScript.myResourceCost);
        }
    }

    public override IEnumerator SpellChannelRoutine(Spell aSpell, float aDuration, float aStunDuration)
    {
        myStats.SetStunned(aStunDuration);
        myIsCasting = true;
        float castSpeed = aDuration;
        float rate = 1.0f / castSpeed;
        float progress = 0.0f;

        int spellIndex = myClass.GetSpellIndex(aSpell);

        myAnimatorWrapper.SetBool(AnimationVariable.IsCasting, true);

        while (progress <= 1.0f)
        {
            if(castSpeed > 0.0f)
            {
                myUIComponent.SetCastbarValues(Mathf.Lerp(1, 0, progress), (castSpeed - (progress * castSpeed)).ToString("0.0"));
                progress += rate * Time.deltaTime;
            }

            if (InterruptDueToMovement(aSpell) || WasSpellChannelButtonReleased(spellIndex) || (Input.GetKeyDown(KeyCode.Escape) && myChannelGameObject != null))
            {
                myAnimatorWrapper.SetTrigger(AnimationVariable.CastingCancelled);
                myStats.SetStunned(0.0f);
                StopCasting(true);

                if (aSpell is ChannelSpell)
                    (aSpell as ChannelSpell).OnStoppedChannel();

                PoolManager.Instance.ReturnObject(myChannelGameObject, myChannelGameObject.GetComponent<UniqueID>().GetID());
                myChannelGameObject = null;

                yield break;
            }

            yield return null;
        }

        StopCasting(false);
        if (myChannelGameObject != null)
        {
            PoolManager.Instance.ReturnObject(myChannelGameObject, myChannelGameObject.GetComponent<UniqueID>().GetID());
            myChannelGameObject = null;
        }
    }

    public void SpawnSpellExternal(Spell aSpell, Vector3 aSpawnPosition)
    {
        SpawnSpell(aSpell, aSpawnPosition);
    }

    protected void SpawnSpell(Spell aSpell, Vector3 aSpawnPosition, bool aIsAutoAttack = false)
    {
        GameObject instance = null;
        if (aIsAutoAttack)
        {
            instance = PoolManager.Instance.GetPooledAutoAttack();
        }
        else
        {
            instance = PoolManager.Instance.GetPooledObject(aSpell.GetComponent<UniqueID>().GetID());
        }

        if (!instance)
            return;

        instance.transform.position = aSpawnPosition + new Vector3(0.0f, 0.5f, 0.0f);

        GameObject target = myTargetingComponent.SpellTarget;
        if (target)
            instance.transform.LookAt(target.transform);
        else
            instance.transform.rotation = transform.rotation;

        Spell spellScript = instance.GetComponent<Spell>();
        spellScript.SetParent(transform.gameObject);
        spellScript.AddDamageIncrease(myStats.myDamageIncrease);

        if (spellScript.myIsOnlySelfCast)
            spellScript.SetTarget(transform.gameObject);
        else if (target)
            spellScript.SetTarget(target);
        else
            spellScript.SetTarget(transform.gameObject);

        spellScript.Restart();

        if (aSpawnPosition == transform.position && spellScript.GetSpellSFX().mySpawnSound)
            GetComponent<AudioSource>().PlayOneShot(spellScript.GetSpellSFX().mySpawnSound);

        myCastingWhileMovingBufferTimestamp = Time.time;
    }

    protected override void StopCasting(bool aWasInterruped)
    {
        base.StopCasting(aWasInterruped);
        myHasChangedMovementDirectionAfterCastingManualHeal = true;
        myKeepLookingAtTargetWhileCasting = false;
    }

    protected override bool IsAbleToCastSpell(Spell aSpellScript)
    {
        if (myIsCasting)
        {
            ShowError(SpellErrorHandler.SpellError.AlreadyCasting);
            return false;
        }

        //if (GetComponent<Resource>().myCurrentResource < aSpellScript.myResourceCost)
        //{
        //    ShowError(SpellErrorHandler.SpellError.OutOfResources);
        //    return false;
        //}

        if (!aSpellScript.IsCastableWhileMoving() && myMovementComponent && myMovementComponent.IsMoving())
        {
            ShowError(SpellErrorHandler.SpellError.CantMoveWhileCasting);
            return false;
        }

        if (aSpellScript.myIsOnlySelfCast)
            return true;

        GameObject target = myTargetingComponent.SpellTarget;
        if (!target)
        {
            if ((aSpellScript.GetSpellTarget() & SpellTargetType.Player) != 0)
            {
                target = gameObject;
            }
            else
            {
                ShowError(SpellErrorHandler.SpellError.NoTarget);
                return false;
            }
        }

        bool isDead = target.GetComponent<Health>().IsDead();
        if (!isDead && UtilityFunctions.HasSpellType(aSpellScript.mySpellType, SpellType.Ressurect))
        {
            ShowError(SpellErrorHandler.SpellError.NotDead);
            return false;
        }
        if (isDead && !UtilityFunctions.HasSpellType(aSpellScript.mySpellType, SpellType.Ressurect))
        {
            ShowError(SpellErrorHandler.SpellError.IsDead);
            return false;
        }

        if(!target.GetComponent<Stats>().IsInRange(transform.position, aSpellScript.myRange))
        {
            ShowError(SpellErrorHandler.SpellError.OutOfRange);
            return false;
        }

        if (!GetComponent<Character>().CanRaycastToObject(target))
        {
            ShowError(SpellErrorHandler.SpellError.NoVision);
            return false;
        }

        if ((aSpellScript.GetSpellTarget() & SpellTargetType.NPC) == 0 && target.tag == "Enemy")
        {
            if ((aSpellScript.GetSpellTarget() & SpellTargetType.Player) != 0)
            {
                myTargetingComponent.SetTarget(gameObject);
            }
            else
            {
                ShowError(SpellErrorHandler.SpellError.WrongTargetEnemy);
                return false;
            }
        }

        if ((aSpellScript.GetSpellTarget() & SpellTargetType.Player) == 0 && target.tag == "Player")
        {
            ShowError(SpellErrorHandler.SpellError.WrongTargetPlayer);
            return false;
        }

        if (!aSpellScript.myCanCastOnSelf && target == transform.gameObject)
        {
            ShowError(SpellErrorHandler.SpellError.NotSelfCast);
            return false;
        }

        return true;
    }

    private bool IsAbleToAutoAttack()
    {
        if (myIsCasting)
        {
            return false;
        }

        GameObject target = myTargetingComponent.Target;
        if (!target)
        {
            return false;
        }

        if (target.GetComponent<Health>().IsDead())
        {
            myShouldAutoAttack = false;
            return false;
        }

        if (!GetComponent<Character>().CanRaycastToObject(target))
        {
            return false;
        }

        if (!target.GetComponent<Stats>().IsInRange(transform.position, GetComponent<Stats>().myAutoAttackRange))
        {
            return false;
        }

        return true;
    }

    public void SetPlayerControls(PlayerControls aPlayerControls)
    {
        myPlayerControls = aPlayerControls;
    }

    protected override void OnDeath()
    {
        base.OnDeath();
        myShouldAutoAttack = false;
    }

    protected void ShowError(SpellErrorHandler.SpellError aSpellError)
    {
        myUIComponent.HighlightSpellError(aSpellError);
        myPlayerControlsVibrationsManager.VibratePlayerCastingError(aSpellError);
    }

    private bool InterruptDueToMovement(Spell aSpell)
    {
        if (aSpell.IsCastableWhileMoving())
            return false;

        if (Time.time - myCastingWhileMovingBufferTimestamp < 0.2f)
            return false;

        return myMovementComponent && myMovementComponent.IsMoving();
    }

    private bool WasSpellChannelButtonReleased(int aSpellIndex)
    {
        switch (aSpellIndex)
        {
            case 0:
                return myPlayerControls.Action1.WasReleased;
            case 1:
                return myPlayerControls.Action2.WasReleased;
            case 2:
                return myPlayerControls.Action3.WasReleased;
            case 3:
                return myPlayerControls.Action4.WasReleased;
        }

        return false;
    }
}
