using UnityEngine;

[System.Flags]
public enum SpellTarget
{
    Friend = 1 << 1,
    Enemy = 1 << 2,
    Anyone = Friend | Enemy
}

public class Spell : PoolableObject
{
    [HideInInspector] public string myQuickInfo;
    [TextArea(2, 5)]
    [HideInInspector] public string myTutorialInfo;

    [HideInInspector] public string myName;

    [HideInInspector] public SpellType mySpellType;
    [HideInInspector] public SpellTarget mySpellTarget;
    [HideInInspector] public SpellAnimationType myAnimationType;

    [HideInInspector] public int myDamage;
    [HideInInspector] public int myHealValue;
    [HideInInspector] public int myResourceCost;

    [HideInInspector] public float myThreatModifier = 1.0f;
    [HideInInspector] public float mySpeed;
    [HideInInspector] public float myCastTime;
    [HideInInspector] public float myCooldown;
    [HideInInspector] public float myRange;
    [HideInInspector] public float myStunDuration;

    [HideInInspector] public Color myCastbarColor;
    [HideInInspector] public Sprite mySpellIcon;

    [HideInInspector] public bool myIsCastableWhileMoving;
    [HideInInspector] public bool myCanCastOnSelf;
    [HideInInspector] public bool myIsOnlySelfCast;

    [HideInInspector] public bool myShouldRotate;

    protected float myRotationSpeed;
    protected Vector3 myRandomRotation;

    [HideInInspector] public GameObject mySpawnedOnHit;

    protected bool myIsFirstUpdate = true;
    protected bool myReturnToPoolWhenReachedTarget = true;

    [System.Serializable]
    [HideInInspector]
    public struct SpellSFX
    {
        public AudioClip myCastSound;
        public AudioClip mySpawnSound;
        public AudioClip myHitSound;
    }
    //[Header("The sound effects for the spell")]
    [SerializeField]
    [HideInInspector] protected SpellSFX mySpellSFX;
    [SerializeField]
    [HideInInspector] protected bool mySpawnOnHitVFXOnSelf;

    //[Header("The spawned effect for the spell")]
    [SerializeField]
    [HideInInspector] protected GameObject mySpellVFX;

    protected GameObject myParent;
    protected GameObject myTarget;

    public virtual void Restart()
    {
        if (myShouldRotate)
        {
            myRotationSpeed = Random.Range(0.7f, 2.5f);
            myRandomRotation = Random.rotation.eulerAngles;
        }
    }

    public override void Reset()
    {
        myTarget = null;
        myIsFirstUpdate = true;
    }

    protected virtual void Update()
    {
        if (myIsFirstUpdate)
            OnFirstUpdate();

        float distanceSqr = 0.0f;
        if (mySpeed > 0.0f)
            distanceSqr = (myTarget.transform.position - transform.position).sqrMagnitude;

        if (distanceSqr > 1.0f)
        {
            if (myShouldRotate)
                transform.Rotate(myRandomRotation * myRotationSpeed * Time.deltaTime);
            else
                transform.LookAt(myTarget.transform);

            Vector3 direction = myTarget.transform.position - transform.position;
            transform.position += direction.normalized * mySpeed * Time.deltaTime;
        }
        else
        {
            OnReachTarget();
        }
    }

    protected virtual void OnReachTarget()
    {
        DealSpellEffect();
        SpawnVFX(2.5f);

        if (mySpawnedOnHit != null)
        {
            SpawnOnHitObject();
        }

        if (myReturnToPoolWhenReachedTarget)
            ReturnToPool();
    }

    protected virtual void OnFirstUpdate()
    {
        myIsFirstUpdate = false;
    }

    protected virtual void DealSpellEffect()
    {
        if(UtilityFunctions.HasSpellType(mySpellType, SpellType.Damage))
        {
            DealDamage(myDamage);
        }
        if (UtilityFunctions.HasSpellType(mySpellType, SpellType.Heal))
        {
            myTarget.GetComponent<Health>().GainHealth(myHealValue);
            PostMaster.Instance.PostMessage(new Message(MessageCategory.SpellSpawned, new MessageData(myParent.GetInstanceID(), myHealValue)));
        }

        if (myStunDuration > 0.0f)
            myTarget.GetComponent<Stats>().SetStunned(myStunDuration);

        if (UtilityFunctions.HasSpellType(mySpellType, SpellType.Interrupt))
        {
            Interrupt();
        }
        if (UtilityFunctions.HasSpellType(mySpellType, SpellType.Taunt))
        {
            NPCThreatComponent threatComponent = myTarget.GetComponent<NPCThreatComponent>();
            if(threatComponent)
                threatComponent.SetTaunt(myParent.GetInstanceID(), 3.0f);
        }
    }

    protected void SpawnOnHitObject()
    {
        PoolManager poolManager = PoolManager.Instance;
        GameObject spawnObject = poolManager.GetPooledObject(mySpawnedOnHit.GetComponent<UniqueID>().GetID());
        spawnObject.GetComponent<SpawnObjectSpell>().SetParent(myParent);
        spawnObject.GetComponent<SpawnObjectSpell>().SetTarget(myTarget);

        spawnObject.transform.localPosition = transform.position;
        spawnObject.transform.localRotation = Quaternion.identity;
    }

