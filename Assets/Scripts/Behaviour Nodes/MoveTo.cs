using UnityEngine;
using UnityEngine.AI;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskDescription("Will move to one of the set values in the inspector.")]
public class MoveTo : Action
{
    public SharedGameObject myTarget;

    public GameObject myGameObject;
    public Vector3 myWorldPosition;

    public float myStopDistance = 0.1f;

    private NavMeshAgent myNavmeshAgent;
    private Animator myAnimator;

    private enum TargetType
    {
        SharedGameObject,
        GameObject,
        WorldPosition
    }
    private TargetType myTargetType;

    public override void OnAwake()
    {
        myNavmeshAgent = GetComponent<NavMeshAgent>();
        myAnimator = GetComponent<Animator>();
    }

    public override void OnStart()
    {
        if (myTarget.Value != null)
            myTargetType = TargetType.SharedGameObject;
        else if (myGameObject)
            myTargetType = TargetType.GameObject;
        else
            myTargetType = TargetType.WorldPosition;
    }

    public override TaskStatus OnUpdate()
    {
        Vector3 targetPosition = myWorldPosition;
        if (myTargetType == TargetType.SharedGameObject)
        {
            targetPosition = myTarget.Value.transform.position;
        }
        if (myTargetType == TargetType.GameObject)
        {
            targetPosition = myGameObject.transform.position;
        }

        myNavmeshAgent.destination = targetPosition;
        float distanceSqr = (targetPosition - transform.position).sqrMagnitude;
        if (distanceSqr <= myStopDistance * myStopDistance)
            return TaskStatus.Success;

        return TaskStatus.Running;
    }
}
