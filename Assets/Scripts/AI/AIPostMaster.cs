using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPostMaster : ScriptableObject
{
    public static AIPostMaster myInstance;

    private List<List<AISubscriber>> mySubscribers;
    private Queue<AIMessage> myMessages;

    public void DelegateMessages()
    {
        while(myInstance.myMessages.Count > 0)
        {
            AIMessage aiMessage = myInstance.myMessages.Dequeue();

            for (int index = 0; index < myInstance.mySubscribers[(int)aiMessage.Type].Count; index++)
            {
                myInstance.mySubscribers[(int)aiMessage.Type][index].ReceiveMessage(aiMessage);
            }
        }
    }

    public static AIPostMaster Instance
    {
        get
        {
            if (myInstance)
                return myInstance;

            Create();

            return myInstance;
        }
    }

    public static void Create()
    {
        if (myInstance)
            return;

        // myInstance = new AIPostMaster();
        myInstance = ScriptableObject.CreateInstance<AIPostMaster>();
        myInstance.mySubscribers = new List<List<AISubscriber>>((int)AIMessageType.Count);
        myInstance.myMessages = new Queue<AIMessage>();
        for (int i = 0; i < (int)AIMessageType.Count; i++)
        {
            List<AISubscriber> subscriberList = new List<AISubscriber>();
            myInstance.mySubscribers.Add(subscriberList);
        }

        DontDestroyOnLoad(myInstance);
    }

    public void RegisterSubscriber(ref AISubscriber anAISubscriber, AIMessageType aMessageType)
    {
        mySubscribers[(int)aMessageType].Add(anAISubscriber);
    }

    public void UnregisterSubscriber(ref AISubscriber anAISubscriber, AIMessageType aMessageType)
    {
        mySubscribers[(int)aMessageType].Remove(anAISubscriber);
    }

    public void PostAIMessage(AIMessage anAIMessage)
    {
        myMessages.Enqueue(anAIMessage);
    }
}
