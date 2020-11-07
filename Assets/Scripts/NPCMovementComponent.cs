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

    private void Awake()
    {
        myBehaviorTree = GetComponent<BehaviorTree>();
        myNavmeshAgent = GetComponent<NavMeshAgent>();
        myNPCComponent = GetComponent<NPCComponent>();
    }

    public void Start()
    {
        myNavmeshAgent.speed = myBaseSpeed;

        GetComponent<Health>().EventOnHealthZero += OnDeath;
    }

    public void OnDisable()
    {
        GetComponent<Health>().EventOnHealthZero -= OnDeath;
    }

    public void Stop()
    {
        myNavmeshAgent.destination = transform.position;
        myNavmeshAgent.isStopped = true;
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
        if (myNavmeshAgent.hasPath)
            return true;

        return false;
    }
}
