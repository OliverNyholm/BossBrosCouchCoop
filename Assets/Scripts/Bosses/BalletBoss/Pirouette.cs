using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pirouette : SpellOverTime
{
    private List<GameObject> myPlayers = new List<GameObject>(4);

    public override void Restart()
    {
        base.Restart();

        myPlayers = myParent.GetComponent<NPCThreatComponent>().Players;
    }

    protected override void DealTickEffect()
    {
        foreach (GameObject player in myPlayers)
        {
            if (UtilityFunctions.IsCharacterInRangeAndAlive(player, transform.position, myRange))
                DealDamage(myDamage, player.transform.position, player);
        }
    }
}