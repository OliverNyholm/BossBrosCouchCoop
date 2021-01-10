using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AoeAttack : Spell
{
    [SerializeField]
    private float myRadius = 10.0f;

    [SerializeField]
    private bool myIsCylinder = false;

    [SerializeField]
    private float myCylinderHeight = 4.0f;

    GameObject myInitialTarget = null;

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
        List<GameObject> targets = mySpellTarget == SpellTargetType.NPC ? myTargetHandler.GetAllEnemies() : myTargetHandler.GetAllPlayers();
        foreach (GameObject target in targets)
        {
            if (myInitialTarget == target || !target)
                continue;

            Health health = target.GetComponent<Health>();
            if (health && health.IsDead())
                continue;

            Vector3 toTarget = target.transform.position - transform.position;
            bool isInsideCylinder = myIsCylinder? Mathf.Abs(toTarget.y) < myCylinderHeight : true;

            if (isInsideCylinder && toTarget.sqrMagnitude <= radiusSqr)
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
