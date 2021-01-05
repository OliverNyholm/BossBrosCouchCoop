using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class CallFunction : Action
{
    public UnityEvent myEventToCall;

    public override TaskStatus OnUpdate()
    {
        myEventToCall.Invoke();

        return TaskStatus.Success;
    }
}
