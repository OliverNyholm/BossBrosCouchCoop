using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ThreatData
{
    public float myThreatAmount;
    public float myExpirationTime;
}

public class ThreatHolder
{
   private const int MaxThreatSize = 32;
   private ThreatData[] myThreatData = new ThreatData[MaxThreatSize];
   private float myThreatLifetime = 10.0f;

    private int myInternalArrayCounter = 0;

    public void ClearAllThreat()
    {
        myInternalArrayCounter = 0;
    }

    public float CalculateAndTrimThreatAmount(float aTimeNow)
    {
        float threatSum = 0.0f;

        for (int index = 0; index < myInternalArrayCounter; index++)
        {
            if(myThreatData[index].myExpirationTime <= aTimeNow)
            {
                RemoveThreat(index);
                if (myInternalArrayCounter <= index)
                    break;
            }

            threatSum += myThreatData[index].myThreatAmount;
        }

        return threatSum;
    }

    public void AddThreat(float aThreatAmount)
    {
        myThreatData[myInternalArrayCounter].myThreatAmount = aThreatAmount;
        myThreatData[myInternalArrayCounter].myExpirationTime = Time.time + myThreatLifetime;

        if(myInternalArrayCounter < MaxThreatSize - 1)
            myInternalArrayCounter++;
    }

    public void RemoveThreat(int anIndex)
    {
        myThreatData[anIndex] = myThreatData[myInternalArrayCounter - 1];
        myInternalArrayCounter--;
    }
}
