using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sacrifice : Spell
{
    protected override void DealSpellEffect()
    {
        int healthToCast = myDamage;
        if (myDamage > myParent.GetComponent<Health>().myCurrentHealth)
            healthToCast = myParent.GetComponent<Health>().myCurrentHealth - 1;

        myParent.GetComponent<Health>().TakeDamage(healthToCast);
        myTarget.GetComponent<Health>().GainHealth(healthToCast);
        AIPostMaster.Instance.PostAIMessage(new AIMessage(AIMessageType.SpellSpawned, new AIMessageData(myParent.GetInstanceID(), myDamage)));
    }

    protected override string GetSpellDetail()
    {
        string detail = "to sacrifice " + myDamage + " of your own health, giving it to your target";

        return detail;
    }
}
