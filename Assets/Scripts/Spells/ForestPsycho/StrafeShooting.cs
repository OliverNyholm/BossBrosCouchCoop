using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrafeShooting : ToggleSpell
{
    [SerializeField]
    private GameObject myProjectileSpell = null;

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
        if (myShootTimer <= 0.0f)
        {
            myShootTimer += myShootInterval;

            PlayerCastingComponent playerCastingComponent = myParent.GetComponent<PlayerCastingComponent>();
            if (playerCastingComponent)
            {
                Spell projectile = playerCastingComponent.SpawnSpellExternal(myProjectileSpell.GetComponent<Spell>(), myParent.transform.position + Vector3.up * 1.0f);
                projectile.SetParent(myParent);
                projectile.transform.forward = myParent.transform.forward;
            }
        }
    }

    public override void CreatePooledObjects(PoolManager aPoolManager, int aSpellMaxCount, GameObject aSpawner = null)
    {
        base.CreatePooledObjects(aPoolManager, aSpellMaxCount, aSpawner);

        Spell projectile = myProjectileSpell.GetComponent<Spell>();
        if (myProjectileSpell)
            projectile.CreatePooledObjects(aPoolManager, projectile.myPoolSize, aSpawner);
    }
}
