using UnityEngine;
using System;

public class DpadInput
{
    private bool[] myInputState;
    private bool[] myPreviousInputState;
    private int myControllerIndex;

    public enum DPADButton
    {
        Up = 0,
        Right = 1,
        Down = 2,
        Left = 3
    };

    public DpadInput(int aControllerIndex)
    {
        myControllerIndex = aControllerIndex;

        myInputState = new bool[4];
        myPreviousInputState = new bool[4];
    }

    public void Update()
    {
        Array.Copy(myInputState, 0, myPreviousInputState, 0, 4);

        int horizontalAxis = (int)Input.GetAxisRaw("DpadHorizontal" + myControllerIndex);
        int verticalAxis = (int)Input.GetAxisRaw("DpadVertical" + myControllerIndex);

        myInputState[(int)DPADButton.Up] = verticalAxis > 0.0 ? true : false;
        myInputState[(int)DPADButton.Right] = horizontalAxis > 0.0 ? true : false;
        myInputState[(int)DPADButton.Down] = verticalAxis < 0.0 ? true : false;
        myInputState[(int)DPADButton.Left] = horizontalAxis < 0.0 ? true : false;
    }

    public bool IsDPADDown(DPADButton aDpad)
    {
        if (myInputState[(int)aDpad])
            return true;

        return false;
    }

    public bool IsDPADPressed(DPADButton aDpad)
    {
        if (myInputState[(int)aDpad] == true && myPreviousInputState[(int)aDpad] == false)
            return true;

        return false;
    }

    public bool IsDPADReleased(DPADButton aDpad)
    {
        if (myInputState[(int)aDpad] == false && myPreviousInputState[(int)aDpad] == true)
            return true;

        return false;
    }
}
