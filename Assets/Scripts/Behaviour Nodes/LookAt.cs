using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class LookAt : Action
{
    public SharedGameObject myTarget;

    public override TaskStatus OnUpdate()
    {
        Vector3 lookDirection = (myTarget.Value.transform.position - transform.position);
        lookDirection.y = 0.0f;
        transform.rotation = Quaternion.LookRotation(lookDirection.normalized);

        return TaskStatus.Success;
    }
}
