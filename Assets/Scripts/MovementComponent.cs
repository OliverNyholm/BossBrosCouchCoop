using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovementComponent : MonoBehaviour
{
    public float myBaseSpeed = 8.0f;

    public abstract bool IsMoving();

    protected abstract void OnDeath();
}
