using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class PlayersInSight : Conditional
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
            if (InSight(myThreatComponent.Players[index].transform))
            {
                GetComponent<NPCThreatComponent>().PlayerSpotted(myThreatComponent.Players[index].gameObject);
                return TaskStatus.Success;
            }
        }
        return TaskStatus.Failure;
    }
    

    public bool InSight(Transform targetTransform)
    {
        if (targetTransform.GetComponent<Health>().IsDead())
            return false;

        Vector3 direction = targetTransform.position - transform.position;
        if(direction.sqrMagnitude > myRange * myRange)
            return false;

        float distance = direction.magnitude;

        Ray ray = new Ray(transform.position, direction / distance);
        LayerMask layerMask = LayerMask.GetMask("Terrain");

        if (Physics.Raycast(ray, distance, layerMask))
        {
            return false;
        }

        return true;
    }
}
