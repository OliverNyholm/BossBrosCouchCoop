using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.SharedVariables
{
    [TaskCategory("Unity/SharedVariable")]
    [TaskDescription("Sets the SharedVector3 variable to the specified object. Returns Success.")]
    public class SetSharedVector3 : Action
    {
        [Tooltip("The GameObject to set the SharedVector3 to. (Ignores targetValue)")]
        public SharedGameObject sharedGameObject;
        [Tooltip("The Vector3 to set the SharedVector3 to. (Ignores sharedGameObject)")]
        public SharedVector3 targetValue;
        [RequiredField]
        [Tooltip("The SharedVector3 to set")]
        public SharedVector3 targetVariable;

        public override TaskStatus OnUpdate()
        {
            if (sharedGameObject.Value == null)
                targetVariable.Value = targetValue.Value;
            else
                targetVariable.Value = sharedGameObject.Value.transform.position;

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            targetValue = Vector3.zero;
            targetVariable = Vector3.zero;
        }
    }
}