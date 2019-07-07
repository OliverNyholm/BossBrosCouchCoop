using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class SpawnSpell : Action
{
    public SharedGameObject myTarget;
    public GameObject mySpell;

    public SharedVector3 mySpawnPosition;

    [BehaviorDesigner.Runtime.Tasks.Tooltip("Enable if you want the spell to spawn without vision, resource, etc checks.")]
    public bool myShouldIgnoreCastability;

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

        myCanCastSpell = GetComponent<Enemy>().CastSpell(mySpell, myTarget.Value, mySpawnPosition.Value, myShouldIgnoreCastability);
        myHasSpawnedSpell = false;
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