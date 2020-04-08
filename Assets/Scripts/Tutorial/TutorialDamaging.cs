using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialDamaging : TutorialCompletion
{
    [Header("The class to try out damaging with")]
    [SerializeField]
    private GameObject myTutorialMelee = null;
    [SerializeField]
    private GameObject myTutorialCaster = null;

    [Header("The targets to kill")]
    [SerializeField]
    private GameObject myGroundTarget = null;
    [SerializeField]
    private GameObject myRangeTarget = null;

    [SerializeField]
    private GameManager myGameManager = null;

    [SerializeField]
    private DynamicCamera myDynamicCamera = null;

    private Subscriber mySubscriber;

    int myKilledCount = 0;

    protected override bool StartTutorial()
    {
        if (!base.StartTutorial())
            return false;

        myGameManager.ChangeClassInGame(myTutorialMelee);
        myDynamicCamera.ReplacePlayersToTarget(myTargetHandler);

        mySubscriber = new Subscriber();
        mySubscriber.EventOnReceivedMessage += ReceiveMessage;

        myTargetHandler.AddEnemy(myGroundTarget, true);
        myTargetHandler.AddEnemy(myRangeTarget, true);

        return true;
    }

    public void ReceiveMessage(Message aMessage)
    {
        switch (aMessage.Type)
        {
            case MessageCategory.EnemyDied:

                if (myKilledCount == 0)
                {
                    myGameManager.ChangeClassInGame(myTutorialCaster);
                    myDynamicCamera.ReplacePlayersToTarget(myTargetHandler);
                }
                else
                {
                    EndTutorial();
                }
                break;
            default:
                break;
        }
    }
}
