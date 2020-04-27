using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPuzzle : TutorialCompletion
{
    [SerializeField]
    private List<Collider> myPuzzleColliders = new List<Collider>();
    private List<int> myPuzzleColliderAmount = new List<int>();

    protected override bool StartTutorial()
    {
        if (!base.StartTutorial())
            return false;

        for (int index = 0; index < myPuzzleColliders.Count; index++)
        {
            myPuzzleColliderAmount.Add(0);
        }

        return true;
    }

    public override void OnChildTriggerEnter(Collider aChildCollider, Collider aHit)
    {
        base.OnChildTriggerEnter(aChildCollider, aHit);

        for (int index = 0; index < myPuzzleColliders.Count; index++)
        {
            if (aChildCollider != myPuzzleColliders[index])
                continue;

            if (aHit.tag == "Player")
            {
                myPuzzleColliderAmount[index]++;
                int triggersContainingPlayers = 0;
                for (int puzzleIndex = 0; puzzleIndex < myPuzzleColliderAmount.Count; puzzleIndex++)
                {
                    if (myPuzzleColliderAmount[puzzleIndex] > 0)
                        triggersContainingPlayers++;
                }

                if (triggersContainingPlayers == Players.Count)
                    EndTutorial();
            }
        }
    }

    public override void OnChildTriggerExit(Collider aChildCollider, Collider aHit)
    {
        for (int index = 0; index < myPuzzleColliders.Count; index++)
        {
            if (aChildCollider != myPuzzleColliders[index])
                continue;

            if (aHit.tag == "Player")
            {
                myPuzzleColliderAmount[index]--;
            }
        }
    }
}
