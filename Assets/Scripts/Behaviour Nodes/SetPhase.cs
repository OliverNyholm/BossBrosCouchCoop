using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class SetPhase : Action
{
    public int myPhaseIndex;

    public override TaskStatus OnUpdate()
    {
        GetComponent<NPCComponent>().PhaseIndex = myPhaseIndex;
        Owner.SendEvent("PhaseChanged");
        return TaskStatus.Success;
    }
}
