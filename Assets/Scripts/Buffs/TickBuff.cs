using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Buff", menuName = "Buff/Tick")]
public class TickBuff : Buff
{
    public int myTotalDamage;
    public int myNrOfTicks;

    public BuffTickSpell InitializeBuff(GameObject aParent, GameObject aTarget)
    {
        return new BuffTickSpell(this, aParent, aTarget);
    }
}