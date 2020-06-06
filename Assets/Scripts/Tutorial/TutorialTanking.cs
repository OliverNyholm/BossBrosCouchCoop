using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;

public class TutorialTanking : TutorialCompletion
{
    [SerializeField]
    private GameObject myTauntSpell = null;
    [SerializeField]
    private GameObject myBoss = null;

    private Subscriber mySubscriber;

    protected override void Awake()
    {
        base.Awake();

        Spell spell = myTauntSpell.GetComponent<Spell>();
        spell.CreatePooledObjects(PoolManager.Instance, 6);
    }

    protected override bool StartTutorial()
    {
        if (!base.StartTutorial())
            return false;

        foreach (GameObject player in Players)
            player.GetComponent<Class>().ReplaceSpell(myTauntSpell, 0);

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
