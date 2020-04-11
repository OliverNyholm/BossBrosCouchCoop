using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class IsEngaged : Conditional
{    public override TaskStatus OnUpdate()
    {
        if (GetComponent<NPCComponent>().State == NPCComponent.CombatState.Combat)
            return TaskStatus.Success;

        return TaskStatus.Failure;
    }
}
