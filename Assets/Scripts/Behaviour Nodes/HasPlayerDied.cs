using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class HasPlayerDied : Conditional
{
    private bool myHasRegisteredForEvent;
    private bool myHasPlayerDied;

    private string myEventName = "PlayerDied";

    public override void OnStart()
    {
        if (!myHasRegisteredForEvent)
        {
            Owner.RegisterEvent(myEventName, ReceivedEvent);
            myHasRegisteredForEvent = true;
        }
    }

    public override TaskStatus OnUpdate()
    {
        if (myHasPlayerDied)
            return TaskStatus.Success;

        return TaskStatus.Failure;
    }

    public override void OnEnd()
    {
        if (myHasPlayerDied)
        {
            Owner.UnregisterEvent(myEventName, ReceivedEvent);
            myHasRegisteredForEvent = false;
        }
        myHasPlayerDied = false;
    }

    public override void OnBehaviorComplete()
    {
        // Stop receiving the event when the behavior tree is complete
        Owner.RegisterEvent(myEventName, ReceivedEvent);
        myHasRegisteredForEvent = true;

        myHasPlayerDied = false;
    }

    private void ReceivedEvent()
    {
        myHasPlayerDied = true;
    }
}