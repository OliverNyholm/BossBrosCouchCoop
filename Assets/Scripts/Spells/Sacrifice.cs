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

        myParent.GetComponent<Health>().TakeDamage(healthToCast, myParent.GetComponent<UIComponent>().myCharacterColor, transform.position);
        myTarget.GetComponent<Health>().GainHealth(healthToCast);
        PostMaster.Instance.PostMessage(new Message(MessageCategory.SpellSpawned, new MessageData(myParent.GetInstanceID(), myDamage)));
    }
}
