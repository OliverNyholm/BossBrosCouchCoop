using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AoeAttack : Spell
{
    [SerializeField]
    private float myRadius = 10.0f;

    TargetHandler myTargetHandler;

    GameObject myInitialTarget = null;

    private void Awake()
    {
        myTargetHandler = FindObjectOfType<TargetHandler>();
    }

    protected override void OnReachTarget()
    {
        if (myTarget)
        {
            myInitialTarget = myTarget;
            DealSpellEffect();
            if (mySpawnedOnHit != null)
                SpawnOnHitObject();
        }

        SpawnVFX(2.5f);
        DealEffectToSurroundingTargets();

        ReturnToPool();
    }

    private void DealEffectToSurroundingTargets()
    {
        if (myDamage <= 0.0f && myHealValue <= 0.0f)
            return;

        float radiusSqr = myRadius * myRadius;
        List<GameObject> targets = mySpellTarget == SpellTarget.Enemy ? myTargetHandler.GetAllEnemies() : myTargetHandler.GetAllPlayers();
        foreach (GameObject target in targets)
        {
            if (myInitialTarget == target)
                continue;

            if((target.transform.position - transform.position).sqrMagnitude <= radiusSqr)
            {
                myTarget = target;
                DealSpellEffect();
                if (mySpawnedOnHit != null)
                    SpawnOnHitObject();
            }
        }
    }

    public override void Reset()
    {
        base.Reset();
        myInitialTarget = null;
    }
}
