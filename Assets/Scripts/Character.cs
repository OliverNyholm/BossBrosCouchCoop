using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : PoolableObject
{
    protected AnimatorWrapper myAnimator;

    protected Health myHealth;
    protected Resource myResource;
    protected Stats myStats;

    protected float myStunDuration;

    protected virtual void Awake()
    {
        if (!myAnimator)
            SetComponents();
    }

    private void SetComponents()
    {
        myAnimator = GetComponent<AnimatorWrapper>();

        myHealth = GetComponent<Health>();
        myResource = GetComponent<Resource>();
        myStats = GetComponent<Stats>();

        myHealth.EventOnHealthZero += OnDeath;
    }

    protected virtual void Update()
    {
    }

    public bool CanRaycastToObject(GameObject anObject)
    {
        Vector3 hardcodedEyePosition = new Vector3(0.0f, 1.4f, 0.0f);
        Vector3 infrontOfPlayer = (transform.position + hardcodedEyePosition) + transform.forward;
        Vector3 direction = (anObject.transform.position + hardcodedEyePosition) - infrontOfPlayer;

        Ray ray = new Ray(infrontOfPlayer, direction);
        float distance = Vector3.Distance(infrontOfPlayer, anObject.transform.position + hardcodedEyePosition);
        LayerMask layerMask = LayerMask.GetMask("Terrain");

        if (Physics.Raycast(ray, distance, layerMask))
        {
            return false;
        }

        return true;
    }

    protected virtual void OnDeath()
    {
        AnimatorWrapper animatorWrapper = GetComponent<AnimatorWrapper>();
        if (animatorWrapper)
            animatorWrapper.SetTrigger(AnimationVariable.Death);
    }

    public override void Reset()
    {
        myHealth.SetHealthPercentage(1.0f);
    }
}