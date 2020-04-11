﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using BehaviorDesigner.Runtime;

public class NPCComponent : Character
{
    protected Subscriber mySubscriber;

    private NavMeshAgent myNavmeshAgent;
    private BehaviorTree myBehaviorTree;

    private TargetHandler myTargetHandler;

    private Vector3 mySpawnPosition;
    private Quaternion mySpawnRotation;

    public int PhaseIndex { get; set; }

    public enum CombatState
    {
        Idle,
        Combat,
        Disengage
    };

    public CombatState State { get; set; }

    // Use this for initialization
    private void Start()
    {
        myBehaviorTree = GetComponent<BehaviorTree>();

        mySpawnPosition = transform.position;
        mySpawnRotation = transform.rotation;

        State = CombatState.Idle;
        PhaseIndex = 1;

        myTargetHandler = FindObjectOfType<TargetHandler>();

        Subscribe();
    }

    protected virtual void OnDestroy()
    {
        Unsubscribe();
    }

    void Subscribe()
    {
        mySubscriber = new Subscriber();
        mySubscriber.EventOnReceivedMessage += ReceiveMessage;
        PostMaster.Instance.RegisterSubscriber(ref mySubscriber, MessageCategory.EnteredCombat);
        PostMaster.Instance.RegisterSubscriber(ref mySubscriber, MessageCategory.EnemyDied);
    }

    void Unsubscribe()
    {
        mySubscriber.EventOnReceivedMessage -= ReceiveMessage;
        PostMaster.Instance.UnregisterSubscriber(ref mySubscriber, MessageCategory.EnteredCombat);
        PostMaster.Instance.UnregisterSubscriber(ref mySubscriber, MessageCategory.EnemyDied);
    }

    protected void ReceiveMessage(Message anAiMessage)
    {
        switch (anAiMessage.Type)
        {
            case MessageCategory.EnteredCombat:
                if (State == CombatState.Idle)
                    SetState(CombatState.Combat);
                break;
            case MessageCategory.EnemyDied:
                if (myBehaviorTree)
                    myBehaviorTree.SendEvent(myTargetHandler.GetEnemyName(anAiMessage.Data.myInt) + "Died");
                break;
            default:
                break;
        }
    }


    public void SetState(CombatState aState)
    {
        State = aState;

        switch (State)
        {
            case CombatState.Idle:
                transform.rotation = mySpawnRotation;
                myAnimator.SetBool("IsRunning", false);
                break;
            case CombatState.Combat:
                PostMaster.Instance.PostMessage(new Message(MessageCategory.EnteredCombat));
                break;
            case CombatState.Disengage:
                if (myNavmeshAgent)
                    myNavmeshAgent.destination = mySpawnPosition;
                myAnimator.SetBool("IsRunning", true);

                if (myBehaviorTree)
                {
                    myBehaviorTree.DisableBehavior();
                    myBehaviorTree.EnableBehavior();
                }
                break;
        }
    }
}
