﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chicken : Character
{
    [SerializeField]
    private GameObject myParent = null;
    private Stats myParentStats = null;
    private ChickenFarmerChickenHandler myChickenHandler = null;

    [SerializeField]
    private GameObject myOnDeathVfx = null;

    [SerializeField]
    private SpellOverTime myEggBuff = null;

    [SerializeField]
    private float myDropEggInterval = 2.0f;
    private float myDropEggTimer = 0.0f;

    private bool myIsBombDropping = false;

    protected override void Awake()
    {
        base.Awake();

        PoolManager.Instance.AddPoolableObjects(myOnDeathVfx, myOnDeathVfx.GetComponent<UniqueID>().GetID(), 1);
    }

    public void SetParent(GameObject aParent)
    {
        myParent = aParent;
        myParentStats = aParent.GetComponent<Stats>();
        myChickenHandler = aParent.GetComponent<ChickenFarmerChickenHandler>();
    }

    public override void Reset()
    {
        base.Reset();
        myDropEggTimer = myDropEggInterval;

        myParent = null;
        myParentStats = null;
        myIsBombDropping = false;

        GetComponent<ChickenMovementComponent>().Reset();
    }

    protected override void Update()
    {
        if (!myParent || !myEggBuff || myIsBombDropping)
            return;

        myDropEggTimer -= Time.deltaTime;
        if (myDropEggTimer <= 0.0f)
        {
            if (!myParentStats.HasMaxSpellOverTimeStackCount(myEggBuff))
                myChickenHandler.SpawnEgg(gameObject);

            myDropEggTimer = myDropEggInterval;
        }
    }

    public void SetIsDroppingBomb()
    {
        myIsBombDropping = true;
    }

    protected override void OnDeath()
    {
        myChickenHandler.ReturnChicken(gameObject);

        PoolManager poolManager = PoolManager.Instance;
        GameObject onDeathVfx = poolManager.GetPooledObject(myOnDeathVfx.GetComponent<UniqueID>().GetID());
        onDeathVfx.transform.position = transform.position + Vector3.up * 0.8f;
        poolManager.AddTemporaryObject(onDeathVfx, 4.0f);

        ReturnToPool();
    }
}