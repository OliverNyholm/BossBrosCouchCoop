using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialAutoTimed : TutorialCompletion
{
    [SerializeField]
    private float myTimerUntilComplete = 8.0f;

    [SerializeField]
    private Collider myInstantCompleteHitbox = null;

    private void Update()
    {
        if (!myHasStarted)
            return;

        myTimerUntilComplete -= Time.deltaTime;
        if (myTimerUntilComplete <= 0.0f)
            EndTutorial();
    }

    public override void OnChildTriggerEnter(Collider aChildCollider, Collider aHit)
    {
        base.OnChildTriggerEnter(aChildCollider, aHit);

        ObjectTag objectTag = aHit.GetComponentInParent<ObjectTag>();
        if (!objectTag)
            return;

        if (aChildCollider == myInstantCompleteHitbox && objectTag.IsTargetType(SpellTargetType.Player))
        {
            EndTutorial();
        }
    }

    protected override void EndTutorial()
    {
        base.EndTutorial();

        myInstantCompleteHitbox.enabled = false;
        this.enabled = false;
    }
}
