using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using BehaviorDesigner.Runtime;

[RequireComponent(typeof(NavMeshAgent))]
public class NPCMovementComponent : MovementComponent
{
    private BehaviorTree myBehaviorTree;
    private NavMeshAgent myNavmeshAgent;
    private NPCComponent myNPCComponent;
    private AnimatorWrapper myAnimatorWrapper;

    private void Awake()
    {
        myBehaviorTree = GetComponent<BehaviorTree>();
        myNavmeshAgent = GetComponent<NavMeshAgent>();
        myNPCComponent = GetComponent<NPCComponent>();
        myAnimatorWrapper = GetComponent<AnimatorWrapper>();
    }

    public void Start()
    {
        myNavmeshAgent.speed = myBaseSpeed;

        GetComponent<Health>().EventOnHealthZero += OnDeath;
    }

    public void Update()
    {
        if (!myCanBeAffectedByMovingPlatform)
            return;

        if (UtilityFunctions.FindGroundFromLocation(transform.position, out Vector3 hitLocation, out MovablePlatform movablePlatform))
        {
            if (myMovablePlatform != movablePlatform)
            {
                if (myMovablePlatform)
                    myMovablePlatform.RemoveFromPlatform(gameObject);

                myMovablePlatform = movablePlatform;
                if(myMovablePlatform)
                    myMovablePlatform.AddToPlatform(gameObject);
            }
        }
    }

    public void OnDisable()
    {
        GetComponent<Health>().EventOnHealthZero -= OnDeath;

        if(myMovablePlatform)
            myMovablePlatform.RemoveFromPlatform(gameObject);
    }

    public void MoveTo(Vector3 aTargetPosition)
    {
        myNavmeshAgent.destination = aTargetPosition;
        myNavmeshAgent.isStopped = false;

        myAnimatorWrapper.SetBool(AnimationVariable.IsRunning, true);
    }

    public void Stop()
    {
        if (myNavmeshAgent.isStopped)
            return;

        myNavmeshAgent.destination = transform.position;
        myNavmeshAgent.isStopped = true;
        myAnimatorWrapper.SetBool(AnimationVariable.IsRunning, false);
    }

    protected override void OnDeath()
    {
        if (myBehaviorTree)
            myBehaviorTree.enabled = false;

        myNavmeshAgent.isStopped = true;

        PostMaster.Instance.PostMessage(new Message(MessageCategory.EnemyDied, gameObject.GetInstanceID()));
    }

    public override bool IsMoving()
    {
        if (myNavmeshAgent.isStopped)
            return false;

        if (myNavmeshAgent.hasPath)
            return true;

        return false;
    }
}
