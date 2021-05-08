using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;

public class NPCCastingComponent : CastingComponent
{
    private Resource myResource;
    private BehaviorTree myBehaviorTree;
    private TargetingComponent myTargetingComponent;
    private UIComponent myUIComponent;

    public float AutoAttackTimer { get { return myAutoAttackCooldown; } }

    protected override void Awake()
    {
        base.Awake();

        myResource = GetComponent<Resource>();
        myBehaviorTree = GetComponent<BehaviorTree>();
        myTargetingComponent = GetComponent<TargetingComponent>();
        myUIComponent = GetComponent<UIComponent>();
    }

    public bool CastSpell(GameObject aSpell, GameObject aTarget, Transform aSpawnTransform, bool aShouldIgnoreCastability = false)
    {
        Spell spellScript = aSpell.GetComponent<Spell>();

        if (!aShouldIgnoreCastability && !IsAbleToCastSpell(spellScript))
            return false;

        if (spellScript.myCastTime <= 0.0f)
        {
            SpawnSpell(aSpell, aTarget, aSpawnTransform);
            if (myResource)
                myResource.LoseResource(spellScript.myResourceCost);
            if (myAnimatorWrapper)
                myAnimatorWrapper.SetTrigger(spellScript.myAnimationType);

            return true;
        }

        if (myAnimatorWrapper)
            myAnimatorWrapper.SetBool(AnimationVariable.IsCasting, true);

        myUIComponent.SetCastbarStartValues(spellScript);

        myCastingRoutine = StartCoroutine(CastbarProgress(aSpell, aTarget, aSpawnTransform, aShouldIgnoreCastability));

        return true;
    }

    protected IEnumerator CastbarProgress(GameObject aSpell, GameObject aTarget, Transform aSpawnTransform, bool aShouldIgnoreCastability)
    {
        Spell spellScript = aSpell.GetComponent<Spell>();

        GetComponent<AudioSource>().clip = spellScript.GetSpellSFX().myCastSound;
        GetComponent<AudioSource>().Play();

        myTargetingComponent.SetSpellTarget(aTarget);

        myIsCasting = true;
        float castSpeed = spellScript.myCastTime / myStats.myAttackSpeed;
        float rate = 1.0f / castSpeed;
        float progress = 0.0f;

        while (progress <= 1.0f)
        {
            myUIComponent.SetCastbarValues(Mathf.Lerp(0, 1, progress), (castSpeed - (progress * castSpeed)).ToString("0.0"));

            progress += rate * Time.deltaTime;

            yield return null;
        }

        StopCasting(false);

        if (aShouldIgnoreCastability || IsAbleToCastSpell(spellScript))
        {
            SpawnSpell(aSpell, myTargetingComponent.SpellTarget, aSpawnTransform);
            if (myResource)
                GetComponent<Resource>().LoseResource(spellScript.myResourceCost);
            if (myAnimatorWrapper)
                myAnimatorWrapper.SetTrigger(AnimationVariable.CastingDone);

            myTargetingComponent.SetTarget(myTargetingComponent.Target);
        }
    }

