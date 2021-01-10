using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class OnHealthReached : Conditional
{
    public float myHealthPercentage = 0.5f;
    public bool myCanTriggerMultipleTimes = false;
    public bool myHasTriggeredPreviously = false;

    public enum Comparison
    {
        Less,
        LessEqual,
        Greater,
        GreaterEqual
    }

    public Comparison myComparison;

    // A cache of all of the possible targets
    private Health myHealthComponent = null;
    private bool myHasRegistered = false;
    private bool myHasHitHealthPercentage = false;

    public override void OnAwake()
    {
        myHealthComponent = GetComponent<Health>();
    }

    public override void OnStart()
    {
        if (!myHasRegistered && !myHasTriggeredPreviously)
        {
            myHealthComponent.EventOnHealthChange += OnHealthChanged;
            myHasRegistered = true;
        }
    }

    public override TaskStatus OnUpdate()
    {
        if (myHasHitHealthPercentage)
            return TaskStatus.Success;

        return TaskStatus.Failure;
    }

    public override void OnEnd()
    {
        if (myHasHitHealthPercentage && myHasRegistered)
        {
            myHasTriggeredPreviously = true;
            myHealthComponent.EventOnHealthChange -= OnHealthChanged;
            myHasRegistered = false;
        }

        if (myCanTriggerMultipleTimes)
            myHasTriggeredPreviously = false;

        myHasHitHealthPercentage = false;
    }

    private void OnHealthChanged(float aHealthPercentage, string aHealthText, int aShieldValue, bool aIsDamage)
    {
        switch (myComparison)
        {
            case Comparison.Less:
                if (aHealthPercentage < myHealthPercentage)
                    myHasHitHealthPercentage = true;
                break;
            case Comparison.LessEqual:
                if (aHealthPercentage <= myHealthPercentage)
                    myHasHitHealthPercentage = true;
                break;
            case Comparison.Greater:
                if (aHealthPercentage > myHealthPercentage)
                    myHasHitHealthPercentage = true;
                break;
            case Comparison.GreaterEqual:
                if (aHealthPercentage >= myHealthPercentage)
                    myHasHitHealthPercentage = true;
                break;
        }
    }

    public override void OnReset()
    {
        base.OnReset();

        if (myHasRegistered && myHealthComponent)
            myHealthComponent.EventOnHealthChange -= OnHealthChanged;

        myHasRegistered = false;
        myHasHitHealthPercentage = false;
        myHasTriggeredPreviously = false;
    }
}
