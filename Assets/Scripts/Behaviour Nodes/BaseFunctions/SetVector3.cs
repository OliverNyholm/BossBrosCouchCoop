using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskDescription("Will set the vector3 to the sum of all references added.")]
public class SetVector3 : Action
{
    [BehaviorDesigner.Runtime.Tasks.Tooltip("Vector3 to set")]
    public SharedVector3 myVector3;

    [BehaviorDesigner.Runtime.Tasks.Tooltip("Other Variable to get position from in behaviour tree")]
    public SharedGameObject mySharedGameObjectPosition;

    [BehaviorDesigner.Runtime.Tasks.Tooltip("Vector3 in scene to get position from")]
    public GameObject myGameObjectPosition;

    [BehaviorDesigner.Runtime.Tasks.Tooltip("Vector3 without an object to refer to")]
    public Vector3 myOffset;

    public override TaskStatus OnUpdate()
    {
        Vector3 vector3 = Vector3.zero;

        if (mySharedGameObjectPosition.Value != null)
            vector3 += mySharedGameObjectPosition.Value.transform.position;

        if (myGameObjectPosition != null)
            vector3 += myGameObjectPosition.transform.position;

        vector3 += myOffset;

        myVector3.Value = vector3;

        return TaskStatus.Success;
    }
}