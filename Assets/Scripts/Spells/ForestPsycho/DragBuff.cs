using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragBuff : SpellOverTime
{
    private Vector3 myDragDirection;
    private Transform myDragOrigin = null;

    //If true, drag is away from Origin, else towards
    private bool myIsPushFromOrigin = false;

    public void SetDragDirection(Vector3 aDirection)
    {
        myDragDirection = aDirection;
    }

    public void SetDragOrigin(Transform aOrigin)
    {
        myDragOrigin = aOrigin;
    }

    protected override void Update()
    {
        base.Update();

        Vector3 dragDirection = myDragDirection;
        if (myDragOrigin)
        {
            dragDirection = (myDragOrigin.position - myParent.transform.position).normalized * mySpeed * Time.deltaTime;
            dragDirection *= myIsPushFromOrigin ? -1.0f : 1.0f;
        }
        else
        {
            dragDirection *= mySpeed * Time.deltaTime;
        }

        myParent.transform.position += dragDirection;
    }
}
