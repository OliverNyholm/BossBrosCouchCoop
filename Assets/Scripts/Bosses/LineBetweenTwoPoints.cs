using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineBetweenTwoPoints : PoolableObject
{
    private Transform myFirstTransform = null;
    private Transform mySecondTransform = null;

    [SerializeField]
    private Vector3 myFirstPositionOffset = Vector3.up;
    [SerializeField]
    private Vector3 mySecondPositionOffset = Vector3.up;

    private Vector3 myOriginalScale;
    private Vector3 myMultiplyScale;

    private void Awake()
    {
        myOriginalScale = transform.localScale;
        myMultiplyScale = Vector3.one;
    }

    private void Update()
    {
        Vector3 firstPosition= myFirstTransform.position + myFirstPositionOffset;
        Vector3 secondPosition = mySecondTransform.position + mySecondPositionOffset;
        transform.position = firstPosition;
        transform.LookAt(secondPosition);

        float distanceBetweenPoints = (firstPosition - secondPosition).magnitude;
        myMultiplyScale.z = distanceBetweenPoints;
        transform.localScale = myOriginalScale.Multiply(myMultiplyScale);
    }

    public void SetPoints(Transform aFirstPoint, Transform aSecondPoint)
    {
        myFirstTransform = aFirstPoint;
        mySecondTransform = aSecondPoint;
    }

    public override void Reset()
    {
        myFirstTransform = null;
        mySecondTransform = null;
    }
}
