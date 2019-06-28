using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObject : MonoBehaviour
{
    [Header("The direction")]
    [SerializeField]
    private Vector3 myDirection = Vector3.zero;
    [Header("The rotation speed")]
    [SerializeField]
    private float myMovementSpeed = 1.0f;

    [Header("The rotation per second")]
    [SerializeField]
    private Vector3 myEulerRotation = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        myDirection.Normalize();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(myEulerRotation * Time.deltaTime, Space.World);

        transform.position += myDirection * Time.deltaTime;
    }
}
