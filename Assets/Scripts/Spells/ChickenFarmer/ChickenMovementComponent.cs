using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChickenMovementComponent : MovementComponent
{
    private NavMeshAgent myNavAgent;
    private AnimatorWrapper myAnimator;
    public PlayerMovementComponent myParentMovement = null;

    [SerializeField]
    private float myDurationBeforeReactingToMovement = 1.0f;
    private float myStartFollowingTimer = 0.0f;

    [SerializeField]
    private float myReactTooFarAwayRadius = 3.0f;
    [SerializeField]
    private float myCloseEnoughMovementRadius = 2.0f;

    private Vector3 myParentOffsetLocation;

    private bool myIsFollowingParent = false;

    public void SetParentAndOffset(GameObject aParent, Vector3 anOffset)
    {
        myParentMovement = aParent.GetComponent<PlayerMovementComponent>();
        myParentOffsetLocation = anOffset;
    }

    private void Awake()
    {
        myNavAgent = GetComponent<NavMeshAgent>();
        myAnimator = GetComponent<AnimatorWrapper>();
    }

    private void Update()
    {
        if (!myParentMovement)
            return;

        if (!myIsFollowingParent && myStartFollowingTimer <= 0.0f && myParentMovement.IsMoving())
        {
            if ((transform.position - myParentMovement.transform.position).SqrMagnitude2D() > (myReactTooFarAwayRadius * myReactTooFarAwayRadius))
            {
                myStartFollowingTimer = myDurationBeforeReactingToMovement;
                return;
            }
        }

        if (myStartFollowingTimer > 0.0f)
        {
            myStartFollowingTimer -= Time.deltaTime;
            if (myStartFollowingTimer <= 0.0f)
            {
                myAnimator.SetBool(AnimationVariable.IsRunning, true);
                myIsFollowingParent = true;
            }
        }

        if (!myIsFollowingParent)
            return;

        if ((transform.position - myParentMovement.transform.position).SqrMagnitude2D() < (myCloseEnoughMovementRadius * myCloseEnoughMovementRadius))
        {
            myAnimator.SetBool(AnimationVariable.IsRunning, false);
            myNavAgent.destination = transform.position;
            myIsFollowingParent = false;
            return;
        }

        myNavAgent.destination = myParentMovement.transform.position;

        //bool hadSeeds = myParentSeeds.Count > 0;
        //
        //myDropSeedTimer += Time.deltaTime;
        //if (myParentMovement.IsMoving())
        //{
        //    float interval = myParentMovement.IsInAir() ? myDropSeedsInAirInterval : myDropSeedsInterval;
        //    if (myDropSeedTimer > interval)
        //    {
        //        myParentSeeds.Enqueue(myParentMovement.transform.position);
        //        myDropSeedTimer = 0.0f;
        //    }
        //}
        //
        //while (myParentSeeds.Count > 0 && Vector3.Distance(transform.position, myParentSeeds.Peek()) < 0.4f)
        //    myParentSeeds.Dequeue();
        //
        //if (myParentSeeds.Count == 0)
        //{
        //    if (hadSeeds)
        //        myAnimator.SetBool(AnimationVariable.IsRunning, false);
        //
        //    return;
        //}
        //
        //if (!hadSeeds)
        //    myAnimator.SetBool(AnimationVariable.IsRunning, true);
        //
        //myTargetPosition = myParentSeeds.Peek();
        //Vector3 toTarget = (myTargetPosition - transform.position).normalized;
        //
        //myAnimator.SetBool(AnimationVariable.IsGrounded, toTarget.y > 0);


        //transform.position += toTarget * myBaseSpeed * Time.deltaTime;
        //transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(toTarget, Vector3.up), Time.deltaTime * 360.0f);

        //if (!myNavAgent.hasPath)
        //{
        //    myNavAgent.destination = myParentSeeds.Peek();
        //}
    }

    public override bool IsMoving()
    {
        return myNavAgent.hasPath;
    }

    protected override void OnDeath()
    {

    }

    public void SetFlightMode(Vector3 aFlightOffset, float aDuration)
    {

    }
}
