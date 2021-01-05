using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovementComponent : MovementComponent
{
    public float myJumpSpeed;
    public float myGravity;

    protected AnimatorWrapper myAnimatorWrapper;
    private CharacterController myController;
    private PlayerControls myPlayerControls;
    private PlayerCastingComponent myCastingComponent;
    private PlayerTargetingComponent myTargetingComponent;

    private Health myHealth;
    protected Stats myStats;

    protected Vector3 myVelocity;
    private CameraXZTransform myCameraXZTransform;

    protected bool myIsGrounded;
    private bool myHasJumped = false;
    private Vector3 myPreviousGroundPosition;
    private float myStartFallingTimestamp;

    private bool myIsMovementDisabled = false;

    protected virtual void Awake()
    {
        myController = GetComponent<CharacterController>();
        myAnimatorWrapper = GetComponent<AnimatorWrapper>();
        myCastingComponent = GetComponent<PlayerCastingComponent>();
        myTargetingComponent = GetComponent<PlayerTargetingComponent>();
        myHealth = GetComponent<Health>();
        myStats = GetComponent<Stats>();
    }

    protected virtual void Start()
    {
        Camera camera = Camera.main;
        myCameraXZTransform.myForwards = camera.transform.forward;
        myCameraXZTransform.myForwards.y = 0.0f;
        myCameraXZTransform.myForwards.Normalize();

        myCameraXZTransform.myRight = camera.transform.right;
        myCameraXZTransform.myRight.y = 0.0f;
        myCameraXZTransform.myRight.Normalize();

        myVelocity = Vector3.zero;
        myPreviousGroundPosition = transform.position;
        myStartFallingTimestamp = 0.0f;

        myHealth.EventOnHealthZero += OnDeath;
    }

    public void SetPlayerController(PlayerControls aPlayerControls)
    {
        myPlayerControls = aPlayerControls;
    }

    protected virtual void Update()
    {
        if (myIsMovementDisabled)
            return;

        myVelocity.y -= myGravity * Time.deltaTime;
        myController.Move(myVelocity * Time.deltaTime);

        UpdateGrounded();

        if (myHealth.IsDead())
            return;

        if (myStats.IsStunned())
            return;

        DetectLanding();
        DetectMovementInput();
    }

    private void DetectMovementInput()
    {
        if (Time.timeScale <= 0.0f)
            return;

        Vector3 previousVelocity = myVelocity;
        Vector2 leftStickAxis = myPlayerControls.Movement;

        myVelocity = (leftStickAxis.x * myCameraXZTransform.myRight + leftStickAxis.y * myCameraXZTransform.myForwards).normalized;

        PlayerTargetingComponent.ManualHealTargetingMode manualHealTargetingMode = myTargetingComponent.GetHealTargetingMode();
        if (manualHealTargetingMode != PlayerTargetingComponent.ManualHealTargetingMode.LeftJoystick)
            RotatePlayer();

        //Allow rotationg of player despite speedMultiplier being 0
        myVelocity *= myBaseSpeed * Mathf.Max(0.0f, GetComponent<Stats>().mySpeedMultiplier); //If speedMultiplier is < 0 we go backwards
        bool isMoving = IsMoving();

        if (!myIsGrounded)
        {
            myVelocity.y = previousVelocity.y;
        }
        else
        {
            if (manualHealTargetingMode != PlayerTargetingComponent.ManualHealTargetingMode.NotActive || myCastingComponent.HasSameLookDirectionAfterReleasingManualHeal())
            {
                myVelocity = Vector2.zero;
                return;
            }

            myAnimatorWrapper.SetBool(AnimationVariable.IsRunning, isMoving);
        }

        if (myPlayerControls.Jump.WasPressed && !myHasJumped)
        {
            if (myIsGrounded || Time.time - myStartFallingTimestamp < 0.2f)
            {
                myVelocity.y = myJumpSpeed;
                myAnimatorWrapper.ResetTrigger(AnimationVariable.Land);
                myAnimatorWrapper.SetBool(AnimationVariable.IsGrounded, false);
                myAnimatorWrapper.SetTrigger(AnimationVariable.Jump);

                myHasJumped = true;
            }
        }
    }

    protected bool IsGrounded()
    {
        if (myVelocity.y > 0.0f)
            return false;

        const float offsetLength = 0.3f;
        Vector3 offset = new Vector3(0.0f, offsetLength, 0.0f);

        float distance = 0.5f + offsetLength;
        Ray ray = new Ray(transform.position + offset, Vector3.down);
        LayerMask layerMask = LayerMask.GetMask("Terrain");

        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, distance, layerMask))
        {
            if (Vector3.Dot(hitInfo.normal, Vector3.down) < -0.6f)
            {
                MovablePlatform movablePlatform = hitInfo.collider.gameObject.GetComponent<MovablePlatform>();
                if(myMovablePlatform != movablePlatform)
                {
                    if(myMovablePlatform)
                        myMovablePlatform.RemoveFromPlatform(gameObject);

                    myMovablePlatform = movablePlatform;
                    if(myMovablePlatform)
                        myMovablePlatform.AddToPlatform(gameObject);
                }
                return true;
            }
        }

        if(myMovablePlatform)
        {
            myMovablePlatform.RemoveFromPlatform(gameObject);
            myMovablePlatform = null;
        }

        return false;
    }

    private void UpdateGrounded()
    {
        bool wasGrounded = myIsGrounded;

        myIsGrounded = IsGrounded();

        if (myIsGrounded)
            myPreviousGroundPosition = transform.position;

        if (!myIsGrounded && wasGrounded) //Started falling/jumping
        {
            myStartFallingTimestamp = Time.time;
        }
        else if(myIsGrounded && !wasGrounded) //Landed
        {
            if (myHealth.IsDead())
                myVelocity = Vector3.zero;

            if (!myAnimatorWrapper.GetBool(AnimationVariable.IsGrounded) && myIsGrounded)
                myAnimatorWrapper.SetBool(AnimationVariable.IsGrounded, true);

            myHasJumped = false;
        }

        SlideOnAngledSurface();
        HandleEndlessFalling();
    }

    private void HandleEndlessFalling()
    {
        if (myIsGrounded || myHealth.IsDead())
            return;

        const float endlessFallingDuration = 5.0f;
        if (Time.time - myStartFallingTimestamp > endlessFallingDuration)
            transform.position = myPreviousGroundPosition;
    }

    private void SlideOnAngledSurface()
    {
        if (myIsGrounded)
            return;

        const float offsetLength = 0.3f;
        Vector3 offset = new Vector3(0.0f, offsetLength, 0.0f);

        float distance = 0.3f + offsetLength;
        Ray ray = new Ray(transform.position + offset, Vector3.down);
        LayerMask layerMask = LayerMask.GetMask("Terrain");

        Vector3 pushDirection = Vector3.zero;
        Vector3 horizontalSlopeNormal = Vector3.zero;
        if (DoesRayHitSlope(ray, distance, layerMask, ref horizontalSlopeNormal))
        {
            pushDirection += horizontalSlopeNormal;
        }

        const int circlePoints = 7;
        float radius = myController.radius;
        for (int index = 0; index < circlePoints; index++)
        {
            float angle = (Mathf.PI * 2.0f) / circlePoints * index;
            ray.origin = transform.position + new Vector3(radius * Mathf.Cos(angle), offsetLength, radius * Mathf.Sin(angle));
            if (DoesRayHitSlope(ray, distance, layerMask, ref horizontalSlopeNormal))
            {
                pushDirection += horizontalSlopeNormal;
            }
        }

        myVelocity += pushDirection.normalized;
    }

    private bool DoesRayHitSlope(Ray aRay, float aDistance, LayerMask aLayermask, ref Vector3 outNormal)
    {
        Debug.DrawLine(aRay.origin, aRay.origin + Vector3.down * aDistance);

        RaycastHit hitInfo;
        if (Physics.Raycast(aRay, out hitInfo, aDistance, (int)aLayermask))
        {
            if (Vector3.Dot(hitInfo.normal, Vector3.up) < 0.5f)
            {
                outNormal = hitInfo.normal;
                outNormal.y = 0.0f;
                return true;
            }
        }

        return false;
    }

    private void DetectLanding()
    {
        if (myIsGrounded)
            return;

        if (myVelocity.y > 0.0f)
            return;

        Ray ray = new Ray(transform.position, Vector3.down);
        float distance = Mathf.Abs(myVelocity.y) * 0.15f;
        LayerMask layerMask = LayerMask.GetMask("Terrain");

        //If needed for future - predict actual landing position with curves and speed.

        //Debug.DrawLine(transform.position, transform.position + Vector3.down * distance);

        if (Physics.Raycast(ray, distance, layerMask))
        {
            myAnimatorWrapper.SetTrigger(AnimationVariable.Land);
        }
    }

    private void RotatePlayer()
    {
        if (myVelocity == Vector3.zero)
            return;

        transform.rotation = Quaternion.LookRotation(myVelocity, Vector3.up);
    }

    public override bool IsMoving()
    {
        if (myIsMovementDisabled)
            return false;

        if (myVelocity.x != 0 || myVelocity.z != 0)
            return true;

        if (!myIsGrounded)
            return true;

        return false;
    }

    public void GiveImpulse(Vector3 aVelocity, float aStunDuration = 0.2f)
    {
        myStats.SetStunned(aStunDuration);
        myVelocity = aVelocity;
    }

    public void GiveImpulse(Vector3 aVelocity, Vector3 aLookAtPosition, float aStunDuration = 0.2f)
    {
        GiveImpulse(aVelocity, aStunDuration);
        transform.LookAt(aLookAtPosition);
    }

    protected override void OnDeath()
    {
        myVelocity.x = 0.0f;
        myVelocity.z = 0.0f;
    }

    public void SetEnabledMovement(bool anIsEnabled)
    {
        myIsMovementDisabled = !anIsEnabled;
        GetComponent<CharacterController>().enabled = anIsEnabled;

        if (anIsEnabled)
            myStartFallingTimestamp = Time.time;
    }

    public bool IsMovementDisabled()
    {
        return myIsMovementDisabled;
    }
}
