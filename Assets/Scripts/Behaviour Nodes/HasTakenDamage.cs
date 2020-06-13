using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class HasTakenDamage : Conditional
{
    public bool myShouldListenToTaunt = true;
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
            if (myShouldListenToTaunt)
            {
                NPCThreatComponent threatComponent = GetComponent<NPCThreatComponent>();
                if (threatComponent)
                {
                    threatComponent.EventOnTaunted += Unregister;
                }
                else
                {
                    Debug.LogWarning("Boss[" + transform.name + "] cant listend to taunt without threat component.");
                }
            }

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
        Unregister();
    }

    private void Unregister()
    {
        myHasTakenDamage = true;
        myIsRegistered = false;
        GetComponent<Health>().EventOnHealthChange -= OnDamageTaken;

        if (myShouldListenToTaunt)
        {
            NPCThreatComponent threatComponent = GetComponent<NPCThreatComponent>();
            if (threatComponent)
                threatComponent.EventOnTaunted -= Unregister;
        }
    }
}
