using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class SetPhase : Action
{
    public int myPhaseIndex;

    public override TaskStatus OnUpdate()
    {
        GetComponent<Enemy>().PhaseIndex = myPhaseIndex;
        return TaskStatus.Success;
    }
}
