using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostMaster : ScriptableObject
{
    public static PostMaster myInstance;

    private List<List<Subscriber>> mySubscribers;
    private Queue<Message> myMessages;

    public void DelegateMessages()
    {
        while(myInstance.myMessages.Count > 0)
        {
            Message message = myInstance.myMessages.Dequeue();

            for (int index = 0; index < myInstance.mySubscribers[(int)message.Type].Count; index++)
            {
                myInstance.mySubscribers[(int)message.Type][index].ReceiveMessage(message);
            }
        }
    }

    public static PostMaster Instance
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
        
        myInstance = ScriptableObject.CreateInstance<PostMaster>();
        myInstance.mySubscribers = new List<List<Subscriber>>((int)MessageCategory.Count);
        myInstance.myMessages = new Queue<Message>();
        for (int i = 0; i < (int)MessageCategory.Count; i++)
        {
            List<Subscriber> subscriberList = new List<Subscriber>();
            myInstance.mySubscribers.Add(subscriberList);
        }

        DontDestroyOnLoad(myInstance);
    }

    public void RegisterSubscriber(ref Subscriber aSubscriber, MessageCategory aMessageType)
    {
        mySubscribers[(int)aMessageType].Add(aSubscriber);
    }

    public void UnregisterSubscriber(ref Subscriber aSubscriber, MessageCategory aMessageType)
    {
        mySubscribers[(int)aMessageType].Remove(aSubscriber);
    }

    public void PostMessage(Message aMessage)
    {
        myMessages.Enqueue(aMessage);
    }
}
