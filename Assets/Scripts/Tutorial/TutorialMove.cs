using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialMove : TutorialCompletion
{
    [SerializeField]
    private Collider myMoveToCollider = null;

    public override void OnChildTriggerEnter(Collider aChildCollider, Collider aHit)
    {
        base.OnChildTriggerEnter(aChildCollider, aHit);

        if (aChildCollider == myMoveToCollider && aHit.tag == "Player")
        {
            if (!myCompletedPlayers.Contains(aHit.gameObject))
            {
                myCompletedPlayers.Add(aHit.gameObject);
                SetPlayerCompleted(aHit.gameObject);
                if (myCompletedPlayers.Count == Players.Count)
                    EndTutorial();
            }
        }
    }
}
