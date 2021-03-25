using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChickenFarmerThrowEgg : Spell
{
    [SerializeField]
    private SpellOverTime myRequiredEggBuff = null;

    public override bool IsSpellAvailable(GameObject aCaster)
    {
        Stats stats = aCaster.GetComponent<Stats>();
        if (!stats)
            return false;

        return stats.HasSpellOverTime(myRequiredEggBuff);
    }

    protected override void OnFirstUpdate()
    {
        base.OnFirstUpdate();

        Stats stats = myParent.GetComponent<Stats>();
        if (stats)
            stats.RemoveSpellOverTimeStack(myRequiredEggBuff);
    }
}
