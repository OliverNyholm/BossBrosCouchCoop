using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AoeAttack : Spell
{
    private float myLifeTime = 0.05f;

    [Header("The target to damage")]
    [SerializeField]
    private string myAttackTag = "Enemy";

    private bool myHasRechedTarget = false;

    TargetHandler myTargetHandler;

    private void Awake()
    {
        myTargetHandler = FindObjectOfType<TargetHandler>();
    }

    protected override void Update()
    {
        if(!myHasRechedTarget)
            base.Update();

        myLifeTime -= Time.deltaTime;
        if (myLifeTime <= 0.0f)
        {
            ReturnToPool();
        }
    }

    protected override void OnReachTarget()
    {
        myHasRechedTarget = true;

        if (myTarget)
            DealSpellEffect();

        SpawnVFX(2.5f);
        DealDamageToSurroundingTargets();

        if (mySpawnedOnHit != null)
        {
            SpawnOnHitObject();
        }
    }

    private void DealDamageToSurroundingTargets()
    {
        if (myDamage <= 0.0f)
            return;

        float radiusSqr = Mathf.Pow(GetComponent<SphereCollider>().radius * transform.lossyScale.x, 2);
        List<GameObject> targets = myAttackTag == "Enemy" ? myTargetHandler.GetAllEnemies() : myTargetHandler.GetAllPlayers();
        foreach (GameObject target in targets)
        {
            if((target.transform.position - transform.position).sqrMagnitude <= radiusSqr)
            {
                target.GetComponentInParent<Health>().TakeDamage(myDamage, myParent.GetComponent<UIComponent>().myCharacterColor.linear, target.transform.position);
            }
        }
    }

    public override void Reset()
    {
        base.Reset();
        myHasRechedTarget = false;
    }
}
