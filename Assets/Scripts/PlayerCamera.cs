using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public Transform myTarget;

    public float myMinDistance = 0.2f;
    public float myMaxDistance = 20.0f;
    public float myTargetHeight = 1.7f;
    public float myScrollSpeed = 5.0f;
    public float zoomDampening = 5.0f;
    public float myRotationSpeedX = 2.0f;
    public float myRotationSpeedY = 2.0f;

    private float myDesiredDistance;
    private float myCurrentDistance;
    private float myCorrectedDistance;
    private float myOffsetFromWall = 0.1f;

    private float myXDegrees;
    private float myYDegrees;

    private float myMinYRotation = -80;
    private float myMaxYRotation = 80;


    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        myXDegrees = angles.x;
        myYDegrees = angles.y;

        myMinDistance = 0.2f;
        myMaxDistance = 20.0f;

        myCurrentDistance = myDesiredDistance = myCorrectedDistance = 10.0f;
    }

    void LateUpdate()
    {
        if (!myTarget)
            return;

        if (Input.GetMouseButton(1))
        {
            myXDegrees += Input.GetAxis("Mouse X") * myRotationSpeedX;
            myYDegrees -= Input.GetAxis("Mouse Y") * myRotationSpeedY;
        }
        else if (myTarget.GetComponent<PlayerCharacterOLD>() && !myTarget.GetComponent<PlayerCharacterOLD>().myShouldStrafe && (Input.GetButton("Vertical") || Input.GetButton("Horizontal")))
        {
            float rotationDampening = 3.0f;
            float targetRotationAngle = myTarget.GetComponentInChildren<MeshFilter>().transform.eulerAngles.y;
            float currentRotationAngle = transform.eulerAngles.y;
            myXDegrees = Mathf.LerpAngle(currentRotationAngle, targetRotationAngle, rotationDampening * Time.deltaTime);
        }
        else if (myTarget.GetComponent<Player>() && !myTarget.GetComponent<Player>().myShouldStrafe && (Input.GetButton("Vertical") || Input.GetButton("Horizontal")))
        {
            float rotationDampening = 3.0f;
            float targetRotationAngle = myTarget.GetComponentInChildren<MeshFilter>().transform.eulerAngles.y;
            float currentRotationAngle = transform.eulerAngles.y;
            myXDegrees = Mathf.LerpAngle(currentRotationAngle, targetRotationAngle, rotationDampening * Time.deltaTime);
        }

        myYDegrees = ClampAngle(myYDegrees, myMinYRotation, myMaxYRotation);

        Quaternion rotation = Quaternion.Euler(myYDegrees, myXDegrees, 0.0f);

        myDesiredDistance -= Input.GetAxis("Mouse ScrollWheel") * myScrollSpeed;
        myDesiredDistance = Mathf.Clamp(myDesiredDistance, myMinDistance, myMaxDistance);
        myCorrectedDistance = myDesiredDistance;

        Vector3 targetOffset = new Vector3(0.0f, -myTargetHeight, 0.0f);
        Vector3 position = myTarget.position - (rotation * Vector3.forward * myDesiredDistance + targetOffset);

        RaycastHit collisionHit;
        LayerMask layerMask = LayerMask.GetMask("Terrain");
        Vector3 trueTargetPosition = new Vector3(myTarget.position.x, myTarget.position.y + myTargetHeight, myTarget.position.z);

        bool hasHitWall = false;
        if (Physics.Linecast(trueTargetPosition, position, out collisionHit, layerMask))
        {
            myCorrectedDistance = Vector3.Distance(trueTargetPosition, collisionHit.point) - myOffsetFromWall;
            hasHitWall = true;
        }

        if (hasHitWall || myCorrectedDistance > myCurrentDistance)
        {
            myCurrentDistance = Mathf.Lerp(myCurrentDistance, myCorrectedDistance, Time.deltaTime * zoomDampening);
        }

        myCurrentDistance = Mathf.Clamp(myCurrentDistance, myMinDistance, myMaxDistance);

        position = myTarget.position - (rotation * Vector3.forward * myCurrentDistance + targetOffset);

        transform.rotation = rotation;
        transform.position = position;
    }

    public void SetTarget(Transform aTarget)
    {
        myTarget = aTarget;
    }

    private float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;

        return Mathf.Clamp(angle, min, max);
    }
}
