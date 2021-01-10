using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class ResetTarget : Action
{
    private NPCThreatComponent myThreatComponent = null;

    public override void OnAwake()
    {
        base.OnAwake();
        myThreatComponent = GetComponent<NPCThreatComponent>();
    }
    public override TaskStatus OnUpdate()
    {
        myThreatComponent.SetTarget(-1);

        return TaskStatus.Success;
    }
}
