using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class PlayersInSight : Conditional
{
    public float myRange;

    public string myTargetTag;

    // A cache of all of the possible targets
    private Transform[] possibleTargets;

    public override void OnAwake()
    {
        var targets = GameObject.FindGameObjectsWithTag(myTargetTag);
        possibleTargets = new Transform[targets.Length];
        for (int i = 0; i < targets.Length; ++i)
        {
            possibleTargets[i] = targets[i].transform;
        }
    }

    public override TaskStatus OnUpdate()
    {
        for (int i = 0; i < possibleTargets.Length; ++i)
        {
            if (InSight(possibleTargets[i]))
            {
                GetComponent<NPCThreatComponent>().PlayerSpotted(possibleTargets[i].gameObject);
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
