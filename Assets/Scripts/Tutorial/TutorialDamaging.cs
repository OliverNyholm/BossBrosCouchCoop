using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialDamaging : TutorialCompletion
{
    [Header("The class to try out damaging with")]
    [SerializeField]
    private GameObject myTutorialClass = null;

    [Header("The targets to kill")]
    [SerializeField]
    private GameObject myGroundTarget = null;
    [SerializeField]
    private GameObject myRangeTarget = null;

    [SerializeField]
    private GameManager myGameManager = null;

    [SerializeField]
    private DynamicCamera myDynamicCamera = null;

    int myKilledCount = 0;

    protected override bool StartTutorial()
    {
        if (!base.StartTutorial())
            return false;

        myGameManager.ChangeClassInGame(myTutorialClass);
        myDynamicCamera.ReplacePlayersToTarget(myTargetHandler);

        myTargetHandler.AddEnemy(myGroundTarget, true);
        myTargetHandler.AddEnemy(myRangeTarget, true);

        myGroundTarget.GetComponent<Health>().EventOnHealthZero += OnTargetDied;
        myRangeTarget.GetComponent<Health>().EventOnHealthZero += OnTargetDied;

        return true;
    }

    private void OnTargetDied()
    {
        myKilledCount++;
        if(myKilledCount == 2)
        {
            myTargetHandler.RemoveEnemy(myGroundTarget);
            myTargetHandler.RemoveEnemy(myRangeTarget);
            EndTutorial();
        }
    }
}
