using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class IsAnyPlayerNearby : Conditional
{
    public float myRange;

    // A cache of all of the possible targets
    NPCThreatComponent myThreatComponent;

    public override void OnAwake()
    {
        myThreatComponent = GetComponent<NPCThreatComponent>();
    }

    public override TaskStatus OnUpdate()
    {
        for (int index = 0; index < myThreatComponent.Players.Count; ++index)
        {
            if (IsNearby(myThreatComponent.Players[index]))
                return TaskStatus.Success;
        }

        return TaskStatus.Failure;
    }
    

    public bool IsNearby(GameObject aTarget)
    {
        if (myThreatComponent.ShouldIgnoreTarget(aTarget))
            return false;

        float distanceSqr = (aTarget.transform.position - transform.position).sqrMagnitude;
        if(distanceSqr > myRange * myRange)
            return false;

        return true;
    }
}
