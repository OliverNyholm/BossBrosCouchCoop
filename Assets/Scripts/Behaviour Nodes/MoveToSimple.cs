using UnityEngine;
using UnityEngine.AI;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskDescription("Will move to one of the set values in the inspector.")]
public class MoveToSimple : Action
{
    public SharedGameObject myTarget;
    public float mySpeed = 5.0f;
    public float myRotationSpeed = 5.0f;

    public GameObject myGameObject;
    public Vector3 myWorldPosition;

    public float myStopDistance = 0.1f;

    [BehaviorDesigner.Runtime.Tasks.Tooltip("When reaching target, rotates same directon as gameobject")]
    public bool myShouldApplyGameObjectRotation = false;

    private enum TargetType
    {
        SharedGameObject,
        GameObject,
        WorldPosition
    }
    private TargetType myTargetType;

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
        Quaternion targetRotation = Quaternion.identity;
        if (myTargetType == TargetType.SharedGameObject)
        {
            targetPosition = myTarget.Value.transform.position;
        }
        if (myTargetType == TargetType.GameObject)
        {
            targetPosition = myGameObject.transform.position;
            if (myShouldApplyGameObjectRotation)
                targetRotation = myGameObject.transform.rotation;
        }

        Vector3 toTarget = (new VectorXZ(targetPosition) - new VectorXZ(transform.position));
        float distanceToTarget = toTarget.magnitude;
        toTarget /= distanceToTarget;

        transform.position += toTarget * mySpeed * Time.deltaTime;

        Quaternion lookRotation = Quaternion.LookRotation(toTarget, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, myRotationSpeed * Time.deltaTime);

        if (distanceToTarget <= myStopDistance)
        {
            if (myShouldApplyGameObjectRotation)
                transform.rotation = targetRotation;

            return TaskStatus.Success;
        }

        return TaskStatus.Running;
    }
}
