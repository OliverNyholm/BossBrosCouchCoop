using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meditate : Spell
{

    [SerializeField]
    private int myRegenerationPercentage = 1;

    [SerializeField]
    private float myChannelTime;

    private float myIntervalTimer;
    private float myCurrentIntervalTimer;
    private int myResourcePerTick;
    
    protected override void Start()
    {
        const int NrOfTicks = 10;

        myIntervalTimer = myChannelTime / NrOfTicks;
        myCurrentIntervalTimer = 0.0f;

        myResourcePerTick = (int)(myParent.GetComponent<Resource>().MaxResource * (myRegenerationPercentage * 0.01f));
        myResourcePerTick /= NrOfTicks;

        myChannelTime += 0.02f;

        StartCoroutine();
        SpawnVFX(myChannelTime + 1.5f);
    }

    protected override void Update()
    {
        myCurrentIntervalTimer += Time.deltaTime;
        if (myCurrentIntervalTimer >= myIntervalTimer)
        {
            myCurrentIntervalTimer -= myIntervalTimer;
            myParent.GetComponent<Resource>().GainResource(myResourcePerTick);
        }

        myChannelTime -= Time.deltaTime;
        if (myChannelTime <= 0.0f)
            Destroy(gameObject);
    }

    private void StartCoroutine()
    {
        myParent.GetComponent<Player>().StartChannel(myChannelTime, this, null, myChannelTime);
    }

    protected override string GetSpellDetail()
    {
        string detail = "to regenerate " + myRegenerationPercentage + " % of your mana over a period of " + myChannelTime + " seconds. You are stunned for the whole duration";

        return detail;
    }
}
