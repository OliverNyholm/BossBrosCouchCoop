using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectCamera : MonoBehaviour
{
    [Header("The offset the camera will have from each level")]
    [SerializeField]
    private Vector3 myOffset = Vector3.zero;
    private Quaternion myBaseRotation;

    [Header("The speed of the camera\'s movement")]
    [SerializeField]
    private float mySpeed = 1.0f;

    private Coroutine myMovementRoutine;

    void Awake()
    {
        myBaseRotation = transform.rotation;
    }

    public void SetTargetPosition(Transform aTargetTransform)
    {
        Vector3 targetPosition = aTargetTransform.position;
        Quaternion targetRotation = aTargetTransform.rotation;

        Vector3 movement = myOffset.x * aTargetTransform.forward + myOffset.y * aTargetTransform.up + myOffset.z * aTargetTransform.forward;
        targetPosition += movement;

        targetRotation = aTargetTransform.rotation * myBaseRotation;

        float duration = Vector3.Distance(targetPosition, transform.position) / mySpeed * Time.deltaTime;
        if (duration < 1.5f)
            duration = 1.5f;

        if (myMovementRoutine != null)
            StopCoroutine(myMovementRoutine);
        myMovementRoutine = StartCoroutine(LerpToNextLevel(targetPosition, targetRotation, duration));
    }

    public void SetTargetPositionInstant(Transform aTargetTransform)
    {
        Vector3 targetPosition = aTargetTransform.position;
        Quaternion targetRotation = aTargetTransform.rotation;

        Vector3 movement = myOffset.x * aTargetTransform.forward + myOffset.y * aTargetTransform.up + myOffset.z * aTargetTransform.forward;
        targetPosition += movement;

        targetRotation = aTargetTransform.rotation * myBaseRotation;

        transform.position = targetPosition;
        transform.rotation = targetRotation;
    }

    IEnumerator LerpToNextLevel(Vector3 aTargetPosition, Quaternion aTargetRotation, float aDuration)
    {
        float myTimer = 0.0f;
        float myInterpolation = 0.0f;

        Vector3 startPosition = transform.position;
        Quaternion startRotation = transform.rotation;

        while (myTimer < aDuration)
        {
            myTimer += Time.deltaTime;
            myInterpolation = myTimer / aDuration;

            transform.position = Vector3.Lerp(startPosition, aTargetPosition, myInterpolation);
            transform.rotation = Quaternion.Lerp(startRotation, aTargetRotation, myInterpolation);

            yield return null;
        }

        transform.position = aTargetPosition;
        transform.rotation = aTargetRotation;
    }
}
