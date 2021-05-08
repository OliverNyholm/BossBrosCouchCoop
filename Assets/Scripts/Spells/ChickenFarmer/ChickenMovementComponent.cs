using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChickenMovementComponent : MovementComponent
{
    private NavMeshAgent myNavAgent;
    private AnimatorWrapper myAnimator;
    public PlayerMovementComponent myParentMovement = null;
    private ChickenFarmerChickenHandler myParentChickenHandler = null;
    private CharacterController myController;

    [SerializeField]
    private float myDurationBeforeReactingToMovement = 1.0f;
    private float myStartFollowingTimer = 0.0f;

    [SerializeField]
    private float myFlightSpeed = 15.0f;

    private Vector3 myFlightOffset;
    private float myFlightDuration = 0.0f;

    [SerializeField]
    private float myGravity = 8.0f;
    private bool myIsFalling = false;
    private Vector3 myFallingVelocity = new Vector3();
    private Vector3 myGroundLocation;

    [SerializeField]
    private float myReactTooFarAwayRadius = 3.0f;
    [SerializeField]
    private float myCloseEnoughMovementRadius = 2.0f;

    private Vector3 myParentOffsetLocation;

    private bool myIsFollowingParent = false;

    enum BombingState
    {
        ToParent,
        ToTarget,
        LeaveArea
    }

    private BombingState myBombingState;
    private GameObject myBombTarget = null;
    private Vector3 myFlyAwayTarget;

    public void SetParentAndOffset(GameObject aParent, Vector3 anOffset)
    {
        myParentMovement = aParent.GetComponent<PlayerMovementComponent>();
        myParentChickenHandler = aParent.GetComponent<ChickenFarmerChickenHandler>();
        myParentOffsetLocation = anOffset;
    }

    private void Awake()
    {
        myNavAgent = GetComponent<NavMeshAgent>();
        myAnimator = GetComponent<AnimatorWrapper>();
        myController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (!myParentMovement)
            return;

        if (myIsFalling)
            Falling();
        else if (myBombTarget)
            BombingMovement();
        else if (myFlightDuration > 0.0f)
            FlightMovement();
        else
            GroundMovement();
    }

    public void Reset()
    {
        myIsFalling = false;
        myIsFollowingParent = false;
        myAnimator.SetBool(AnimationVariable.IsGrounded, true);
        myAnimator.SetBool(AnimationVariable.IsRunning, false);
        myNavAgent.enabled = true;

        myBombTarget = null;
    }

    public override bool IsMoving()
    {
        return myNavAgent.hasPath;
    }

    protected override void OnDeath()
    {
        Reset();
    }

    public void SetFlightMode(Vector3 aFlightOffset, float aDuration)
    {
        myFlightDuration = aDuration;
        myFlightOffset = aFlightOffset;
        myNavAgent.enabled = false;

        myAnimator.SetBool(AnimationVariable.IsGrounded, false);
    }

    public void EnableBombingMode(GameObject aTarget)
    {
        myBombingState = BombingState.ToParent;
        myBombTarget = aTarget;

        myNavAgent.enabled = false;
        myAnimator.SetBool(AnimationVariable.IsGrounded, false);

        GetComponent<Chicken>().SetIsDroppingBomb();
    }

    private void GroundMovement()
    {
        Vector3 myTargetOffset = myParentMovement.transform.position + myParentMovement.transform.rotation * myParentOffsetLocation;
        if (!myIsFollowingParent && myStartFollowingTimer <= 0.0f && myParentMovement.IsMoving())
        {
            if ((transform.position - myTargetOffset).SqrMagnitude2D() > (myReactTooFarAwayRadius * myReactTooFarAwayRadius))
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

        if ((transform.position - myTargetOffset).SqrMagnitude2D() < (myCloseEnoughMovementRadius * myCloseEnoughMovementRadius))
        {
            myAnimator.SetBool(AnimationVariable.IsRunning, false);
            myNavAgent.destination = transform.position;
            myIsFollowingParent = false;
            return;
        }

        myNavAgent.destination = myParentMovement.transform.position;
    }

    private void FlightMovement()
    {
        myFlightDuration -= Time.deltaTime;
        if (myFlightDuration <= 0.0f)
        {
            myIsFalling = true;
            myFallingVelocity.y = 0.0f;
            if (UtilityFunctions.FindGroundFromLocation(transform.position, out RaycastHit hitInfo, out _, 20.0f))
                myGroundLocation = hitInfo.point + Vector3.up * 0.2f;
            else
                myGroundLocation = transform.position - Vector3.up * -100.0f;

            return;
        }

        Vector3 targetPosition = myParentMovement.transform.position + myParentMovement.transform.rotation * myFlightOffset;
        float distanceToTargetPosition = (targetPosition - transform.position).sqrMagnitude;
        const float closeEnoughDistanceSqr = 0.1f * 0.1f;
        if (distanceToTargetPosition < closeEnoughDistanceSqr)
        {
            if (!myParentMovement.IsFlying())
                myParentMovement.SetFlying(true);

            transform.position = targetPosition;
            transform.rotation = myParentMovement.transform.rotation;
            return;
        }

        Vector3 toTarget = (targetPosition - transform.position).normalized;
        Vector3 toTargetHorizontal = (targetPosition - transform.position).Normalized2D();

        myController.Move(toTarget * myFlightSpeed * Time.deltaTime);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(toTargetHorizontal, Vector3.up), Time.deltaTime * 360.0f);
    }

    private void BombingMovement()
    {
        Vector3 targetPosition = Vector3.zero;
        switch (myBombingState)
        {
            case BombingState.ToParent:
                {
                    const float heightOffset = 3.0f;
                    targetPosition = myParentMovement.transform.position + Vector3.up * heightOffset;
                    float distanceToTargetPosition = (targetPosition - transform.position).sqrMagnitude;
                    const float closeEnoughDistanceSqr = 2.0f * 2.0f;
                    if (distanceToTargetPosition < closeEnoughDistanceSqr)
                    {
                        myBombingState = BombingState.ToTarget;
                        return;
                    }
                }
                break;
            case BombingState.ToTarget:
                {
                    if (!myBombTarget)
                    {
                        myBombingState = BombingState.LeaveArea;
                        return;
                    }

                    const float heightOffset = 4.0f;
                    targetPosition = myBombTarget.transform.position + Vector3.up * heightOffset;
                    float distanceToTargetPosition = (targetPosition - transform.position).sqrMagnitude;
                    const float closeEnoughDistanceSqr = 5.0f * 5.0f;
                    if (distanceToTargetPosition < closeEnoughDistanceSqr)
                    {
                        myBombingState = BombingState.LeaveArea;
                        myFlyAwayTarget = transform.position + (transform.forward + transform.up) * 20.0f;
                        myParentChickenHandler.DropBomb(gameObject, myBombTarget);
                        return;
                    }
                }
                break;
            case BombingState.LeaveArea:
                {
                    targetPosition = myFlyAwayTarget;
                    float distanceToTargetPosition = (targetPosition - transform.position).sqrMagnitude;
                    const float closeEnoughDistanceSqr = 5.0f * 5.0f;
                    if (distanceToTargetPosition < closeEnoughDistanceSqr)
                    {
                        GetComponent<Chicken>().ReturnToPool();
                        return;
                    }
                }
                break;
        }

        Vector3 toTarget = (targetPosition - transform.position).normalized;
        Vector3 toTargetHorizontal = (targetPosition - transform.position).Normalized2D();

        myController.Move(toTarget * myFlightSpeed * Time.deltaTime);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(toTargetHorizontal, Vector3.up), Time.deltaTime * 360.0f);
    }

    private void Falling()
    {
        if (transform.position.y < myGroundLocation.y)
        {
            myIsFalling = false;
            myNavAgent.enabled = true;
            myNavAgent.destination = transform.position;
            myIsFollowingParent = false;
            transform.position = myGroundLocation;

            myAnimator.SetBool(AnimationVariable.IsGrounded, true);
            myAnimator.SetBool(AnimationVariable.IsRunning, false);
            return;
        }

        myFallingVelocity.y -= myGravity * Time.deltaTime;
        myController.Move(myFallingVelocity * Time.deltaTime);
    }
}