    public virtual void AddDamageIncrease(float aDamageIncrease)
    {
        myDamage = (int)(myDamage * aDamageIncrease);
    }

    public void SetDamage(int aDamage)
    {
        myDamage = aDamage;
    }

    protected void DealDamage(int aDamage, GameObject aTarget = null)
    {
        GameObject target = myTarget;
        if (aTarget)
            target = aTarget;

        Vector3 damageFloatSpawnPosition = transform.position;
        if(mySpeed <= 0.0f)
        {
            Vector3 toParent = (myParent.transform.position - transform.position);
            float distance = toParent.magnitude;
            toParent /= distance;

            const float distanceFromParent = 2.0f;
            damageFloatSpawnPosition += toParent * Mathf.Min(distance - distanceFromParent, target.GetComponent<Stats>().myRangeCylinder.myRadius);
        }

        int parentID = myParent.GetInstanceID();
        int damageDone = target.GetComponent<Health>().TakeDamage(aDamage, myParent.GetComponent<UIComponent>().myCharacterColor, damageFloatSpawnPosition);
        target.GetComponent<Health>().GenerateThreat((int)(damageDone * myThreatModifier), parentID, true);

        if (myParent.tag == "Player")
            PostMaster.Instance.PostMessage(new Message(MessageCategory.DamageDealt, new Vector2(parentID, damageDone)));
    }

    public void SetParent(GameObject aParent)
    {
        myParent = aParent;
    }

    public void SetTarget(GameObject aTarget)
    {
        myTarget = aTarget;
    }

    public GameObject GetTarget()
    {
        return myTarget;
    }

    public SpellTarget GetSpellTarget()
    {
        return mySpellTarget;
    }

    public bool IsCastableWhileMoving()
    {
        return myIsCastableWhileMoving || myCastTime <= 0.0f;
    }

    private void Interrupt()
    {
        if (myTarget.tag == "Player")
            myTarget.GetComponent<CastingComponent>().InterruptSpellCast();
        else if (myTarget.tag == "Enemy")
            myTarget.GetComponent<CastingComponent>().InterruptSpellCast();
    }

    public SpellSFX GetSpellSFX()
    {
        return mySpellSFX;
    }

    public SpellAnimationType GetAnimationType()
    {
        if (myAnimationType == SpellAnimationType.DefaultChannel && myCastTime == 0.0f)
            return SpellAnimationType.DefaultCast;
        else if (myAnimationType == SpellAnimationType.DefaultCast && myCastTime > 0.0f)
            return SpellAnimationType.DefaultChannel;
        else if (myAnimationType == SpellAnimationType.OverheadChannel && myCastTime == 0.0f)
            return SpellAnimationType.DefaultCast;
        else if (myAnimationType == SpellAnimationType.DefaultCast && myCastTime > 0.0f)
            return SpellAnimationType.OverheadChannel;

        return myAnimationType;
    }

    public virtual bool IsCastOnFriends()
    {
        if (myIsOnlySelfCast)
            return false;

        return UtilityFunctions.HasSpellType(mySpellType, SpellType.Heal | SpellType.Ressurect);
    }

    protected GameObject SpawnVFX(float aDuration, GameObject aTarget = null)
    {
        if (!mySpellVFX)
        {
            Debug.Log("Missing VFX for spell: " + myName);
            return null;
        }

        GameObject target = aTarget;
        if (target == null)
            target = myTarget;

        PoolManager poolManager = PoolManager.Instance;
        GameObject vfxGO = poolManager.GetPooledObject(mySpellVFX.GetComponent<UniqueID>().GetID()); ;
        if(mySpawnOnHitVFXOnSelf)
        {
            vfxGO.transform.parent = myParent.transform;
            vfxGO.transform.localPosition = Vector3.zero;
        }
        else if (target)
        {
            vfxGO.transform.parent = target.transform;
            vfxGO.transform.localPosition = Vector3.zero;
        }
        else
        {
            vfxGO.transform.position = transform.position;
        }
        vfxGO.transform.rotation = Quaternion.Euler(-90f, 0.0f, 0.0f);


        vfxGO.GetComponent<AudioSource>().clip = mySpellSFX.myHitSound;
        vfxGO.GetComponent<AudioSource>().Play();

        poolManager.AddTemporaryObject(vfxGO, aDuration);

        return vfxGO;
    }

    public virtual void CreatePooledObjects(PoolManager aPoolManager, int aSpellMaxCount)
    {
        aPoolManager.AddPoolableObjects(gameObject, GetComponent<UniqueID>().GetID(), aSpellMaxCount);

        if (mySpawnedOnHit)
            aPoolManager.AddPoolableObjects(mySpawnedOnHit, mySpawnedOnHit.GetComponent<UniqueID>().GetID(), aSpellMaxCount);

        if (mySpellVFX)
        {
            aPoolManager.AddPoolableObjects(mySpellVFX, mySpellVFX.GetComponent<UniqueID>().GetID(), aSpellMaxCount);
        }
    }
}
