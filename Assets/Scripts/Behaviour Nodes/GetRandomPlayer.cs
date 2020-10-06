using UnityEngine;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class GetRandomPlayer : Action
{
    public SharedGameObject myGameObject;
    public bool myShouldExcludePlayer;
    public SharedInt myPlayerIndexToExclude;

    public SharedGameObjectList myPlayers;


    public override TaskStatus OnUpdate()
    {
        List<GameObject> players;
        if(myPlayers.Value != null && myPlayers.Value.Count != 0)
            players = myPlayers.Value;
        else
            players = GetComponent<NPCThreatComponent>().Players;


        if (players.Count == 0)
            return TaskStatus.Failure;

        if(players.Count == 1)
        {
            myGameObject.Value = players[0];
            return TaskStatus.Success;
        }

        int randomPlayer = -1;
        do
        {
            randomPlayer = Random.Range(0, players.Count);
        } while (ShouldRepickRandom(randomPlayer));

        myGameObject.Value = players[randomPlayer];

        return TaskStatus.Success;
    }

    private bool ShouldRepickRandom(int aRandomIndex)
    {
        int deathCount = 0;
        for (int index = 0; index < myPlayers.Value.Count; index++)
        {
            if (!myPlayers.Value[index])
            {
                if (aRandomIndex == index)
                    return true;
                else
                    continue;
            }

            if(myPlayers.Value[index].GetComponent<Health>().IsDead())
            {
                deathCount++;
                if (aRandomIndex == index)
                    return true;
            }
        }

        //If we don't care about who we pick as random and current target isn't dead, go ahead.
        if (!myShouldExcludePlayer)
            return false;

        //If we care about excluding, but our current target is the only one alive we'll go with it anyway.
        if (deathCount == myPlayers.Value.Count - 1)
            return false;

        return true;
    }
}
