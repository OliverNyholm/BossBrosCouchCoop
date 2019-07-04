using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class SpawnSpell : Action
{
    public SharedGameObject myTarget;
    public GameObject mySpell;

    public SharedVector3 mySpawnPosition;

    public override TaskStatus OnUpdate()
    {
        GetComponent<Enemy>().SpawnSpell(mySpell, myTarget.Value, mySpawnPosition.Value);

        return TaskStatus.Success;
    }
}