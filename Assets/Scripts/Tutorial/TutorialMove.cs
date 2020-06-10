using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialMove : TutorialCompletion
{
    [SerializeField]
    private Collider myMoveToCollider = null;
    
    [SerializeField]
    private GameObject myHighlightObject = null;
    [SerializeField]
    private float myHighlightScale = 1.0f;
    [SerializeField]
    private bool myHighlightOnStart = false;

    protected override bool StartTutorial()
    {
        if (!base.StartTutorial())
            return false;

        if (myHighlightOnStart && myHighlightObject)
        {
            myHighlightManager.HighlightArea(myHighlightObject, Vector3.one * myHighlightScale);
        }

        return true;
    }

    protected override void EndTutorial()
    {
        if(!myHighlightOnStart && myHighlightObject)
            myHighlightManager.HighlightArea(myHighlightObject, Vector3.one * myHighlightScale);


        base.EndTutorial();
    }

    public override void OnChildTriggerEnter(Collider aChildCollider, Collider aHit)
    {
        base.OnChildTriggerEnter(aChildCollider, aHit);

        if (aChildCollider == myMoveToCollider && aHit.tag == "Player")
        {
            if (!myCompletedPlayers.Contains(aHit.gameObject))
            {
                myCompletedPlayers.Add(aHit.gameObject);
                if (myCompletedPlayers.Count == Players.Count)
                    EndTutorial();
            }
        }
    }
}
