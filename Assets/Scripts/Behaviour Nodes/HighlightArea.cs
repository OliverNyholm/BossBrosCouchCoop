using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class HighlightArea : Action
{
    public GameObject myHighlightPrefab = null;
    private GameObject myHightlightObject = null;

    public SharedTransform mySpawnTransform = null;

    public float myHighlightDuration;
    public float myTimer;

    public override void OnStart()
    {
        if (mySpawnTransform.Value == null)
            mySpawnTransform.Value = transform;

        myTimer = myHighlightDuration;
    }

    public override TaskStatus OnUpdate()
    {
        myTimer -= Time.time;
        if(myTimer <= 0.0f)
            return TaskStatus.Success;

        return TaskStatus.Running;
    }

    public override void OnEnd()
    {

    }

    public override void OnBehaviorComplete()
    {

    }
}
