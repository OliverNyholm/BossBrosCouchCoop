using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionalSpell : Spell
{
    [Header("Lifetime of movement until removed")]
    [SerializeField]
    private float myLifeTime = 0.05f;
    private float myLifeTimeReset;

    [SerializeField]
    private bool myIgnoreCollisionOnParent = true;

    [SerializeField]
    private bool myRemoveOnTargetImpact = false;

    [Header("The target to damage")]
    [SerializeField]
    private SpellTargetType myTargetType = SpellTargetType.Player;

    private LayerMask myTerrainLayerMask;

    protected override void Awake()
    {
        base.Awake();
        myLifeTimeReset = myLifeTime;

        myTerrainLayerMask = LayerMask.NameToLayer("Terrain");
    }

    public override void Reset()
    {
        base.Reset();
        myLifeTime = myLifeTimeReset;
    }

    protected override void Update()
    {
        myLifeTime -= Time.deltaTime;
        if (myLifeTime <= 0.0f)
        {
            ReturnToPool();
        }

        transform.position += transform.forward * mySpeed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        ObjectTag objectTag = other.GetComponentInParent<ObjectTag>();
        if (objectTag && objectTag.IsTargetType(myTargetType))
        {
            if (myIgnoreCollisionOnParent && objectTag.gameObject == myParent)
                return;

            Health health = other.GetComponentInParent<Health>();
            if (health && !health.IsDead() && myDamage > 0.0f)
                health.TakeDamage(myDamage, myParent.GetComponent<UIComponent>().myCharacterColor, other.transform.position);

            if (myRemoveOnTargetImpact)
            {
                ReturnToPool();
                SpawnVFX(2.5f, gameObject);
                return;
            }
        }

        if (other.gameObject.layer == myTerrainLayerMask)
        {
            ReturnToPool();
            SpawnVFX(2.5f, other.gameObject);
        }
    }
}
