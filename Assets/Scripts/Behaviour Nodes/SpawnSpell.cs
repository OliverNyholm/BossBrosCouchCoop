using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class SpawnSpell : Action
{
    public SharedGameObject myTarget = null;
    public GameObject mySpell = null;

    public SharedTransform mySpawnTransform = null;

    [BehaviorDesigner.Runtime.Tasks.Tooltip("Enable if you want the spell to spawn without vision, resource, etc checks.")]
    public bool myShouldIgnoreCastability = true;

    [BehaviorDesigner.Runtime.Tasks.Tooltip("Enable if you want the spell to be interruptable.")]
    public bool myIsInterruptable = true;

    private bool myCanCastSpell;
    private bool myHasRegisteredForEvent;
    private bool myHasSpawnedSpell;
    private string myEventName = "SpellSpawned";

    public override void OnStart()
    {
        if (!myHasRegisteredForEvent)
        {
            Owner.RegisterEvent(myEventName, ReceivedEvent);
            myHasRegisteredForEvent = true;
        }

        myHasSpawnedSpell = false;
        Enemy enemyComponent = GetComponent<Enemy>();
        enemyComponent.IsInterruptable = myIsInterruptable;
        myCanCastSpell = enemyComponent.CastSpell(mySpell, myTarget.Value, mySpawnTransform.Value, myShouldIgnoreCastability);
    }

    public override TaskStatus OnUpdate()
    {
        if (myHasSpawnedSpell)
            return TaskStatus.Success;

        if (!myCanCastSpell)
            return TaskStatus.Failure;

        return TaskStatus.Running;
    }

    public override void OnEnd()
    {
        if (myHasSpawnedSpell)
        {
            Owner.UnregisterEvent(myEventName, ReceivedEvent);
            myHasRegisteredForEvent = false;
        }
        myHasSpawnedSpell = false;
    }

    public override void OnBehaviorComplete()
    {
        // Stop receiving the event when the behavior tree is complete
        Owner.RegisterEvent(myEventName, ReceivedEvent);
        myHasRegisteredForEvent = true;

        myHasSpawnedSpell = false;
    }

    private void ReceivedEvent()
    {
        myHasSpawnedSpell = true;
    }
}