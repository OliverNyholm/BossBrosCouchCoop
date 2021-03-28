using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChickenFarmerFlight : SpellOverTime
{
    [SerializeField]
    private float myOffsetFromGround = 4.0f;
    private float myTargetHeight = 0.0f;

    private float myMoveUpSpeed = 12.0f;

    private PlayerMovementComponent myPlayerMovementComponent = null;

    public override void Reset()
    {
        base.Reset();

        myPlayerMovementComponent = null;
    }

    public override bool IsSpellAvailable(GameObject aCaster)
    {
        ChickenFarmerChickenHandler chickenHandler = aCaster.GetComponent<ChickenFarmerChickenHandler>();
        if (!chickenHandler)
            return false;

        return chickenHandler.GetCurrentChickenCount() > 0;
    }

    protected override void OnFirstUpdate()
    {
        base.OnFirstUpdate();

        myPlayerMovementComponent = myParent.GetComponent<PlayerMovementComponent>();
        if (!myPlayerMovementComponent)
            return;

        myTargetHeight = myPlayerMovementComponent.GetPreviousGroundedPosition().y + myOffsetFromGround;
        myLifeTimeLeft = myParent.GetComponent<ChickenFarmerChickenHandler>().EnableFlightMode();
    }

    protected override void Update()
    {
        base.Update();

        if (!myPlayerMovementComponent || !myPlayerMovementComponent.IsFlying())
            return;

        if (myParent.transform.position.y < myTargetHeight)
            myParent.transform.position += Vector3.up * myMoveUpSpeed * Time.deltaTime;
    }

    private void OnDisable()
    {
        if (myParent)
            myParent.GetComponent<ChickenFarmerChickenHandler>().DisableFlightMode();
    }
}
