using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISubscriber
{
    public delegate void ReceivedMessage(AIMessage anAIMessage);
    public event ReceivedMessage EventOnReceivedMessage;

    public AISubscriber()
    {

    }

    public void ReceiveMessage(AIMessage anAIMessage)
    {
        EventOnReceivedMessage?.Invoke(anAIMessage);
    }
}
