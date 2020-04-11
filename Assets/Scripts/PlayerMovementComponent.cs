using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovementComponent : MovementComponent
{
    public float myJumpSpeed;
    public float myGravity;

    private CharacterController myController;
    private PlayerControls myPlayerControls;
    private AnimatorWrapper myAnimatorWrapper;
    private PlayerCastingComponent myCastingComponent;
    private PlayerTargetingComponent myTargetingComponent;

    private Health myHealth;
    private Stats myStats;

    private Vector3 myVelocity;
    private CameraXZTransform myCameraXZTransform;

    private bool myIsGrounded;

    private void Awake()
    {
        myController = GetComponent<CharacterController>();
        myAnimatorWrapper = GetComponent<AnimatorWrapper>();
        myCastingComponent = GetComponent<PlayerCastingComponent>();
        myTargetingComponent = GetComponent<PlayerTargetingComponent>();
        myHealth = GetComponent<Health>();
        myStats = GetComponent<Stats>();
    }

    void Start()
    {
        Camera camera = Camera.main;
        myCameraXZTransform.myForwards = camera.transform.forward;
        myCameraXZTransform.myForwards.y = 0.0f;
        myCameraXZTransform.myForwards.Normalize();

        myCameraXZTransform.myRight = camera.transform.right;
        myCameraXZTransform.myRight.y = 0.0f;
        myCameraXZTransform.myRight.Normalize();

        myVelocity = Vector3.zero;

        myHealth.EventOnHealthZero += OnDeath;
    }

    public void SetPlayerController(PlayerControls aPlayerControls)
    {
        myPlayerControls = aPlayerControls;
    }

    void Update()
    {
        myVelocity.y -= myGravity * Time.deltaTime;
        myController.Move(myVelocity * Time.deltaTime);

        //myController.isGrounded unstable further ahead is seems...
        myIsGrounded = IsGrounded();
        if (!myAnimatorWrapper.GetBool(AnimationVariable.IsGrounded) && myIsGrounded)
            myAnimatorWrapper.SetBool(AnimationVariable.IsGrounded, true);

        SlideOnAngledSurface();

        if (myHealth.IsDead())
            return;

        if (myStats.IsStunned())
            return;

        DetectLanding();
        DetectMovementInput();
    }

    private void DetectMovementInput()
    {
        if (!myIsGrounded)
            return;

        if (myCastingComponent.HasRecentlyFinishedHealTargeting())
            return;

        Vector2 leftStickAxis = myPlayerControls.Movement;

        myVelocity = (leftStickAxis.x * myCameraXZTransform.myRight + leftStickAxis.y * myCameraXZTransform.myForwards).normalized;
        myVelocity *= myBaseSpeed * GetComponent<Stats>().mySpeedMultiplier;

        bool isMoving = IsMoving();
        if (isMoving)
            RotatePlayer();

        if (myTargetingComponent.IsHealTargeting())
        {
            myVelocity = Vector2.zero;
            return;
        }

        myAnimatorWrapper.SetBool(AnimationVariable.IsRunning, isMoving);

        if (myPlayerControls.Jump.WasPressed)
        {
            myVelocity.y = myJumpSpeed;
            myAnimatorWrapper.ResetTrigger(AnimationVariable.Land);
            myAnimatorWrapper.SetBool(AnimationVariable.IsGrounded, false);
            myAnimatorWrapper.SetTrigger(AnimationVariable.Jump);
        }
    }

    bool IsGrounded()
    {
        if (myVelocity.y > 0.0f)
            return false;

        const float offsetLength = 0.3f;
        Vector3 offset = new Vector3(0.0f, offsetLength, 0.0f);

        float distance = 0.3f + offsetLength;
        Ray ray = new Ray(transform.position + offset, Vector3.down);
        LayerMask layerMask = LayerMask.GetMask("Terrain");

        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, distance, layerMask))
        {
            if (Vector3.Dot(hitInfo.normal, Vector3.down) < -0.5f)
                return true;
        }

        return false;
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
        transform.rotation = Quaternion.LookRotation(myVelocity, Vector3.up);
    }

    public override bool IsMoving()
    {
        if (myVelocity.x != 0 || myVelocity.z != 0)
            return true;

        if (!myIsGrounded)
            return true;

        return false;
    }

    public void GiveImpulse(Vector3 aVelocity)
    {
        myStats.SetStunned(0.2f);
        myVelocity = aVelocity;
    }

    public void GiveImpulse(Vector3 aVelocity, Vector3 aLookAtPosition)
    {
        GiveImpulse(aVelocity);
        transform.LookAt(aLookAtPosition);
    }

    protected override void OnDeath()
    {
        myVelocity.x = 0.0f;
        myVelocity.z = 0.0f;
    }

    private void OnStun()
    {
        if (!myIsGrounded)
            return;

        myVelocity.x = 0.0f;
        myVelocity.z = 0.0f;
    }
}
