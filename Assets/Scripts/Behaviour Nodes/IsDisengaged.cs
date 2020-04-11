using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class IsDisengaged : Conditional
{
    public override TaskStatus OnUpdate()
    {
        if (GetComponent<NPCComponent>().State == NPCComponent.CombatState.Disengage)
            return TaskStatus.Success;

        return TaskStatus.Failure;
    }
}
