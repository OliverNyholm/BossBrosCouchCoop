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

    private void Awake()
    {
        myBehaviorTree = GetComponent<BehaviorTree>();
        myNavmeshAgent = GetComponent<NavMeshAgent>();
    }

    public void Start()
    {
        GetComponent<Health>().EventOnHealthZero += OnDeath;
    }

    public void OnDisable()
    {
        GetComponent<Health>().EventOnHealthZero -= OnDeath;
    }

    protected override void OnDeath()
    {
        if (myBehaviorTree)
            myBehaviorTree.enabled = false;
        if (myNavmeshAgent)
            myNavmeshAgent.isStopped = true;

        PostMaster.Instance.PostMessage(new Message(MessageCategory.EnemyDied, gameObject.GetInstanceID()));
    }

    public override bool IsMoving()
    {
        if (myNavmeshAgent && myNavmeshAgent.hasPath)
            return true;

        return false;
    }
}
