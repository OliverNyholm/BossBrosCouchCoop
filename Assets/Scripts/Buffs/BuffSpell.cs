using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BuffSpell
{
    protected Buff myBuff;

    protected float myDuration = 0.0f;

    [SyncVar]
    private GameObject myParent;

    public BuffSpell(Buff aBuff, GameObject aParent)
    {
        myBuff = aBuff;
        myParent = aParent;
    }

    public virtual void Tick()
    {
        myDuration += Time.deltaTime;
    }

    public virtual bool IsFinished()
    {
        if (myDuration >= myBuff.myDuration)
            return true;

        return false;
    }

    public Buff GetBuff()
    {
        return myBuff;
    }
}
