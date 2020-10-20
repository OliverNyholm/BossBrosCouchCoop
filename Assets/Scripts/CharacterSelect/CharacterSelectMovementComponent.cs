using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterSelectMovementComponent : PlayerMovementComponent
{
    private NavMeshAgent myNavmeshAgent;
    private Vector3 myStartPosition;
    private Quaternion myStartRotation;

    protected override void Awake()
    {
        myStartPosition = transform.position;
        myStartRotation = transform.rotation;

        myNavmeshAgent = GetComponent<NavMeshAgent>();
        myAnimatorWrapper = GetComponent<AnimatorWrapper>();
        myStats = GetComponent<Stats>();
    }

    protected override void Start()
    {
    }

    protected override void Update()
    {
        myIsGrounded = IsGrounded();
        if (!myAnimatorWrapper.GetBool(AnimationVariable.IsGrounded) && myIsGrounded)
            myAnimatorWrapper.SetBool(AnimationVariable.IsGrounded, true);

        if (!myIsGrounded)
        {
            myVelocity.y -= myGravity * Time.deltaTime;
            transform.position += myVelocity * Time.deltaTime;
            return;
        }

        if(myStartPosition != transform.position && !myNavmeshAgent.enabled)
        {
            myNavmeshAgent.enabled = true;
            myNavmeshAgent.destination = myStartPosition;

            myAnimatorWrapper.SetBool(AnimationVariable.IsRunning, true);
        }

        if (myNavmeshAgent.enabled)
        {
            const float stopDistance = 0.01f;
            float distanceSqr = (new VectorXZ(myStartPosition) - new VectorXZ(transform.position)).sqrMagnitude;
            if (distanceSqr <= stopDistance * stopDistance && myNavmeshAgent.remainingDistance <= stopDistance)
            {
                transform.position = myStartPosition;
                myNavmeshAgent.destination = transform.position;
                transform.rotation = myStartRotation;
                myNavmeshAgent.enabled = false;

                myAnimatorWrapper.SetBool(AnimationVariable.IsRunning, false);
            }
        }
    }
}
