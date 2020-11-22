using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControlsJoystickMovedHelper
{
    private PlayerControls myPlayerControls;

    private bool[] myCurrentState = new bool[4]; //left, up, down, right
    private bool[] myPreviousState = new bool[4];
    private float myTriggerZone;
    private float myResetZone;

    public PlayerControlsJoystickMovedHelper(PlayerControls aPlayerControls, float aTriggerZone, float aResetZone)
    {
        myPlayerControls = aPlayerControls;
        myTriggerZone = aTriggerZone;
        myResetZone = aResetZone;
    }

    public void Update()
    {
        myCurrentState.CopyTo(myPreviousState, 0);

        if (myPlayerControls.Movement.X > -myResetZone) //Left
            myCurrentState[0] = false;
        else if (myPlayerControls.Movement.X < myTriggerZone)
            myCurrentState[0] = true;

        if (myPlayerControls.Movement.Y < myResetZone) //Up
            myCurrentState[1] = false;
        else if (myPlayerControls.Movement.Y > myTriggerZone)
            myCurrentState[1] = true;

        if (myPlayerControls.Movement.Y > -myResetZone) //Down
            myCurrentState[2] = false;
        else if (myPlayerControls.Movement.Y < -myTriggerZone)
            myCurrentState[2] = true;

        if (myPlayerControls.Movement.X < myResetZone) //Right
            myCurrentState[3] = false;
        else if (myPlayerControls.Movement.X > myTriggerZone)
            myCurrentState[3] = true;
    }

    /// <summary>
    /// InputArgument aDirection is 0 for left, 1 for up, 2 for down and 3 for right
    /// </summary>
    /// <param name="aDirection"></param>
    /// <returns></returns>
    public bool WasPressed(int aDirection)
    {
        return myCurrentState[aDirection] && !myPreviousState[aDirection];
    }

    /// <summary>
    /// InputArgument aDirection is 0 for left, 1 for up, 2 for down and 3 for right
    /// </summary>
    /// <param name="aDirection"></param>
    /// <returns></returns>
    public bool WasReleased(int aDirection)
    {
        return !myCurrentState[aDirection] && myPreviousState[aDirection];
    }
}
