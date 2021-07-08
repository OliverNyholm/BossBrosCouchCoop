using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrafeShooting : ToggleSpell
{
    [SerializeField]
    private GameObject myProjectileSpell = null;
    [SerializeField]
    private GameObject myKnockbackBuff = null;

    [SerializeField]
    private float myShootInterval = 1.5f;
    private float myShootTimer = 0.0f;

    public override void ToggledOn()
    {
        base.ToggledOn();

        PlayerMovementComponent parentMovement = myParent.GetComponent<PlayerMovementComponent>();
        if (parentMovement)
            parentMovement.SetStrafeFocusTarget(myTarget.transform);

        PlayerTargetingComponent parentTargeting = myParent.GetComponent<PlayerTargetingComponent>();
        if (parentTargeting)
            parentTargeting.myOnTargetChangedEvent += OnTargetChanged;

        myShootTimer = myShootInterval / 3.0f;
    }

    public override void ToggleOff()
    {
        base.ToggleOff();

        PlayerMovementComponent parentMovement = myParent.GetComponent<PlayerMovementComponent>();
        if (parentMovement)
            parentMovement.SetStrafeFocusTarget(null);

        PlayerTargetingComponent parentTargeting = myParent.GetComponent<PlayerTargetingComponent>();
        if (parentTargeting)
            parentTargeting.myOnTargetChangedEvent -= OnTargetChanged;
    }

    public void OnTargetChanged(GameObject aTarget)
    {
        if (!aTarget)
        {
            ToggleOff();
        }
        else
        {
            myTarget = aTarget;
            PlayerMovementComponent parentMovement = myParent.GetComponent<PlayerMovementComponent>();
            if (parentMovement)
                parentMovement.SetStrafeFocusTarget(aTarget.transform);
        }
    }

    protected override void Update()
    {
        base.Update();

        myShootTimer -= Time.deltaTime;
        if (myShootTimer > 0.0f)
            return;

        myShootTimer += myShootInterval;

        PlayerCastingComponent playerCastingComponent = myParent.GetComponent<PlayerCastingComponent>();
        if (playerCastingComponent)
        {
            Spell projectile = playerCastingComponent.SpawnSpellExternal(myProjectileSpell.GetComponent<Spell>(), myParent.transform.position + Vector3.up * 1.0f);
            if (!projectile)
            {
                Debug.Log("Out of projectiles while shooting: " + name);
                return;
            }

            projectile.SetParent(myParent);
            projectile.transform.forward = myParent.transform.forward;

            if (myKnockbackBuff)
                SpawnKnockbackBuff();
        }
    }

    public override void CreatePooledObjects(PoolManager aPoolManager, int aSpellMaxCount, GameObject aSpawner = null)
    {
        base.CreatePooledObjects(aPoolManager, aSpellMaxCount, aSpawner);

        if (myProjectileSpell)
        {
            Spell projectile = myProjectileSpell.GetComponent<Spell>();
            projectile.CreatePooledObjects(aPoolManager, projectile.myPoolSize, aSpawner);
        }

        if (myKnockbackBuff)
        {
            Spell knockback = myKnockbackBuff.GetComponent<Spell>();
            knockback.CreatePooledObjects(aPoolManager, knockback.myPoolSize, aSpawner);
        }
    }

    public void SpawnKnockbackBuff()
    {
        PoolManager poolManager = PoolManager.Instance;
        GameObject spawnObject = poolManager.GetPooledObject(myKnockbackBuff.GetComponent<UniqueID>().GetID());
        if (!spawnObject)
            return;

        spawnObject.transform.parent = myParent.transform;
        spawnObject.transform.localPosition = Vector3.zero;

        DragBuff knockback = spawnObject.GetComponent<DragBuff>();
        knockback.SetParent(myParent);
        knockback.SetTarget(myParent);
        knockback.SetDragDirection(-myParent.transform.forward);
    }
}
