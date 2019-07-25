using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskDescription(@"The repeater task will repeat execution of its child task until the child task has been run a specified number of times. " +
                      "It has the option of continuing to execute the child task even if the child task returns a failure.")]
    [TaskIcon("{SkinColor}RepeaterIcon.png")]
    public class Repeater : Decorator
    {
        [Tooltip("The number of times to repeat the execution of its child task")]
        public SharedInt count = 1;
        [Tooltip("Allows the repeater to repeat forever")]
        public SharedBool repeatForever;
        [Tooltip("Should the task return if the child task returns a failure")]
        public SharedBool endOnFailure;

        public bool myUseRandomRepeatCount;
        public SharedInt myMinCount = 0;
        public SharedInt myMaxCount = 0;
        private int myRandomCount;

        // The number of times the child task has been run.
        private int executionCount = 0;
        // The status of the child after it has finished running.
        private TaskStatus executionStatus = TaskStatus.Inactive;

        public override void OnStart()
        {
            if(myUseRandomRepeatCount)
            {
                if (myMinCount.Value >= myMaxCount.Value)
                    Debug.LogError("Repeater with random repeat count must have a max count that is larger than min count.");
                myRandomCount = Random.Range(myMinCount.Value, myMaxCount.Value + 1);
            }
        }

        public override bool CanExecute()
        {
            if (myUseRandomRepeatCount)
                return executionCount < myRandomCount;
            // Continue executing until we've reached the count or the child task returned failure and we should stop on a failure.
            return (repeatForever.Value || executionCount < count.Value) && (!endOnFailure.Value || (endOnFailure.Value && executionStatus != TaskStatus.Failure));
        }

        public override void OnChildExecuted(TaskStatus childStatus)
        {
            // The child task has finished execution. Increase the execution count and update the execution status.
            executionCount++;
            executionStatus = childStatus;
        }

        public override void OnEnd()
        {
            // Reset the variables back to their starting values.
            executionCount = 0;
            executionStatus = TaskStatus.Inactive;
        }

        public override void OnReset()
        {
            // Reset the public properties back to their original values.
            count = 0;
            endOnFailure = true;
            myUseRandomRepeatCount = false;
            myMinCount = 0;
            myMaxCount = 0;
        }
    }
}