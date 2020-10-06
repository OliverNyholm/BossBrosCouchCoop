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
   private ThreatData[] myThreatData = new ThreatData[32];
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

        myInternalArrayCounter++;
    }

    public void RemoveThreat(int anIndex)
    {
        myThreatData[anIndex] = myThreatData[myInternalArrayCounter - 1];
        myInternalArrayCounter--;
    }
}
