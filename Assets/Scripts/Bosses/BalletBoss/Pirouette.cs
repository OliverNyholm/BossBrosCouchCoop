using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pirouette : SpellOverTime
{
    protected override void DealTickEffect()
    {
        List<GameObject> playersAndMinions = myTargetHandler.GetPlayersAndMinions();
        foreach (GameObject targets in playersAndMinions)
        {
            if (UtilityFunctions.IsCharacterInRangeAndAlive(targets, transform.position, myRange))
                DealDamage(myDamage, targets.transform.position, targets);
        }
    }
}