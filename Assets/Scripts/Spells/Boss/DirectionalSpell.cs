using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionalSpell : Spell
{
    [Header("Lifetime of movement until removed")]
    [SerializeField]
    private float myLifeTime = 0.05f;

    [Header("The target to damage")]
    [SerializeField]
    private string myAttackTag = "Enemy";

    private Vector3 myDirection;

    protected override void Start()
    {
        base.Start();
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
        if (other.gameObject.tag == myAttackTag)
        {
            if (!other.GetComponent<Health>().IsDead() && myDamage > 0.0f)
                other.GetComponentInParent<Health>().TakeDamage(myDamage, myParent.GetComponent<UIComponent>().myCharacterColor);
        }
        
        if(other.gameObject.tag == "Terrain")
        {
            ReturnToPool();
            SpawnVFX(2.5f, other.gameObject);
        }
    }

    protected override string GetSpellDetail()
    {
        string detail = "Move forward and deals damage to + " + myAttackTag + "s that are hit. Destroyed after " + myLifeTime.ToString("0.0") + " secodnds or hitting a wall";

        return detail;
    }
}
