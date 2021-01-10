using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalletBossMinion : NPCComponent
{
    private WhirlpoolHandler myWhirlpoolHandler = null;

    protected override void Awake()
    {
        base.Awake();
        myWhirlpoolHandler = FindObjectOfType<WhirlpoolHandler>();
    }

    protected override void OnDeath()
    {
        base.OnDeath();

        myWhirlpoolHandler.AddWhirlpool(this);
    }
}
