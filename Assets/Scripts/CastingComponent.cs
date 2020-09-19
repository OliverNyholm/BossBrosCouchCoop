using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CastingComponent : MonoBehaviour
{
    protected AnimatorWrapper myAnimatorWrapper;
    protected Health myHealth;
    protected Stats myStats;

    protected GameObject myChannelGameObject;
    protected Coroutine myCastingRoutine;

    protected bool myIsCasting;
    public bool IsInterruptable { get; set; }

    protected float myAutoAttackCooldown;
    public float myAutoAttackCooldownReset = 1.0f;

    protected virtual void Awake()
    {
        myAnimatorWrapper = GetComponent<AnimatorWrapper>();
        myHealth = GetComponent<Health>();
        myStats = GetComponent<Stats>();
    }

    void Start()
    {
        myIsCasting = false;
        IsInterruptable = true;

        if (myHealth)
            myHealth.EventOnHealthZero += OnDeath;
    }

    void Update()
    {
        if (myAutoAttackCooldown > 0.0f)
        {
            myAutoAttackCooldown -= Time.deltaTime * myStats.myAttackSpeed;
        }
    }

    public abstract IEnumerator SpellChannelRoutine(float aDuration, float aStunDuration);

    public void StartChannel(float aDuration, Spell aSpellScript, GameObject aChannelGO, float aStunDuration = 1.0f)
    {
        GetComponent<UIComponent>().SetCastbarChannelingStartValues(aSpellScript, aDuration);

        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.clip = aSpellScript.GetSpellSFX().myCastSound;
        audioSource.Play();

        myChannelGameObject = aChannelGO;
        myCastingRoutine = StartCoroutine(SpellChannelRoutine(aDuration, aStunDuration));
    }

    protected abstract bool IsAbleToCastSpell(Spell aSpellScript);

    public Vector3 GetSpellSpawnPosition(Spell aSpellScript)
    {
        GameObject target = GetComponent<TargetingComponent>().Target;
        if (aSpellScript.mySpeed <= 0.0f && !aSpellScript.myIsOnlySelfCast && target != null)
        {
            return target.transform.position;
        }

        return transform.position;
    }

    protected virtual void StopCasting(bool aWasInterruped)
    {
        if (myCastingRoutine != null)
            StopCoroutine(myCastingRoutine);
        myIsCasting = false;
        GetComponent<AudioSource>().Stop();

        GetComponent<UIComponent>().FadeCastbar(aWasInterruped);

        if (myAnimatorWrapper)
        {
            myAnimatorWrapper.SetBool(AnimationVariable.IsCasting, false);
            if (!aWasInterruped)
                myAnimatorWrapper.SetTrigger(AnimationVariable.CastingDone);
        }
    }

    public void InterruptSpellCast()
    {
        if (myIsCasting && IsInterruptable)
        {
            StopCasting(true);
        }
    }

    protected virtual void OnDeath()
    {
        StopAllCoroutines();
        StopCasting(true);
        GetComponent<AudioSource>().Stop();

        if (myChannelGameObject)
        {
            PoolManager.Instance.ReturnObject(myChannelGameObject, myChannelGameObject.GetComponent<UniqueID>().GetID());
            myChannelGameObject = null;
        }
    }

    public bool IsCasting()
    {
        return myIsCasting;
    }
}
