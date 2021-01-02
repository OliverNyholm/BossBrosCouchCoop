using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbOfLight : Spell
{
    [SerializeField]
    private LayerMask myBlockMovmentLayerMask = 0;

    [SerializeField]
    private float myLifetime = 20.0f;
    private float myDurationLeft = 0.0f;

    [SerializeField]
    private AnimationCurve myBounceCurve = null;
    private float myBounceCurveDuration = 0.0f;
    private float myBounceTimer = 0.0f;

    [SerializeField]
    private float myOffsetFromGround = 1.0f;
    private float myBounceOffset = 0.0f;

    [SerializeField]
    private float myMoveToLaunchPositionDuration = 3.5f;
    private float myMoveInterpolationValue = 0.0f;

    private float myIgnoreTargetInitialDuration = 0.75f;

    [SerializeField]
    private float myPlayerAttractionRadius = 4.0f;
    private float myAttractionAcceleration = 12f;
    private float myAttractionMaxSpeed = 17.0f;
    private float myAttractionSpeed = 0.0f;

    private Vector3 myMoveToTargetDirection = Vector3.zero;

    [SerializeField]
    private float myTravelDistance = 15.0f;

    private Vector3 myTargetPosition = Vector3.zero;
    private Vector3 myStartPosition = Vector3.zero;
    private Vector3 myPreviousPosition = Vector3.zero;

    private List<GameObject> myPlayers;
    private GameObject myMoveToTarget = null;

    protected override void Awake()
    {
        base.Awake();
        myBounceCurveDuration = myBounceCurve.keys[myBounceCurve.length - 1].time;
        SetPlayers(FindObjectOfType<TargetHandler>().GetAllPlayers());
    }

    public override void Restart()
    {
        myIgnoreTargetInitialDuration = 1.0f;
        myMoveInterpolationValue = 0.0f;
        myStartPosition = transform.position;
        myMoveToTarget = null;
        myDurationLeft = myLifetime;

        myPreviousPosition = myStartPosition;

        Ray ray = new Ray(myStartPosition + Vector3.up * myOffsetFromGround, myParent.transform.forward);

        const float offsetFromWall = 0.4f;
        float distanceToMove = myTravelDistance;
        if (Physics.Raycast(ray, out RaycastHit hitInfo, myTravelDistance, myBlockMovmentLayerMask))
            distanceToMove = hitInfo.distance - offsetFromWall;

        myTargetPosition = myStartPosition + myParent.transform.forward * distanceToMove;

        base.Restart();
    }

    private void SetPlayers(List<GameObject> somePlayers)
    {
        myPlayers = somePlayers;
    }

    protected override void Update()
    {
        DetectClosePlayer();
        myPreviousPosition = transform.position;

        if (myMoveToTarget)
        {
            MoveToTargetLocation();
        }
        else if (myMoveInterpolationValue < myMoveToLaunchPositionDuration)
        {
            MoveToLaunchPosition();
        }
        else
        {
            myDurationLeft -= Time.deltaTime;
            if (myDurationLeft <= 0.0f)
            {
                ReturnToPool();
                return;
            }
        }

        myBounceTimer += Time.deltaTime;
        if (myBounceTimer >= myBounceCurveDuration)
            myBounceTimer -= myBounceCurveDuration;

        myBounceOffset = myBounceCurve.Evaluate(myBounceTimer);

        Vector3 position = transform.position;
        position.y = myStartPosition.y + myOffsetFromGround + myBounceOffset;
        transform.position = position;
    }

    private void MoveToLaunchPosition()
    {
        myMoveInterpolationValue += Time.deltaTime;

        float interpolation = LeanTween.easeOutCubic(0.0f, 1.0f, myMoveInterpolationValue / myMoveToLaunchPositionDuration);
        transform.position = Vector3.Lerp(myStartPosition, myTargetPosition, interpolation);
    }

    private void MoveToTargetLocation()
    {
        myAttractionSpeed += myAttractionAcceleration * Time.deltaTime;
        myAttractionSpeed = Mathf.Min(myAttractionSpeed, myAttractionMaxSpeed);

        Vector3 toTargetHorizontal = (myMoveToTarget.transform.position - transform.position).Normalized2D();
        Vector3 movementDifference = (toTargetHorizontal - myMoveToTargetDirection).Normalized2D();
        myMoveToTargetDirection += movementDifference * 4.0f * Time.deltaTime;
        myMoveToTargetDirection.Normalize2D();

        transform.position += myMoveToTargetDirection * myAttractionSpeed * Time.deltaTime;

        const float closeEnoughRange = 0.7f * 0.7f;
        if((myMoveToTarget.transform.position - transform.position).SqrMagnitude2D() < closeEnoughRange)
        {
            OnReachTarget();
        }
    }

    private void DetectClosePlayer()
    {
        if (myIgnoreTargetInitialDuration > 0.0f)
        {
            myIgnoreTargetInitialDuration -= Time.deltaTime;
            return;
        }

        float attractionRadiusSqr = myPlayerAttractionRadius * myPlayerAttractionRadius;
        float smallestDistance = float.MaxValue;
        foreach (GameObject player in myPlayers)
        {
            if (!player)
                continue;

            Health playerHealth = player.GetComponent<Health>();
            if (playerHealth.IsDead())
                continue;

            float distanceToPlayerSqr = (player.transform.position - transform.position).SqrMagnitude2D();
            if(distanceToPlayerSqr < attractionRadiusSqr && distanceToPlayerSqr < smallestDistance)
            {
                if (myMoveToTarget == null)
                {
                    myMoveToTargetDirection = (transform.position - myPreviousPosition);
                    float horizontalSpeed = myMoveToTargetDirection.Magnitude2D();
                    myAttractionSpeed = horizontalSpeed / Time.deltaTime;
                    myAttractionSpeed = Mathf.Min(myAttractionSpeed, myAttractionMaxSpeed);
                    
                    if(horizontalSpeed > float.Epsilon)
                        myMoveToTargetDirection /= horizontalSpeed;
                }

                myMoveToTarget = player;
                smallestDistance = distanceToPlayerSqr;
            }
        }
    }
    
    public float GetOffsetFromGround()
    {
        return myOffsetFromGround;
    }

    public float GetTravelDistance()
    {
        return myTravelDistance;
    }
}
