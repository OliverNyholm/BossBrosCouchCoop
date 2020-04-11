using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class SpawnSpellMultipleTargets : Action
{
    public SharedGameObjectList myTargets = null;
    public GameObject mySpell = null;
    public int mySpellMaxCount = 4;

    public SharedTransform mySpawnTransform = null;

    [BehaviorDesigner.Runtime.Tasks.Tooltip("Enable if you want the spell to spawn without vision, resource, etc checks.")]
    public bool myShouldIgnoreCastability = true;

    [BehaviorDesigner.Runtime.Tasks.Tooltip("Enable if you want the spell to be interruptable.")]
    public bool myIsInterruptable = true;

    private bool myCanCastSpell;
    private bool myHasRegisteredForEvent;
    private bool myHasSpawnedSpell;
    private string myEventName = "SpellSpawned";
    private string myEventInterruptedName = "SpellInterrupted";

    public override void OnAwake()
    {
        base.OnAwake();

        mySpell.GetComponent<Spell>().CreatePooledObjects(PoolManager.Instance, mySpellMaxCount);
    }

    public override void OnStart()
    {
        if (!myHasRegisteredForEvent)
        {
            Owner.RegisterEvent(myEventName, ReceivedEvent);
            Owner.RegisterEvent(myEventInterruptedName, ReceivedEventInterrupted);
            myHasRegisteredForEvent = true;
        }

        if (mySpawnTransform.Value == null)
            mySpawnTransform.Value = transform;

        myHasSpawnedSpell = false;
        NPCCastingComponent castingComponent = GetComponent<NPCCastingComponent>();
        castingComponent.IsInterruptable = myIsInterruptable;
        myCanCastSpell = castingComponent.CastSpell(mySpell, myTargets.Value[0], mySpawnTransform.Value, myShouldIgnoreCastability);
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
            Owner.UnregisterEvent(myEventInterruptedName, ReceivedEventInterrupted);
            myHasRegisteredForEvent = false;
        }
        myHasSpawnedSpell = false;
    }

    public override void OnBehaviorComplete()
    {
        // Stop receiving the event when the behavior tree is complete
        Owner.RegisterEvent(myEventName, ReceivedEvent);
        Owner.RegisterEvent(myEventInterruptedName, ReceivedEventInterrupted);
        myHasRegisteredForEvent = true;

        myHasSpawnedSpell = false;
    }

    private void ReceivedEvent()
    {
        if (myHasSpawnedSpell)
            return;

        myHasSpawnedSpell = true;
        for (int index = 1; index < myTargets.Value.Count; index++)
        {
            GetComponent<NPCCastingComponent>().SpawnSpell(mySpell, myTargets.Value[index], mySpawnTransform.Value);
        }
    }

    private void ReceivedEventInterrupted()
    {
        myCanCastSpell = false;
    }
}
