using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

public class SetState : Action
{
    [Header("State to changed to")]
    public NPCComponent.CombatState myState;

    public override TaskStatus OnUpdate()
    {
        GetComponent<NPCComponent>().SetState(myState);
        return TaskStatus.Success;
    }
}
