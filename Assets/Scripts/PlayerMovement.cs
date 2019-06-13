using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public int myControllerIndex;
    public float myBaseSpeed;
    public float myJumpSpeed;
    public float myGravity;

    public bool myShouldStrafe = true;

    private CharacterController myController;
    private Camera myCamera;
    private Animator myAnimator;

    private Vector3 myDirection;

    private bool myIsGrounded;

    void Start()
    {
        myCamera = Camera.main;

        myController = transform.GetComponent<CharacterController>();
        myAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        myDirection.y -= myGravity * Time.deltaTime;
        myController.Move(myDirection * Time.deltaTime);

        //myController.isGrounded unstable further ahead is seems...
        myIsGrounded = myController.isGrounded;
        if (!myAnimator.GetBool("IsGrounded") && myIsGrounded)
            myAnimator.SetBool("IsGrounded", true);


        DetectMovementInput();
        RotatePlayer();

    }
    private void DetectMovementInput()
    {
        if (!myIsGrounded)
            return;

        if (myShouldStrafe)
        {
            myDirection = new Vector3(Input.GetAxisRaw("LeftHorizontal" + myControllerIndex), 0.0f, Input.GetAxisRaw("LeftVertical" + myControllerIndex)).normalized * myBaseSpeed;
        }
        else
        {
            myDirection = new Vector3(0.0f, 0.0f, Input.GetAxisRaw("LeftVertical" + myControllerIndex)) * myBaseSpeed;
        }

        myDirection = transform.TransformDirection(myDirection);

        myAnimator.SetBool("IsRunning", IsMoving());

        if (Input.GetButton("RightBumper" + myControllerIndex))
        {
            myDirection.y = myJumpSpeed;
            myAnimator.SetBool("IsGrounded", false);
            myAnimator.SetTrigger("Jump");
        }
    }
    private void RotatePlayer()
    {
        if (myShouldStrafe)
        {
            //if (Mathf.Abs(myDirection.x) > 0.0f || Mathf.Abs(myDirection.y) > 0.0f)
            //{
            //    Vector3 newRotation = transform.eulerAngles;
            //    newRotation.y = myCamera.transform.eulerAngles.y;
            //    transform.eulerAngles = newRotation;
            //}
        }
        else
        {
            if (Input.GetAxisRaw("LeftHorizontal" + myControllerIndex) != 0.0f)
            {
                Vector3 newRotation = transform.eulerAngles;
                newRotation.y += Input.GetAxisRaw("LeftHorizontal" + myControllerIndex) * 2.0f;
                transform.eulerAngles = newRotation;
            }
        }
    }
    private bool IsMoving()
    {
        if (myDirection.x != 0 && myDirection.z != 0)
            return true;

        if (!myIsGrounded)
            return true;

        return false;
    }
}