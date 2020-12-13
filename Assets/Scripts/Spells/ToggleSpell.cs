using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleSpell : Spell
{
    public virtual void ToggledOn()
    {
        transform.parent = myParent.transform;

        Stats parentStats = myParent.GetComponent<Stats>();
        if(parentStats)
            parentStats.mySpeedMultiplier -= mySpeedWhileCastingReducement;
    }

    public virtual void ToggleOff()
    {
        ReturnToPool();

        Stats parentStats = myParent.GetComponent<Stats>();
        if (parentStats)
            parentStats.mySpeedMultiplier += mySpeedWhileCastingReducement;
    }

    protected override void Update()
    {
    }
}
