using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFacingBillboard : MonoBehaviour
{
    private Camera myCamera;

    private void Start()
    {
        myCamera = Camera.main;
    }

    void LateUpdate()
    {
        transform.LookAt(transform.position + myCamera.transform.rotation * Vector3.forward,
            myCamera.transform.rotation * Vector3.up);
    }
}
