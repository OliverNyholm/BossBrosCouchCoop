using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Meditate : Spell {

    [SerializeField]
    private int myRegenerationPercentage;

    [SerializeField]
    private float myChannelTime;

    private float myIntervalTimer;
    private float myCurrentIntervalTimer;
    private int myResourcePerTick;

    private void Start()
    {
        if (!isServer)
            return;
        
        const int NrOfTicks = 10;

        myIntervalTimer = NrOfTicks / myChannelTime;
        myCurrentIntervalTimer = 0.0f;

        myResourcePerTick = myParent.GetComponent<Resource>().MaxResource / myRegenerationPercentage;

        myChannelTime += 0.02f;

        RpcStartCoroutine();
    }

    protected override void Update()
    {
        if (!isServer)
            return;

        myCurrentIntervalTimer += Time.deltaTime;
        if(myCurrentIntervalTimer >= myIntervalTimer)
        {
            myCurrentIntervalTimer -= myIntervalTimer;
            myParent.GetComponent<Resource>().GainResource(myResourcePerTick); ;
        }

        myChannelTime -= Time.deltaTime;
        if (myChannelTime <= 0.0f)
            NetworkServer.Destroy(gameObject);
    }

    [ClientRpc]
    private void RpcStartCoroutine()
    {
        myParent.GetComponent<PlayerCharacter>().StartChannel(myChannelTime, this, null);
    }

    protected override string GetSpellDetail()
    {
        string detail = "to regenerate " + myRegenerationPercentage + " % of your mana over a period of " + myChannelTime + " seconds. You are stunned for the whole duration";

        return detail;
    }
}
