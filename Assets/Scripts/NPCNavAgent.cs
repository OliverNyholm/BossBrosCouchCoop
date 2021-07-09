using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NPCNavAgent : MonoBehaviour
{
    private NavMeshAgent myNavmeshAgent;
    private AnimatorWrapper myAnimatorWrapper;

    public delegate void DelegateTrigger();
    public event DelegateTrigger OnEnterNavLink;
    public event DelegateTrigger OnExitNavLink;
    public event DelegateTrigger OnStoppedMoving;

    private bool myWasOnNavLink = false;
    private bool myWasMoving = false;

    private void Awake()
    {
        myNavmeshAgent = GetComponent<NavMeshAgent>();
        myAnimatorWrapper = GetComponent<AnimatorWrapper>();
    }

    private void Update()
    {
        bool isMoving = myNavmeshAgent.hasPath && !myNavmeshAgent.isStopped;
        if (!isMoving && !myWasMoving)
            return;

        bool isOnNavLink = myNavmeshAgent.isOnOffMeshLink;
        if (isOnNavLink != myWasOnNavLink)
        {
            if (isOnNavLink)
                EnteredNavLink();
            else
                ExitedNavLink();
        }

        if (!isMoving)
            StoppedMoving();

        myWasMoving = isMoving;
        myWasOnNavLink = isOnNavLink;
    }

    public void Move(Vector3 aDestination)
    {
        myNavmeshAgent.isStopped = false;
        myNavmeshAgent.destination = aDestination;
        myAnimatorWrapper.SetBool(AnimationVariable.IsRunning, true);
    }

    private void StoppedMoving()
    {
        OnStoppedMoving?.Invoke();
        if (myAnimatorWrapper)
            myAnimatorWrapper.SetBool(AnimationVariable.IsRunning, false);
    }

    private void EnteredNavLink()
    {
        OnEnterNavLink?.Invoke();
        if (myAnimatorWrapper)
        {
            myAnimatorWrapper.SetTrigger(AnimationVariable.Jump);
            myAnimatorWrapper.SetBool(AnimationVariable.IsGrounded, false);
        }
    }

    private void ExitedNavLink()
    {
        OnExitNavLink?.Invoke();
        if (myAnimatorWrapper)
            myAnimatorWrapper.SetBool(AnimationVariable.IsGrounded, true);
    }
}
