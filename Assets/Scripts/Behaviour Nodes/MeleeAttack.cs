using UnityEngine;
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

    public override void OnAwake()
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
            Vector3 toTarget = (myTarget.Value.transform.position - transform.position);
            toTarget.y = 0.0f;

            Vector3 normalized = toTarget.normalized;
            if(normalized != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(toTarget.normalized, Vector3.up);

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
