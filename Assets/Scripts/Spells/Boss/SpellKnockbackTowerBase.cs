using BehaviorDesigner.Runtime;
using UnityEngine;
using EZCameraShake;

public class SpellKnockbackTowerBase : SpellKnockback
{
    [SerializeField]
    private string myEventName = "PlayerHitWall";
    [SerializeField]
    private LayerMask myCollisionLayerMask = 0;

    private Stats myTargetStats = null;

    protected override void Awake()
    {
        base.Awake();
        myReturnToPoolWhenReachedTarget = false;
    }

    public override void Restart()
    {
        base.Restart();

        if(myTarget)
            myTargetStats = myTarget.GetComponent<Stats>();
    }

    protected override void Update()
    {
        if(myIsFirstUpdate)
            base.Update();

        if (!myTargetStats || !myTargetStats.IsStunned())
        {
            ReturnToPool();
            return;
        }

        const float distance = 0.7f;
        const float heightOffset = 1.3f;
        Ray ray = new Ray(myTarget.transform.position + Vector3.up * heightOffset, myKnockbackHorizontalDirection);
        if (Physics.Raycast(ray, distance, myCollisionLayerMask))
        {
            BehaviorTree behaviorTree = myParent.GetComponent<BehaviorTree>();
            behaviorTree.SendEvent(myEventName);
            CameraShaker.Instance.ShakeOnce(2.0f, 2.0f, 0.1f, 1.0f);
            
            ReturnToPool();
        }
    }
}