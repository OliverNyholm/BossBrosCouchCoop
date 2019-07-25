using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PercentageSpell : Spell
{
    protected override void Start()
    {
        myDamage = (int)(myTarget.GetComponent<Health>().MaxHealth * ((float)myDamage / 100));
    }
}
