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
    private float myStartTimeOfHoldingKeyDown;
    private bool myIsFriendlySpellKeyHeldDown;
    private bool myHasChangedMovementDirectionAfterCastingManualHeal;
    private int myFriendlySpellKeyHeldDownIndex;

    private bool myShouldAutoAttack;

    public delegate void EventOnSpellSpawned(GameObject aPlayer, GameObject aSpell, int aSpellIndex);
    public event EventOnSpellSpawned myEventOnSpellSpawned;

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

        if (ShouldHealTargetBeEnabled())
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
        myAutoAttackCooldown = 1.2f;

        myTargetingComponent.SpellTarget = myTargetingComponent.Target;
        SpawnSpell(-1, myTargetingComponent.SpellTarget.transform.position);
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

        if (!myIsFriendlySpellKeyHeldDown)
            return;

        if (myPlayerControls.Action1.WasReleased)
            CastFriendlySpell(0);
        else if (myPlayerControls.Action2.WasReleased)
            CastFriendlySpell(1);
        else if (myPlayerControls.Action3.WasReleased)
            CastFriendlySpell(2);
        else if (myPlayerControls.Action4.WasReleased)
            CastFriendlySpell(3);
    }

    public void CheckSpellToCast(int aKeyIndex)
    {
        if (!myClass.HasSpell(aKeyIndex))
            return;

        if (!myClass.IsSpellCastOnFriends(aKeyIndex))
        {
            if (myTargetingComponent.Target == null)
                myTargetingComponent.DetermineNewEnemyTarget();

            CastSpell(aKeyIndex, true);
            return;
        }

        myUIComponent.SpellHeldDown(aKeyIndex);
        myStartTimeOfHoldingKeyDown = Time.time;
        myIsFriendlySpellKeyHeldDown = true;
        myFriendlySpellKeyHeldDownIndex = aKeyIndex;
    }

    private bool ShouldHealTargetBeEnabled()
    {
        return myIsFriendlySpellKeyHeldDown && Time.time - myStartTimeOfHoldingKeyDown > myTargetingComponent.GetSmartTargetHoldDownMaxDuration();
    }

    public bool StillHasSameLookDirectionAfterReleasingManualHeal()
    {
        return !myHasChangedMovementDirectionAfterCastingManualHeal;
    }

    private void CheckHasChangedLookDirectionAfterManualHeal()
    {
        if (myPlayerControls.Movement == Vector3.zero)
            myHasChangedMovementDirectionAfterCastingManualHeal = true;

        if (Vector3.Dot(transform.forward, myHealTargetingReleaseDirection) < 0.7f)
            myHasChangedMovementDirectionAfterCastingManualHeal = true;
    }

    public void CastFriendlySpell(int aKeyIndex)
    {
        if (Time.time - myStartTimeOfHoldingKeyDown < myTargetingComponent.GetSmartTargetHoldDownMaxDuration())
        {
            if (myClass.GetSpell(aKeyIndex).GetComponent<Spell>().myIsOnlySelfCast)
                myTargetingComponent.SpellTarget = gameObject;
            else
                myTargetingComponent.SetTargetWithSmartTargeting(aKeyIndex);

            CastSpell(aKeyIndex, true);
        }
        else
        {
            CastSpell(aKeyIndex, false);
        }

        myHealTargetingReleaseDirection = transform.forward;
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
            SpawnSpell(aKeyIndex, GetSpellSpawnPosition(spellScript));
            myClass.SetSpellOnCooldown(aKeyIndex);
            GetComponent<Resource>().LoseResource(spellScript.myResourceCost);
            myAnimatorWrapper.SetTrigger(spellScript.myAnimationType);
            return;
        }

        myAnimatorWrapper.SetBool(AnimationVariable.IsCasting, true);
        myHasChangedMovementDirectionAfterCastingManualHeal = false;

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

            if (!spellScript.IsCastableWhileMoving() && myMovementComponent && myMovementComponent.IsMoving() || Input.GetKeyDown(KeyCode.Escape))
            {
                //myCastbar.SetCastTimeText("Cancelled");
                myAnimatorWrapper.SetTrigger(AnimationVariable.CastingCancelled);
                StopCasting(true);
                yield break;
            }

            yield return null;
        }

        StopCasting(false);

        if (IsAbleToCastSpell(spellScript))
        {
            SpawnSpell(aKeyIndex, GetSpellSpawnPosition(spellScript));
            myClass.SetSpellOnCooldown(aKeyIndex);
            GetComponent<Resource>().LoseResource(spellScript.myResourceCost);
        }
    }

    public override IEnumerator SpellChannelRoutine(float aDuration, float aStunDuration)
    {
        myStats.SetStunned(aStunDuration);
        myIsCasting = true;
        float castSpeed = aDuration;
        float rate = 1.0f / castSpeed;
        float progress = 0.0f;

        myAnimatorWrapper.SetBool(AnimationVariable.IsCasting, true);
        UIComponent uiComponent = GetComponent<UIComponent>();

        while (progress <= 1.0f)
        {
            uiComponent.SetCastbarValues(Mathf.Lerp(1, 0, progress), (castSpeed - (progress * castSpeed)).ToString("0.0"));

            progress += rate * Time.deltaTime;

            if (myMovementComponent && myMovementComponent.IsMoving() || (Input.GetKeyDown(KeyCode.Escape) && myChannelGameObject != null))
            {
                //myCastbar.SetCastTimeText("Cancelled");
                myAnimatorWrapper.SetTrigger(AnimationVariable.CastingCancelled);
                myStats.SetStunned(0.0f);
                StopCasting(true);
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

    protected void SpawnSpell(int aKeyIndex, Vector3 aSpawnPosition)
    {
        GameObject instance = null;
        if (aKeyIndex == -1)
        {
            instance = PoolManager.Instance.GetPooledAutoAttack();
        }
        else
        {
            instance = PoolManager.Instance.GetPooledObject(myClass.GetSpell(aKeyIndex).GetComponent<UniqueID>().GetID());
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

        myEventOnSpellSpawned?.Invoke(gameObject, instance, aKeyIndex);
    }

    protected override void StopCasting(bool aWasInterruped)
    {
        base.StopCasting(aWasInterruped);
        myHasChangedMovementDirectionAfterCastingManualHeal = true;
    }

    protected override bool IsAbleToCastSpell(Spell aSpellScript)
    {
        if (myIsCasting)
        {
            ShowError(SpellErrorHandler.SpellError.AlreadyCasting);
            return false;
        }

        if (GetComponent<Resource>().myCurrentResource < aSpellScript.myResourceCost)
        {
            ShowError(SpellErrorHandler.SpellError.OutOfResources);
            return false;
        }

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
            if ((aSpellScript.GetSpellTarget() & SpellTarget.Friend) != 0)
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

        if ((aSpellScript.GetSpellTarget() & SpellTarget.Enemy) == 0 && target.tag == "Enemy")
        {
            ShowError(SpellErrorHandler.SpellError.WrongTargetEnemy);
            return false;
        }

        if ((aSpellScript.GetSpellTarget() & SpellTarget.Friend) == 0 && target.tag == "Player")
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
}
