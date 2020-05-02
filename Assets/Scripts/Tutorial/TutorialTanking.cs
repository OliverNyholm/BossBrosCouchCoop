using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;

public class TutorialTanking : TutorialCompletion
{
    [SerializeField]
    private GameObject myTutorialClass = null;
    [SerializeField]
    private GameObject myBoss = null;

    [SerializeField]
    private GameManager myGameManager = null;

    [SerializeField]
    private DynamicCamera myDynamicCamera = null;

    private Subscriber mySubscriber;

    protected override bool StartTutorial()
    {
        if (!base.StartTutorial())
            return false;

        myGameManager.ChangeClassInGame(myTutorialClass);
        myDynamicCamera.ReplacePlayersToTarget(myTargetHandler);

        myTargetHandler.AddEnemy(myBoss, true);
        myBoss.GetComponent<Health>().EventOnHealthZero += OnTargetDied;

        mySubscriber = new Subscriber();
        mySubscriber.EventOnReceivedMessage += ReceiveMessage;

        PostMaster.Instance.RegisterSubscriber(ref mySubscriber, MessageCategory.Wipe);

        return true;
    }

    private void ReceiveMessage(Message aMessage)
    {
        mySubscriber.EventOnReceivedMessage -= ReceiveMessage;

        myBoss.GetComponent<Health>().SetHealthPercentage(1.0f);
        myBoss.GetComponent<BehaviorTree>().SendEvent("Disengage");
    }

    private void OnTargetDied()
    {
        PostMaster.Instance.UnregisterSubscriber(ref mySubscriber, MessageCategory.Wipe);
        myTargetHandler.RemoveEnemy(myBoss);
        EndTutorial();
    }
}
