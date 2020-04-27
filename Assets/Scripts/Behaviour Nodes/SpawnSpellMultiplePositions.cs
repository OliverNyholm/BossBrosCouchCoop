using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskDescription("Will spawn an INSTANT spell at multiple GameObjects positions.")]
public class SpawnSpellMultiplePositions : Action
{
    public GameObject mySpell;
    public int mySpellMaxCount = 4;

    public SharedGameObjectList mySpawnObjects;

    [BehaviorDesigner.Runtime.Tasks.Tooltip("Enable if you want the spell to spawn without vision, resource, etc checks.")]
    public bool myShouldIgnoreCastability;

    private int myNumberOfSpellsSpawned;

    private bool myCanCastSpell;
    private bool myHasRegisteredForEvent;
    private bool myHasSpawnedSpell;
    private string myEventName = "SpellSpawned";

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
            myHasRegisteredForEvent = true;
        }

        myNumberOfSpellsSpawned = 0;
        myHasSpawnedSpell = false;

        for (int index = 0; index < mySpawnObjects.Value.Count; index++)
        {
            myCanCastSpell = GetComponent<NPCCastingComponent>().CastSpell(mySpell, mySpawnObjects.Value[index], mySpawnObjects.Value[index].transform, myShouldIgnoreCastability);
            if (!myCanCastSpell)
                break;
        }
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
        myNumberOfSpellsSpawned = 0;
    }

    public override void OnBehaviorComplete()
    {
        // Stop receiving the event when the behavior tree is complete
        Owner.RegisterEvent(myEventName, ReceivedEvent);
        myHasRegisteredForEvent = true;

        myHasSpawnedSpell = false;
        myNumberOfSpellsSpawned = 0;
    }

    private void ReceivedEvent()
    {
        myNumberOfSpellsSpawned++;
        if (myNumberOfSpellsSpawned >= mySpawnObjects.Value.Count)
            myHasSpawnedSpell = true;
    }
}