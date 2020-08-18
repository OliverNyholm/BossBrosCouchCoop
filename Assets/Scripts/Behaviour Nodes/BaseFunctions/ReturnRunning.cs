namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskDescription("The return running task will always return running.")]
    public class ReturnRunning : Decorator
    {
        // The status of the child after it has finished running.
        private TaskStatus executionStatus = TaskStatus.Inactive;

        public override bool CanExecute()
        {
            // Continue executing until the child task returns success or failure.
            return executionStatus == TaskStatus.Inactive || executionStatus == TaskStatus.Running;
        }

        public override void OnChildExecuted(TaskStatus childStatus)
        {
            // Update the execution status after a child has finished running.
            executionStatus = childStatus;
        }

        public override TaskStatus Decorate(TaskStatus status)
        {
            return TaskStatus.Running;
        }

        public override void OnEnd()
        {
            // Reset the execution status back to its starting values.
            executionStatus = TaskStatus.Inactive;
        }
    }
}