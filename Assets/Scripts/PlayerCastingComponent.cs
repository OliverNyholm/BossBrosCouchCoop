using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCastingComponent : CastingComponent
{
    private PlayerControls myPlayerControls;

    private PlayerTargetingComponent myTargetingComponent;
    private Class myClass;
    private Stats myStats;

    private float myStartTimeOfHoldingKeyDown;
    private float myStartTimeOfReleasingHealingKeyDown;
    private int myFriendlySpellKeyHeldDownIndex;
    private bool myIsFriendlySpellKeyHeldDown;
    private bool myIsHealTargetingEnabled;

    public delegate void EventOnSpellSpawned(GameObject aPlayer, GameObject aSpell, int aSpellIndex);
    public event EventOnSpellSpawned myEventOnSpellSpawned;

    protected override void Awake()
    {
        base.Awake();

        myClass = GetComponent<Class>();
        myTargetingComponent = GetComponent<PlayerTargetingComponent>();
    }

    private void Update()
    {
        
    }

    private void DetectSpellInput()
    {
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
        if (!myClass.IsSpellCastOnFriends(aKeyIndex))
        {
            CastSpell(aKeyIndex, true);
            return;
        }

        myUIComponent.SpellHeldDown(aKeyIndex);
        myStartTimeOfHoldingKeyDown = Time.time;
        myIsFriendlySpellKeyHeldDown = true;
        myFriendlySpellKeyHeldDownIndex = aKeyIndex;
    }

    public void CastFriendlySpell(int aKeyIndex)
    {
        if (Time.time - myStartTimeOfHoldingKeyDown < myTargetingComponent.GetSmartTargetHoldDownMaxDuration())
        {
            myTargetingComponent.SetTargetWithSmartTargeting(aKeyIndex);
            CastSpell(aKeyIndex, true);
        }
        else
        {
            CastSpell(aKeyIndex, false);
            myStartTimeOfReleasingHealingKeyDown = Time.time;
        }

        myTargetingComponent.DisableManualHealTargeting();
    }

    public void CastSpell(int aKeyIndex, bool isPressed)
    {
        if (isPressed)
            myUIComponent.SpellPressed(aKeyIndex);
        else
            myUIComponent.SpellReleased(aKeyIndex);

        if (myClass.IsSpellOnCooldown(aKeyIndex))
        {
            myUIComponent.HighlightSpellError(SpellErrorHandler.SpellError.Cooldown);
            return;
        }

        GameObject spell = myClass.GetSpell(aKeyIndex);
        Spell spellScript = spell.GetComponent<Spell>();

        if (GetComponent<Health>().IsDead())
            return;

        if (!IsAbleToCastSpell(spellScript))
            return;

        myAnimatorWrapper.SetTrigger(spellScript.GetAnimationType());

        if (spellScript.myCastTime <= 0.0f)
        {
            SpawnSpell(aKeyIndex, GetSpellSpawnPosition(spellScript));
            myClass.SetSpellOnCooldown(aKeyIndex);
            GetComponent<Resource>().LoseResource(spellScript.myResourceCost);
            //myAnimator.SetTrigger("CastingDone");
            return;
        }

        myAnimatorWrapper.SetBool(AnimationVariable.IsCasting, true);

        myUIComponent.SetCastbarStartValues(spellScript);

        myCastingRoutine = StartCoroutine(CastbarProgress(aKeyIndex));
    }

    protected override IEnumerator CastbarProgress(int aKeyIndex)
    {
        GameObject spell = myClass.GetSpell(aKeyIndex);
        Spell spellScript = spell.GetComponent<Spell>();

        GetComponent<AudioSource>().clip = spellScript.GetSpellSFX().myCastSound;
        GetComponent<AudioSource>().Play();
        UIComponent uiComponent = GetComponent<UIComponent>();


        myIsCasting = true;
        float castSpeed = spellScript.myCastTime / myStats.myAttackSpeed;
        float rate = 1.0f / castSpeed;
        float progress = 0.0f;

        while (progress <= 1.0f)
        {
            uiComponent.SetCastbarValues(Mathf.Lerp(1, 0, progress), (castSpeed - (progress * castSpeed)).ToString("0.0"));

            progress += rate * Time.deltaTime;

            if (!spellScript.IsCastableWhileMoving() && GetComponent<PlayerMovementComponent>().IsMoving() || Input.GetKeyDown(KeyCode.Escape))
            {
                //myCastbar.SetCastTimeText("Cancelled");
                myAnimatorWrapper.SetTrigger(AnimationVariable.CastingCancelled);
                StopCasting();
                yield break;
            }

            yield return null;
        }

        StopCasting();

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

            if (GetComponent<PlayerMovementComponent>().IsMoving() || (Input.GetKeyDown(KeyCode.Escape) && myChannelGameObject != null))
            {
                //myCastbar.SetCastTimeText("Cancelled");
                myAnimatorWrapper.SetTrigger(AnimationVariable.CastingCancelled);
                myStats.SetStunned(0.0f);
                StopCasting();
                PoolManager.Instance.ReturnObject(myChannelGameObject, myChannelGameObject.GetComponent<UniqueID>().GetID());
                myChannelGameObject = null;
                yield break;
            }

            yield return null;
        }

        StopCasting();
        if (myChannelGameObject != null)
        {
            PoolManager.Instance.ReturnObject(myChannelGameObject, myChannelGameObject.GetComponent<UniqueID>().GetID());
            myChannelGameObject = null;
        }
    }
    protected void SpawnSpell(int aKeyIndex, Vector3 aSpawnPosition)
    {
        GameObject spell;
        if (aKeyIndex == -1)
            spell = myClass.GetAutoAttack();
        else
            spell = myClass.GetSpell(aKeyIndex);

        GameObject target = myTargetingComponent.Target;

        GameObject instance = PoolManager.Instance.GetPooledObject(spell.GetComponent<UniqueID>().GetID());
        instance.transform.position = aSpawnPosition + new Vector3(0.0f, 0.5f, 0.0f);
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

        if (aSpawnPosition == transform.position)
            GetComponent<AudioSource>().PlayOneShot(spellScript.GetSpellSFX().mySpawnSound);

        myEventOnSpellSpawned?.Invoke(gameObject, spell, aKeyIndex);
    }

    protected override bool IsAbleToCastSpell(Spell aSpellScript)
    {
        if (myIsCasting)
        {
            myUIComponent.HighlightSpellError(SpellErrorHandler.SpellError.AlreadyCasting);
            return false;
        }

        if (GetComponent<Resource>().myCurrentResource < aSpellScript.myResourceCost)
        {
            myUIComponent.HighlightSpellError(SpellErrorHandler.SpellError.OutOfResources);
            return false;
        }

        if (!aSpellScript.IsCastableWhileMoving() && GetComponent<PlayerMovementComponent>().IsMoving())
        {
            myUIComponent.HighlightSpellError(SpellErrorHandler.SpellError.CantMoveWhileCasting);
            return false;
        }

        if (aSpellScript.myIsOnlySelfCast)
            return true;

        GameObject target = myTargetingComponent.Target;
        if (!target)
        {
            if ((aSpellScript.GetSpellTarget() & SpellTarget.Friend) != 0)
            {
                target = gameObject;
            }
            else
            {
                myUIComponent.HighlightSpellError(SpellErrorHandler.SpellError.NoTarget);
                return false;
            }
        }

        bool isDead = target.GetComponent<Health>().IsDead();
        if (!isDead && aSpellScript.mySpellType == SpellType.Ressurect)
        {
            myUIComponent.HighlightSpellError(SpellErrorHandler.SpellError.NotDead);
            return false;
        }
        if (isDead && aSpellScript.mySpellType != SpellType.Ressurect)
        {
            myUIComponent.HighlightSpellError(SpellErrorHandler.SpellError.IsDead);
            return false;
        }

        float distance = Vector3.Distance(transform.position, target.transform.position);
        if (distance > aSpellScript.myRange)
        {
            myUIComponent.HighlightSpellError(SpellErrorHandler.SpellError.OutOfRange);
            return false;
        }

        if (!GetComponent<Character>().CanRaycastToObject(target))
        {
            myUIComponent.HighlightSpellError(SpellErrorHandler.SpellError.NoVision);
            return false;
        }

        if ((aSpellScript.GetSpellTarget() & SpellTarget.Enemy) == 0 && target.tag == "Enemy")
        {
            myUIComponent.HighlightSpellError(SpellErrorHandler.SpellError.WrongTargetEnemy);
            return false;
        }

        if ((aSpellScript.GetSpellTarget() & SpellTarget.Friend) == 0 && target.tag == "Player")
        {
            myUIComponent.HighlightSpellError(SpellErrorHandler.SpellError.WrongTargetPlayer);
            return false;
        }

        if (!aSpellScript.myCanCastOnSelf && target == transform.gameObject)
        {
            myUIComponent.HighlightSpellError(SpellErrorHandler.SpellError.NotSelfCast);
            return false;
        }

        return true;
    }

    public void SetPlayerControls(PlayerControls aPlayerControls)
    {
        myPlayerControls = aPlayerControls;
    }
}
