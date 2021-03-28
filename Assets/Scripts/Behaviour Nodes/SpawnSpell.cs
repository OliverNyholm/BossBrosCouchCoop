using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class SpawnSpell : Action
{
    public SharedGameObject myTarget = null;
    public GameObject mySpell = null;
    public int mySpellMaxCount = 4;

    public SharedTransform mySpawnTransform = null;
    [Header("If only want to use position, use spawn position")]
    public SharedVector3 mySpawnPosition = null;

    private GameObject myEmptyTransformHolder;

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

        if(mySpell.GetComponent<UniqueID>() == null)
            Debug.LogError(mySpell.name + " is missing unique ID!");

        PoolManager poolManager = PoolManager.Instance;

        myEmptyTransformHolder = Object.Instantiate(new GameObject("Behaviour[" + ID + "]"), poolManager.GetEmptyTransformHolder());
        mySpell.GetComponent<Spell>().CreatePooledObjects(poolManager, mySpellMaxCount, gameObject);
    }

    public override void OnStart()
    {
        if (!myHasRegisteredForEvent)
        {
            Owner.RegisterEvent(myEventName, ReceivedEvent);
            Owner.RegisterEvent(myEventInterruptedName, ReceivedEventInterrupted);
            myHasRegisteredForEvent = true;
        }

        if (mySpawnTransform.Value != null)
        {
            myEmptyTransformHolder.transform.parent = mySpawnTransform.Value;
            myEmptyTransformHolder.transform.localPosition = Vector3.zero;
            myEmptyTransformHolder.transform.localRotation = Quaternion.identity;
            myEmptyTransformHolder.transform.localScale = Vector3.one;

            if (mySpawnPosition.Value != null)
            {
                myEmptyTransformHolder.transform.position += mySpawnPosition.Value;
            }
        }
        else if(mySpawnPosition.Value != null)
        {
            myEmptyTransformHolder.transform.position = mySpawnPosition.Value;
        }
        else
        {
            myEmptyTransformHolder.transform.position = transform.position;
        }

        myHasSpawnedSpell = false;
        NPCCastingComponent castingComponent = GetComponent<NPCCastingComponent>();
        castingComponent.IsInterruptable = myIsInterruptable;
        myCanCastSpell = castingComponent.CastSpell(mySpell, myTarget.Value, myEmptyTransformHolder.transform, myShouldIgnoreCastability);
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
        if (myHasRegisteredForEvent)
        {
            Owner.UnregisterEvent(myEventName, ReceivedEvent);
            Owner.UnregisterEvent(myEventInterruptedName, ReceivedEventInterrupted);
            myHasRegisteredForEvent = false;

            myEmptyTransformHolder.transform.parent = PoolManager.Instance.GetEmptyTransformHolder();
        }

        NPCCastingComponent castingComponent = GetComponent<NPCCastingComponent>();
        if (!myHasSpawnedSpell && castingComponent.IsCasting())
        {
            castingComponent.IsInterruptable = true;
            castingComponent.InterruptSpellCast();
        }

        myHasSpawnedSpell = false;
    }

    public override void OnBehaviorComplete()
    {
        // Stop receiving the event when the behavior tree is complete
        Owner.UnregisterEvent(myEventName, ReceivedEvent);
        Owner.UnregisterEvent(myEventInterruptedName, ReceivedEventInterrupted);
        myHasRegisteredForEvent = false;

        myHasSpawnedSpell = false;
    }

    private void ReceivedEvent()
    {
        myHasSpawnedSpell = true;
    }

    private void ReceivedEventInterrupted()
    {
        myCanCastSpell = false;
    }
}