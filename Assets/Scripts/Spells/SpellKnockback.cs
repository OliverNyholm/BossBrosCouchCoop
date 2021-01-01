using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellKnockback : Spell
{

    [SerializeField]
    private Vector3 myKnockbackVelocity = Vector3.zero;

    protected Vector3 myKnockbackHorizontalDirection;

    protected override void DealSpellEffect()
    {
        base.DealSpellEffect();

        Vector3 toTarget = (myTarget.transform.position - myParent.transform.position).Normalized2D();
        Vector3 finalKnockbackVelocity = Quaternion.LookRotation(toTarget) * myKnockbackVelocity;

        float knockbackModifier = myParent.GetComponent<Stats>().GetAndResetSpellModifier();
        myTarget.GetComponent<PlayerMovementComponent>().GiveImpulse(finalKnockbackVelocity * knockbackModifier, myStunDuration * knockbackModifier);

        myKnockbackHorizontalDirection = finalKnockbackVelocity.Normalized2D();
    }
}
