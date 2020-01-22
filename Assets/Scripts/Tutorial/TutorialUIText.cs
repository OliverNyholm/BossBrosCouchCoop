using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialUIText : TutorialCompletion
{
    private Subscriber mySubscriber;

    protected override bool StartTutorial()
    {
        if (!base.StartTutorial())
            return false;

        Subscribe();
        return true;
    }

    public void OnInfoToggled(GameObject aPlayer)
    {
        if (myCompletedPlayers.Contains(aPlayer))
            return;

        myCompletedPlayers.Add(aPlayer);
        SetPlayerCompleted(aPlayer);
        if (myCompletedPlayers.Count == myPlayers.Count)
        {
            Unsubscribe();
            EndTutorial();
        }
    }

    private void Subscribe()
    {
        mySubscriber = new Subscriber();
        mySubscriber.EventOnReceivedMessage += ReceiveMessage;
        PostMaster.Instance.RegisterSubscriber(ref mySubscriber, MessageCategory.UITextToggle);
    }

    private void Unsubscribe()
    {
        mySubscriber.EventOnReceivedMessage -= ReceiveMessage;
        PostMaster.Instance.UnregisterSubscriber(ref mySubscriber, MessageCategory.UITextToggle);
    }

    private void ReceiveMessage(Message aMessage)
    {
        for (int index = 0; index < myPlayers.Count; index++)
        {
            if (myPlayers[index].GetInstanceID() == aMessage.Data.myInt)
            {
                OnInfoToggled(myPlayers[index]);
                break;
            }
        }
    }
}
