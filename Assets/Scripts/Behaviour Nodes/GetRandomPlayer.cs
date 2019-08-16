using UnityEngine;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class GetRandomPlayer : Action
{
    public SharedGameObject myGameObject;
    public bool myShouldExcludePlayer;
    public SharedInt myPlayerIndexToExclude;

    public override TaskStatus OnUpdate()
    {
        List<GameObject> players = GetComponent<Enemy>().Players;
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
        } while (myShouldExcludePlayer && randomPlayer == myPlayerIndexToExclude.Value);

        myGameObject.Value = players[randomPlayer];

        return TaskStatus.Success;
    }
}
