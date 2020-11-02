using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskDescription("Will set the vector3 to the sum of all references added.")]
public class SetScale : Action
{

    public SharedTransform myObjectToScale;
    [BehaviorDesigner.Runtime.Tasks.Tooltip("Vector3 to set")]
    public SharedVector3 myScale;

    [BehaviorDesigner.Runtime.Tasks.Tooltip("If false, sets scale to MyScale value")]
    public bool myShouldAddScale = false;

    public override TaskStatus OnUpdate()
    {
        if (myObjectToScale.Value == null)
            return TaskStatus.Failure;

        if (myShouldAddScale)
            myObjectToScale.Value.localScale += myScale.Value;
        else
            myObjectToScale.Value.localScale = myScale.Value;

        return TaskStatus.Success;
    }
}