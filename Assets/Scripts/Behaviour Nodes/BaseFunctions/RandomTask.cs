using UnityEngine;
using System.Collections.Generic;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskIcon("{SkinColor}RandomSelectorIcon.png")]
    public class RandomTask : Composite
    {
        [Tooltip("Seed the random number generator to make things easier to debug")]
        public int mySeed = 0;
        [Tooltip("Do we want to use the seed?")]
        public bool myUseSeed = false;
        
        private TaskStatus myExecutionStatus = TaskStatus.Inactive;

        private int myRandomChildIndex;

        public override void OnAwake()
        {
            if (myUseSeed)
            {
                Random.InitState(mySeed);
            }
        }

        public override void OnStart()
        {
            myRandomChildIndex = Random.Range(0, Children.Count);
        }

        public override int CurrentChildIndex()
        {
            return myRandomChildIndex;
        }

        public override bool CanExecute()
        {
            return myExecutionStatus != TaskStatus.Failure && myExecutionStatus != TaskStatus.Success;
        }

        public override void OnChildExecuted(TaskStatus childStatus)
        {
            myExecutionStatus = childStatus;
        }

        public override void OnConditionalAbort(int childIndex)
        {
            myRandomChildIndex = childIndex;
            myExecutionStatus = TaskStatus.Inactive;
        }

        public override void OnEnd()
        {
            myExecutionStatus = TaskStatus.Inactive;
            myRandomChildIndex = -1;
        }
    }
}