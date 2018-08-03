using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerCharacter : NetworkBehaviour
{

    public float mySpeed;
    public float myJumpSpeed;
    public float myGravity;

    public Vector3 myDirection;

    private CharacterController myController;
    private Camera myCamera;

    // Use this for initialization
    void Start()
    {
        myController = transform.GetComponent<CharacterController>();

        myCamera = Camera.main;
        myCamera.GetComponent<PlayerCamera>().SetTarget(this.transform);
    }

    // Update is called once per frame
    void Update()
    {

        if (!hasAuthority)
            return;

        DetectMovementInput();

        myDirection.y -= myGravity * Time.deltaTime;

        myController.Move(myDirection * Time.deltaTime);
    }

    private void DetectMovementInput()
    {
        if (!myController.isGrounded)
            return;

        myDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0.0f, Input.GetAxisRaw("Vertical")) * mySpeed;
        myDirection = myCamera.transform.TransformDirection(myDirection);

        if (Input.GetButton("Horizontal") || Input.GetButton("Vertical"))
        {
            Vector3 newRotation = transform.eulerAngles;
            newRotation.y = myCamera.transform.eulerAngles.y;
            transform.eulerAngles = newRotation;
        }

        if (Input.GetButtonDown("Jump"))
            myDirection.y = myJumpSpeed;
    }
}
