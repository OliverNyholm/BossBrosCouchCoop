using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class HasTakenDamage : Conditional
{
    private bool myHasRegisteredForEvent;
    private bool myHasTakenDamage;

    private string myEventName = "TakeDamage";

    public override void OnStart()
    {
        if(!myHasRegisteredForEvent)
        {
            Owner.RegisterEvent(myEventName, ReceivedEvent);
            myHasRegisteredForEvent = true;
        }
    }

    public override TaskStatus OnUpdate()
    {
        if (myHasTakenDamage)
            return TaskStatus.Success;

        return TaskStatus.Failure;
    }

    public override void OnEnd()
    {
        if (myHasTakenDamage)
        {
            Owner.UnregisterEvent(myEventName, ReceivedEvent);
            myHasRegisteredForEvent = false;
        }
        myHasTakenDamage = false;
    }

    public override void OnBehaviorComplete()
    {
        // Stop receiving the event when the behavior tree is complete
        Owner.RegisterEvent(myEventName, ReceivedEvent);
        myHasRegisteredForEvent = true;

        myHasTakenDamage = false;
    }

    private void ReceivedEvent()
    {
        myHasTakenDamage = true;
    }
}
