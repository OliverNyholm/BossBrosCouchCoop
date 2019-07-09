using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskDescription("Will set the vector3 to the sum of all references added.")]
public class SetTransform : Action
{
    [BehaviorDesigner.Runtime.Tasks.Tooltip("Transform to set")]
    public SharedTransform myTransform;

    [BehaviorDesigner.Runtime.Tasks.Tooltip("Other Variable to get position from in behaviour tree")]
    public SharedGameObject mySharedGameObjectPosition;

    [BehaviorDesigner.Runtime.Tasks.Tooltip("Vector3 in scene to get position from")]
    public GameObject myGameObjectPosition;

    [BehaviorDesigner.Runtime.Tasks.Tooltip("Vector3 without an object to refer to")]
    public Vector3 myOffsetPosition;

    [BehaviorDesigner.Runtime.Tasks.Tooltip("Rotation without an object to refer to")]
    public Quaternion myOffsetRotation;

    [BehaviorDesigner.Runtime.Tasks.Tooltip("Scale without an object to refer to")]
    public Vector3 myOffsetSize;

    public override TaskStatus OnUpdate()
    {
        Vector3 position = Vector3.zero;
        Quaternion rotation = Quaternion.identity;
        Vector3 size = Vector3.zero;

        if (mySharedGameObjectPosition.Value != null)
        {
            position += mySharedGameObjectPosition.Value.transform.position;
            rotation = mySharedGameObjectPosition.Value.transform.rotation;
            size += mySharedGameObjectPosition.Value.transform.localScale;
        }

        if (myGameObjectPosition != null)
        {
            position += myGameObjectPosition.transform.position;
            rotation = myGameObjectPosition.transform.rotation * rotation;
            size += myGameObjectPosition.transform.localScale;
        }

        position += myOffsetPosition;
        //rotation *= myOffsetRotation;
        size += myOffsetSize;

        myTransform.Value.position = position;
        myTransform.Value.rotation = rotation;
        myTransform.Value.localScale = size;

        return TaskStatus.Success;
    }
}
