using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class IsEngaged : Conditional
{    public override TaskStatus OnUpdate()
    {
        if (GetComponent<Enemy>().State == Enemy.CombatState.Combat)
            return TaskStatus.Success;

        return TaskStatus.Failure;
    }
}
