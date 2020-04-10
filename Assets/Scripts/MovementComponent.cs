using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovementComponent : MonoBehaviour
{
    public abstract bool IsMoving();

    protected abstract void OnDeath();
}
