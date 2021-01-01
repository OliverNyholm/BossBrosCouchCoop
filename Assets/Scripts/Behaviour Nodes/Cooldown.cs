using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class Cooldown : Conditional
{
    public float myCooldownDuration = 1.0f;
    private float myCooldownEndTime = 0.0f;


    public override TaskStatus OnUpdate()
    {
        if (Time.time > myCooldownEndTime)
        {
            myCooldownEndTime = Time.time + myCooldownDuration;
            return TaskStatus.Success;
        }

        return TaskStatus.Failure;
    }
}