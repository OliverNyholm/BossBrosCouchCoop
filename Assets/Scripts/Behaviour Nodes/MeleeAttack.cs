﻿using UnityEngine;
using UnityEngine.AI;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

//[TaskDescription("Will move to the target and start autoattacking.")]
public class MeleeAttack : Action
{
    public SharedGameObject myTarget;

    public SharedFloat myAutoAttackRange;
    public SharedFloat myAutoAttackCooldown;

    private NPCMovementComponent myMovementComponent;
    private NPCCastingComponent myCastingComponent;

    public override void OnStart()
    {
        myMovementComponent = GetComponent<NPCMovementComponent>();
        myCastingComponent = GetComponent<NPCCastingComponent>();
    }

    public override TaskStatus OnUpdate()
    {
        if (myTarget.Value == null)
            return TaskStatus.Failure;

        const float attackRangeOffset = 1.0f;
        float distanceSqr = (myTarget.Value.transform.position - transform.position).sqrMagnitude;
        float autoAttackRange = myAutoAttackRange.Value;
        float moveMinDistance = autoAttackRange - attackRangeOffset;
        if (distanceSqr > moveMinDistance * moveMinDistance)
            myMovementComponent.MoveTo(myTarget.Value.transform.position);
        else
        {
            transform.rotation = Quaternion.LookRotation((myTarget.Value.transform.position - transform.position).normalized, Vector3.up);
            myMovementComponent.Stop();
        }

        if (myAutoAttackCooldown.Value > 0.0f)
            return TaskStatus.Running;

        if (distanceSqr < autoAttackRange * autoAttackRange)
            myCastingComponent.AutoAttack();

        return TaskStatus.Running;
    }

    public override void OnEnd()
    {
        myMovementComponent.Stop();
    }
}
