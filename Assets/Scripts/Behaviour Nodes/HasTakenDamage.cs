using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class HasTakenDamage : Conditional
{
    private bool myHasTakenDamage = false;
    private bool myIsRegistered;

    public override void OnAwake()
    {
        base.OnAwake();
        myIsRegistered = false;
    }

    public override void OnStart()
    {
        if (!myIsRegistered && !myHasTakenDamage)
        {
            GetComponent<Health>().EventOnHealthChange += OnDamageTaken;
            myIsRegistered = true;
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
        myHasTakenDamage = false;
    }

    public override void OnBehaviorComplete()
    {
        if (myIsRegistered)
            GetComponent<Health>().EventOnHealthChange -= OnDamageTaken;

        myHasTakenDamage = false;
    }

    private void OnDamageTaken(float aHealthPercentage, string aHealthText, int aShieldValue, bool aIsDamage)
    {
        myHasTakenDamage = true;
        myIsRegistered = false;
        GetComponent<Health>().EventOnHealthChange -= OnDamageTaken;
    }
}
