using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChickenMovementComponent : MovementComponent
{
    private NavMeshAgent myNavAgent;
    private AnimatorWrapper myAnimator;
    public PlayerMovementComponent myParentMovement = null;

    private Queue<Vector3> myParentSeeds = new Queue<Vector3>(64);
    private Vector3 myTargetPosition;
    private float myDropSeedsInterval = 0.4f;
    private float myDropSeedsInAirInterval = 0.1f;
    private float myDropSeedTimer = 0.0f;

    private void Awake()
    {
        myNavAgent = GetComponent<NavMeshAgent>();
        myAnimator = GetComponent<AnimatorWrapper>();
    }

    private void Update()
    {
        if (!myParentMovement)
            return;

        bool hadSeeds = myParentSeeds.Count > 0;

        myDropSeedTimer += Time.deltaTime;
        if (myParentMovement.IsMoving())
        {
            float interval = myParentMovement.IsInAir() ? myDropSeedsInAirInterval : myDropSeedsInterval;
            if (myDropSeedTimer > interval)
            {
                myParentSeeds.Enqueue(myParentMovement.transform.position);
                myDropSeedTimer = 0.0f;
            }
        }

        while (myParentSeeds.Count > 0 && Vector3.Distance(transform.position, myParentSeeds.Peek()) < 0.4f)
            myParentSeeds.Dequeue();

        if (myParentSeeds.Count == 0)
        {
            if (hadSeeds)
                myAnimator.SetBool(AnimationVariable.IsRunning, false);

            return;
        }

        if (!hadSeeds)
            myAnimator.SetBool(AnimationVariable.IsRunning, true);

        myTargetPosition = myParentSeeds.Peek();
        Vector3 toTarget = (myTargetPosition - transform.position).normalized;

        myAnimator.SetBool(AnimationVariable.IsGrounded, toTarget.y > 0);


        transform.position += toTarget * myBaseSpeed * Time.deltaTime;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(toTarget, Vector3.up), Time.deltaTime * 360.0f);

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
}
