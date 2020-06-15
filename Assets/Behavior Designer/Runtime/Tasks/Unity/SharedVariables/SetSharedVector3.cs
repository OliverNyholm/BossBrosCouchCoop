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

        public bool myShouldSnapToGround = false;

        public override TaskStatus OnUpdate()
        {
            if (sharedGameObject.Value == null)
                targetVariable.Value = targetValue.Value;
            else
                targetVariable.Value = sharedGameObject.Value.transform.position;

            if(myShouldSnapToGround)
            {
                Ray ray = new Ray(targetVariable.Value + Vector3.up, Vector3.down);
                LayerMask layerMask = LayerMask.GetMask("Terrain");

                RaycastHit hitInfo;
                if (Physics.Raycast(ray, out hitInfo, 100.0f, layerMask))
                {
                    targetVariable.Value = hitInfo.point + Vector3.up * 0.01f;
                }
            }

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            targetValue = Vector3.zero;
            targetVariable = Vector3.zero;
        }
    }
}