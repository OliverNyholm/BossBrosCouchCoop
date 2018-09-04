using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Buff", menuName = "Buff/Shield")]
public class ShieldBuff : Buff
{
    public int myShieldValue;

    public override BuffSpell InitializeBuff(GameObject aParent)
    {
        return new BuffShieldSpell(this, aParent);
    }
}
