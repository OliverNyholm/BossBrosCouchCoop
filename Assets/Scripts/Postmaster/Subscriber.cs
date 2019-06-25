using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Subscriber
{
    public delegate void ReceivedMessage(Message anAIMessage);
    public event ReceivedMessage EventOnReceivedMessage;

    public Subscriber()
    {

    }

    public void ReceiveMessage(Message anAIMessage)
    {
        EventOnReceivedMessage?.Invoke(anAIMessage);
    }
}