    public override IEnumerator SpellChannelRoutine(Spell aSpell, float aDuration, float aStunDuration)
    {
        Stats stats = GetComponent<Stats>();
        stats.SetStunned(aStunDuration);
        myIsCasting = true;
        float castSpeed = aDuration;
        float rate = 1.0f / castSpeed;
        float progress = 0.0f;

        if (myAnimatorWrapper)
            myAnimatorWrapper.SetBool(AnimationVariable.IsCasting, true);

        while (progress <= 1.0f)
        {
            myUIComponent.SetCastbarValues(Mathf.Lerp(1, 0, progress), (castSpeed - (progress * castSpeed)).ToString("0.0"));

            progress += rate * Time.deltaTime;

            if (!aSpell.myIsCastableWhileMoving && GetComponent<PlayerMovementComponent>().IsMoving() || (Input.GetKeyDown(KeyCode.Escape) && myChannelGameObject != null))
            {
                //myCastbar.SetCastTimeText("Cancelled");
                if (myAnimatorWrapper)
                    myAnimatorWrapper.SetTrigger(AnimationVariable.CastingCancelled);
                stats.SetStunned(0.0f);
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

    public void SpawnSpell(GameObject aSpell, GameObject aTarget, Transform aSpawnTransform)
    {
        GameObject spellGO = PoolManager.Instance.GetPooledObject(aSpell.GetComponent<UniqueID>().GetID());
        if (!spellGO)
            return;

        Spell spellScript = spellGO.GetComponent<Spell>();
        spellScript.SetParent(transform.gameObject);
        spellScript.AddDamageIncrease(myStats.myDamageIncrease);

        if (spellScript.mySpeed == 0.0f && aTarget)
            aSpawnTransform.position = aTarget.transform.position;

        spellGO.transform.position = aSpawnTransform.position;
        spellGO.transform.rotation = aSpawnTransform.rotation;

        spellScript.SetTarget(aTarget);
        spellScript.Restart();

        if (aTarget && aSpawnTransform.position != aTarget.transform.position)
        {
            AudioClip spawnSound = spellScript.GetSpellSFX().mySpawnSound;
            if(spawnSound)
                GetComponent<AudioSource>().PlayOneShot(spawnSound);
        }

        GetComponent<BehaviorTree>().SendEvent("SpellSpawned");
    }

    protected override void StopCasting(bool aWasInterruped)
    {
        base.StopCasting(aWasInterruped);

        if (myBehaviorTree && aWasInterruped)
            myBehaviorTree.SendEvent("SpellInterrupted");
    }

    public void AutoAttack()
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

        if (myAnimatorWrapper)
            myAnimatorWrapper.SetTrigger(SpellAnimationType.AutoAttack);
        myAutoAttackCooldown = myAutoAttackCooldownReset;

        GameObject target = myTargetingComponent.Target;
        SpawnSpell(PoolManager.Instance.GetAutoAttackPrefab(), target, target.transform);
    }

    protected override bool IsAbleToCastSpell(Spell aSpellScript)
    {
        if (myIsCasting)
        {
            Debug.Log(gameObject.name + " failed to cast spell due to already casting spell");
            return false;
        }

        Resource resource = GetComponent<Resource>();
        if (resource && resource.myCurrentResource < aSpellScript.myResourceCost)
        {
            Debug.Log(gameObject.name + " failed to cast spell due to no resources");
            return false;
        }

        if (!aSpellScript.IsCastableWhileMoving() && GetComponent<NPCMovementComponent>().IsMoving())
        {
            Debug.Log(gameObject.name + " failed to cast spell while moving");
            return false;
        }

        if (aSpellScript.myIsOnlySelfCast)
            return true;

        GameObject spellTarget = myTargetingComponent.SpellTarget;
        if (!spellTarget)
        {
            if ((aSpellScript.GetSpellTarget() & SpellTargetType.Player) != 0)
            {
                spellTarget = gameObject;
            }
            else
            {
                Debug.Log(gameObject.name + " failed to cast spell due to no target");
                return false;
            }
        }

        if (spellTarget.GetComponent<Health>().IsDead())
        {
            Debug.Log(gameObject.name + " failed to cast spell due to target being dead");
            return false;
        }

        if (!GetComponent<Character>().CanRaycastToObject(spellTarget))
        {
            Debug.Log(gameObject.name + " failed to cast spell due to no vision of target");
            return false;
        }

        if (spellTarget.GetComponent<Stats>().IsInRange(transform.position, aSpellScript.myRange))
        {
            Debug.Log(gameObject.name + " failed to cast spell due to target being without of range");
            return false;
        }

        ObjectTag objectTag = spellTarget.GetComponent<ObjectTag>();
        if (aSpellScript.GetSpellTarget() != objectTag.GetTargetTag())
        {
            Debug.Log(gameObject.name + " can't cast friendly spells on players");
            return false;
        }

        if (!aSpellScript.myCanCastOnSelf && spellTarget == transform.gameObject)
        {
            Debug.Log(gameObject.name + " failed to cast spell due to spell not castable on self");
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
            return false;
        }

        if (!GetComponent<Character>().CanRaycastToObject(target))
        {
            return false;
        }


        if(!target.GetComponent<Stats>().IsInRange(transform.position, GetComponent<Stats>().myAutoAttackRange))
        {
            return false;
        }

        return true;
    }
}
