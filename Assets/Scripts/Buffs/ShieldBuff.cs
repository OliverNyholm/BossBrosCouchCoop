using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Buff", menuName = "Buff/Shield")]
public class ShieldBuff : Buff
{
    public int myShieldValue;
    private int myCurrentShieldValue;

    public override void ApplyBuff(ref Stats aStats)
    {
        base.ApplyBuff(ref aStats);

        myCurrentShieldValue = myShieldValue;
    }

    public override BuffSpell InitializeBuff(GameObject aParent)
    {
        return new BuffShieldSpell(this, aParent);
    }
}
