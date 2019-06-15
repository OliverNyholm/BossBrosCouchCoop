using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoAttack : Spell
{
    public override void AddDamageIncrease(float aDamageIncrease)
    {
        myDamage = (int)(myParent.GetComponent<Stats>().myAutoAttackDamage * aDamageIncrease);
    }
}
