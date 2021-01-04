using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OgreGhostBossThreatComponent : NPCThreatComponent
{
    [SerializeField]
    private float myMaxThreatDistance = 8.0f;

    public override bool ShouldIgnoreTarget(GameObject aTarget)
    {
        if (base.ShouldIgnoreTarget(aTarget))
            return true;

        PlayerMovementComponent movementComponent = aTarget.GetComponent<PlayerMovementComponent>();
        if (movementComponent && movementComponent.IsMovementDisabled())
            return true;            

        float distanceSqr = (aTarget.transform.position - transform.position).sqrMagnitude;
        if (distanceSqr > myMaxThreatDistance * myMaxThreatDistance)
            return true;

        return false;
    }
}
