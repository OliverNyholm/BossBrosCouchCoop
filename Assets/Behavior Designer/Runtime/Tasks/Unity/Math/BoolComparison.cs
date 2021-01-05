namespace BehaviorDesigner.Runtime.Tasks.Unity.Math
{
    [TaskCategory("Unity/Math")]
    [TaskDescription("Performs a comparison between two bools.")]
    public class BoolComparison : Conditional
    {
        [Tooltip("The first bool")]
        public SharedBool bool1;
        [Tooltip("The second bool")]
        public SharedBool bool2;

        public override TaskStatus OnUpdate()
        {
            return bool1.Value == bool2.Value ? TaskStatus.Success : TaskStatus.Failure;
        }

        public override void OnReset()
        {
            if (bool1 != null)
                bool1.Value = false;
            if (bool2 != null)
                bool2.Value = false;
        }
    }
}