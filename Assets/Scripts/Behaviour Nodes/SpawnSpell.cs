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
        poolManager.AddPoolableObjects(mySpell, mySpell.GetComponent<UniqueID>().GetID(), mySpellMaxCount);
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
            myEmptyTransformHolder.transform.position = mySpawnTransform.Value.position;
            myEmptyTransformHolder.transform.rotation = mySpawnTransform.Value.rotation;
            myEmptyTransformHolder.transform.localScale = mySpawnTransform.Value.localScale;
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
        Enemy enemyComponent = GetComponent<Enemy>();
        enemyComponent.IsInterruptable = myIsInterruptable;
        myCanCastSpell = enemyComponent.CastSpell(mySpell, myTarget.Value, myEmptyTransformHolder.transform, myShouldIgnoreCastability);
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
        myHasSpawnedSpell = true;
    }

    private void ReceivedEventInterrupted()
    {
        myCanCastSpell = false;
    }
}