using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;
using UnityEngine;

public class ResetGameObject : Action
{
    public SharedGameObject myGameObjectToReset;

    public override TaskStatus OnUpdate()
    {
        myGameObjectToReset.Value = null;
        return TaskStatus.Success;
    }
}
