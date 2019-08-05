using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskDescription("Wait a specified amount of time. The task will return running until the task is done waiting. It will return success after the wait time has elapsed.")]
    [TaskIcon("{SkinColor}WaitIcon.png")]
    public class Wait : Action
    {
        [Tooltip("The amount of time to wait")]
        public SharedFloat waitTime = 1;
        [Tooltip("Should the wait be randomized?")]
        public SharedBool randomWait = false;
        [Tooltip("The minimum wait time if random wait is enabled")]
        public SharedFloat randomWaitMin = 1;
        [Tooltip("The maximum wait time if random wait is enabled")]
        public SharedFloat randomWaitMax = 1;

        public bool myShouldNotifyTimer;
        public GameObject myNotifySpell;
        public string myNotifyText;
        public Sprite myNotifySprite;
        public Color myNofityColor;

        private BossTimerManager myBossTimerManager;

        private int myNotifyID;

        // The time to wait
        private float waitDuration;
        // The time that the task started to wait.
        private float startTime;
        // Remember the time that the task is paused so the time paused doesn't contribute to the wait time.
        private float pauseTime;

        public override void OnAwake()
        {
            if (myShouldNotifyTimer)
                myBossTimerManager = Object.FindObjectOfType<BossTimerManager>();
        }

        public override void OnStart()
        {
            // Remember the start time.
            startTime = Time.time;
            if (randomWait.Value)
            {
                waitDuration = Random.Range(randomWaitMin.Value, randomWaitMax.Value);
            }
            else
            {
                waitDuration = waitTime.Value;
            }

            if (myShouldNotifyTimer)
            {
                string notifyText = myNotifyText;
                Sprite notifySprite = myNotifySprite;
                Color notifyColor = myNofityColor;
                if (myNotifySpell)
                {
                    Spell spell = myNotifySpell.GetComponent<Spell>();
                    notifyText = spell.name;
                    notifySprite = spell.mySpellIcon;
                    notifyColor = spell.myCastbarColor;
                }

                myNotifyID = myBossTimerManager.AddBossTimer(notifyText, waitDuration, notifySprite, notifyColor);
            }
        }

        public override TaskStatus OnUpdate()
        {
            // The task is done waiting if the time waitDuration has elapsed since the task was started.
            if (startTime + waitDuration < Time.time)
            {
                return TaskStatus.Success;
            }
            // Otherwise we are still waiting.
            return TaskStatus.Running;
        }

        public override void OnPause(bool paused)
        {
            if (paused)
            {
                // Remember the time that the behavior was paused.
                pauseTime = Time.time;
            }
            else
            {
                // Add the difference between Time.time and pauseTime to figure out a new start time.
                startTime += (Time.time - pauseTime);
            }
        }

        public override void OnEnd()
        {
            if (myShouldNotifyTimer)
            {
                myBossTimerManager.RemoveBossTimer(myNotifyID);
            }
        }

        public override void OnReset()
        {
            // Reset the public properties back to their original values
            waitTime = 1;
            randomWait = false;
            randomWaitMin = 1;
            randomWaitMax = 1;
        }
    }
}