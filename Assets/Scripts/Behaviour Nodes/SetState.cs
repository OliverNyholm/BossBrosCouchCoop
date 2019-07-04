using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

public class SetState : Action
{
    [Header("State to changed to")]
    public Enemy.CombatState myState;

    public override TaskStatus OnUpdate()
    {
        GetComponent<Enemy>().SetState(myState);
        return TaskStatus.Success;
    }
}
