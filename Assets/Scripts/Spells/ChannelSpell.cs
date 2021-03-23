using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChannelSpell : Spell
{
    [SerializeField]
    protected float myChannelTime = 5.0f;

    public virtual void OnStoppedChannel()
    {
        if (mySpellSFX.myChannelEventStop != "")
            AkSoundEngine.PostEvent(mySpellSFX.myChannelEventStop, myParent);
    }
}
