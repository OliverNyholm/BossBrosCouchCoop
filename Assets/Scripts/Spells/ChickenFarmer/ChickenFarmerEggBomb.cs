using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChickenFarmerEggBomb : AoeAttack
{
    private Vector3 myFallingDirection;

    private float myLifetime = 5.0f;
    private float myLifetimeTimer = 0.0f;

    public override void Reset()
    {
        base.Reset();
        myLifetimeTimer = myLifetime;
    }

    public void SetTargetPosition(Vector3 aTargetPosition)
    {
        myFallingDirection = (aTargetPosition - transform.position).normalized;
    }

    protected override void Update()
    {
        if (IsCollidingWithTerrain())
            OnReachTarget();

        myLifetimeTimer -= Time.deltaTime;
        if (myLifetimeTimer <= 0.0f)
            OnReachTarget();

        transform.position += myFallingDirection * mySpeed * Time.deltaTime;
        transform.Rotate(myRandomRotation * myRotationSpeed * Time.deltaTime * 0.5f);
    }

    public bool IsCollidingWithTerrain()
    {
        Ray ray = new Ray(transform.position, myFallingDirection);
        LayerMask layerMask = LayerMask.GetMask("Terrain");

        if (Physics.Raycast(ray, 0.3f, layerMask))
            return true;

        return false;
    }
}
