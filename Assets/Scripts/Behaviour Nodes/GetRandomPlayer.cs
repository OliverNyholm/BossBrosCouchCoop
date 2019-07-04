using UnityEngine;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class GetRandomPlayer : Action
{
    public SharedGameObject myGameObject;

    public override TaskStatus OnUpdate()
    {
        List<GameObject> players = GetComponent<Enemy>().myPlayers;
        if (players.Count == 0)
            return TaskStatus.Failure;

        int randomPlayer = Random.Range(0, players.Count);
        myGameObject.Value = players[randomPlayer];

        return TaskStatus.Success;
    }
}
