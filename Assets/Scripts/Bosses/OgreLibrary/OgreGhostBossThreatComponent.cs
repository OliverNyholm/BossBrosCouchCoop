using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OgreGhostBossThreatComponent : NPCThreatComponent
{
    List<bool> myArePlayersOnSamePlatform = new List<bool>(4);

    protected override void Awake()
    {
        base.Awake();
        for (int index = 0; index < 4; index++)
            myArePlayersOnSamePlatform.Add(false);
    }

    protected override void Update()
    {
        base.Update();

        GameObject platform = null;
        if (UtilityFunctions.FindGroundFromLocation(transform.position, out RaycastHit selfHitInfo, out _))
        {
            platform = selfHitInfo.collider.gameObject;
        }

        for (int index = 0; index < Players.Count; index++)
        {
            PlayerMovementComponent movementComponent = Players[index].GetComponent<PlayerMovementComponent>();
            if (movementComponent && movementComponent.IsMovementDisabled())
            {
                myArePlayersOnSamePlatform[index] = false;
                continue;
            }

            if (UtilityFunctions.FindGroundFromLocation(Players[index].transform.position, out RaycastHit playerHitInfo, out _))
                myArePlayersOnSamePlatform[index] = playerHitInfo.collider.gameObject == platform;
            else
                myArePlayersOnSamePlatform[index] = false;
        }
    }

    public override bool ShouldIgnoreTarget(GameObject aTarget)
    {
        if (base.ShouldIgnoreTarget(aTarget))
            return true;

        PlayerMovementComponent movementComponent = aTarget.GetComponent<PlayerMovementComponent>();
        if (movementComponent && movementComponent.IsMovementDisabled())
            return true;

        if (!myArePlayersOnSamePlatform[aTarget.GetComponent<Player>().PlayerIndex - 1])
            return true;

        return false;
    }
}
