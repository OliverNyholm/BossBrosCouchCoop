using UnityEngine;
using UnityEngine.AI;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskDescription("Will move to one of the set values in the inspector.")]
public class MoveToHeightSimple : Action
{
    public SharedGameObject myGameObjectToMove = null;
    public float myHeight = 0.0f;
    public float mySpeed = 1.0f;

    public bool myShouldSnapToHeight = true;

    private Vector3 myDirection = Vector3.up;
    public override void OnStart()
    {
        base.OnStart();

        if (transform.position.y > myHeight)
            myDirection.y = -1.0f;
        else
            myDirection.y = 1.0f;
    }

    public override TaskStatus OnUpdate()
    {
        if (myGameObjectToMove == null || myGameObjectToMove.Value == null)
        {
            Debug.Log("myGameObjectToMove is null, can't move to height");
            return TaskStatus.Failure;
        }
        if (mySpeed <= 0.0f)
        {
            Debug.Log("Speed is 0, can't move to height");
            return TaskStatus.Failure;
        }

        transform.position += myDirection * mySpeed * Time.deltaTime;

        bool reachedTarget = myDirection.y > 0 ? transform.position.y >= myHeight : transform.position.y <= myHeight;
        if (reachedTarget)
        {
            if (myShouldSnapToHeight)
            {
                Vector3 snapPosition = transform.position;
                snapPosition.y = myHeight;
                transform.position = snapPosition;
            }

            return TaskStatus.Success;
        }

        return TaskStatus.Running;
    }
}
