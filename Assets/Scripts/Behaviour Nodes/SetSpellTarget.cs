using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class SetSpellTarget : Action
{
    public SharedGameObject myTarget;

    public override TaskStatus OnUpdate()
    {
        transform.GetComponent<TargetingComponent>().SetSpellTarget(myTarget.Value);

        return TaskStatus.Success;
    }
}
