using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class IsDisengaged : Conditional
{
    public override TaskStatus OnUpdate()
    {
        if (GetComponent<Enemy>().State == Enemy.CombatState.Disengage)
            return TaskStatus.Success;

        return TaskStatus.Failure;
    }
}
