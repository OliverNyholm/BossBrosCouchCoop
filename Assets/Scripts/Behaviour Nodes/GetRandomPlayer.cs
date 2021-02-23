using UnityEngine;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class GetRandomPlayer : Action
{
    public SharedGameObject myGameObject;
    public SharedInt mySelectedIndex;
    public bool myShouldExcludePlayer;
    public bool myAcceptExludedPlayerIfAlone = true;
    public SharedInt myPlayerIndexToExclude;

    public SharedGameObjectList myPlayers;
    private List<int> myAvailablePlayerIndices = new List<int>(4);

    public override TaskStatus OnUpdate()
    {
        List<GameObject> players;
        if(myPlayers.Value != null && myPlayers.Value.Count != 0)
            players = myPlayers.Value;
        else
            players = GetComponent<NPCThreatComponent>().Players;

        if (players.Count == 0)
            return TaskStatus.Failure;

        myAvailablePlayerIndices.Clear();

        bool isExludedPlayerAlive = false;
        for (int index = 0; index < players.Count; index++)
        {
            if (players[index].GetComponent<Health>().IsDead())
                continue;

            if (myShouldExcludePlayer && index == myPlayerIndexToExclude.Value)
            {
                isExludedPlayerAlive = true;
                continue;
            }

            myAvailablePlayerIndices.Add(index);
        }

        if (myAvailablePlayerIndices.Count == 0 && isExludedPlayerAlive && myAcceptExludedPlayerIfAlone)
            myAvailablePlayerIndices.Add(myPlayerIndexToExclude.Value);

        if (myAvailablePlayerIndices.Count == 0)
            return TaskStatus.Failure;

        int randomPlayer = Random.Range(0, myAvailablePlayerIndices.Count);

        myGameObject.Value = players[randomPlayer];
        mySelectedIndex.Value = myGameObject.Value.GetComponent<Player>().PlayerIndex - 1;

        return TaskStatus.Success;
    }
}
