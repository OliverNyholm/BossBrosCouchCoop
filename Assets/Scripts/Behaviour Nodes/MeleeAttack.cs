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

    private NavMeshAgent myNavmeshAgent;
    private Animator myAnimator;

    public override void OnStart()
    {
        myNavmeshAgent = GetComponent<NavMeshAgent>();
        myAnimator = GetComponent<Animator>();
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
        {
            myNavmeshAgent.destination = myTarget.Value.transform.position;
            myAnimator.SetBool("IsRunning", true);
        }
        else
        {
            myNavmeshAgent.destination = transform.position;
            myAnimator.SetBool("IsRunning", false);
        }

        if (myAutoAttackCooldown.Value > 0.0f)
            return TaskStatus.Running;

        if (distanceSqr < autoAttackRange * autoAttackRange)
        {
            myNavmeshAgent.destination = transform.position;
            myAnimator.SetBool("IsRunning", false);
            GetComponent<Enemy>().AutoAttack();
        }

        return TaskStatus.Running;
    }

    public override void OnEnd()
    {
        myNavmeshAgent.destination = transform.position;
        myAnimator.SetBool("IsRunning", false);
    }
}
