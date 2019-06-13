using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AoeAttack : Spell {

    private float myLifeTime = 0.05f;

    protected override void Update()
    {
        myLifeTime -= Time.deltaTime;
        if (myLifeTime <= 0.0f)
            Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            if (myDamage > 0.0f)
                other.GetComponent<Health>().TakeDamage(myDamage);
        }
    }

    protected override string GetSpellDetail()
    {
        string detail = "to deal damage to everyone around you for " + myDamage + " damage";

        return detail;
    }
}
