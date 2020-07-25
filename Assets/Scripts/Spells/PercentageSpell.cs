using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PercentageSpell : Spell
{
    public override void Restart()
    {
        myDamage = (int)(myTarget.GetComponent<Health>().MaxHealth * ((float)myDamage / 100));
    }
}
