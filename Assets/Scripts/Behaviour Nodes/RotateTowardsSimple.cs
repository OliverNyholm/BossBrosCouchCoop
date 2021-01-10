using UnityEngine;
using UnityEngine.AI;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class RotateTowardsSimple : Action
{
    public SharedGameObject mySharedTarget;
    public GameObject myGameObject;
    public Vector3 myWorldPosition;

    private Quaternion myOriginalRotation;
    private Quaternion myTargetRotation;

    public float myRotateDuration = 2.0f;
    private float myRotateTimer = 0.0f;

    public override void OnStart()
    {
        Vector3 targetLocation;
        if (mySharedTarget.Value != null)
            targetLocation = mySharedTarget.Value.transform.position;
        else if (myGameObject)
            targetLocation = myGameObject.transform.position;
        else
            targetLocation = myWorldPosition;

        myOriginalRotation = transform.rotation;
        myTargetRotation = Quaternion.LookRotation((targetLocation - transform.position).Normalized2D(), Vector3.up);

        myRotateTimer = 0.0f;
    }

    public override TaskStatus OnUpdate()
    {
        myRotateTimer += Time.deltaTime;

        transform.rotation = Quaternion.Lerp(myOriginalRotation, myTargetRotation, myRotateTimer / myRotateDuration);
        if (myRotateTimer >= myRotateDuration)
        {
            transform.rotation = myTargetRotation;
            return TaskStatus.Success;
        }

        return TaskStatus.Running;
    }
}
