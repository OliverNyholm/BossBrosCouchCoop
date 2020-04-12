using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meditate : Spell
{

    [SerializeField]
    private int myRegenerationPercentage = 1;

    [SerializeField]
    private float myChannelTime = 1.0f;

    private float myIntervalTimer;
    private float myCurrentIntervalTimer;
    private int myResourcePerTick;

    private GameObject myVFX;

    protected override void Start()
    {
        const int NrOfTicks = 10;

        myIntervalTimer = myChannelTime / NrOfTicks;
        myCurrentIntervalTimer = 0.0f;

        myResourcePerTick = (int)(myParent.GetComponent<Resource>().MaxResource * (myRegenerationPercentage * 0.01f));
        myResourcePerTick /= NrOfTicks;

        myChannelTime += 0.02f;

        StartCoroutine();
        myVFX = SpawnVFX(myChannelTime + 1.5f);
    }

    private void OnDisable()
    {
        if (myVFX)
        {
            myVFX.GetComponent<ParticleSystem>().Stop();
            myVFX.transform.parent = null;
        }
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
            ReturnToPool();
    }

    private void StartCoroutine()
    {
        myParent.GetComponent<CastingComponent>().StartChannel(myChannelTime, this, gameObject, 2);
    }

    protected override string GetSpellDetail()
    {
        string detail = "to regenerate " + myRegenerationPercentage + " % of your mana over a period of " + myChannelTime + " seconds. You are stunned for 2 seconds.";

        return detail;
    }
}
