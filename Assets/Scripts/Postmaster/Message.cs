using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageData
{
    public int myInt;
    public float myFloat;
    public Vector2 myVector2;
    public Vector3 myVector3;

    public MessageData(int anInt)
    {
        myInt = anInt;
    }
    public MessageData(int anObjectID, int anInt)
    {
        myVector2 = new Vector2(anObjectID, anInt);
    }
    public MessageData(int anInt, Vector3 aVector3)
    {
        myInt = anInt;
        myVector3 = aVector3;
    }

    public MessageData() { }
}

public class Message
{
    public Message(MessageCategory aMessageType)
    {
        Type = aMessageType;
    }

    public Message(MessageCategory aMessageType, MessageData aAIMessageData)
    {
        Type = aMessageType;
        Data = aAIMessageData;
    }

    public Message(MessageCategory aMessageType, int aIntValue)
    {
        Type = aMessageType;
        Data = new MessageData
        {
            myInt = aIntValue
        };
    }

    public Message(MessageCategory aMessageType, float aFloatValue)
    {
        Type = aMessageType;
        Data = new MessageData
        {
            myFloat = aFloatValue
        };
    }

    public Message(MessageCategory aMessageType, Vector2 aVector2)
    {
        Type = aMessageType;
        Data = new MessageData
        {
            myVector2 = aVector2
        };
    }

    public Message(MessageCategory aMessageType, Vector3 aVector3)
    {
        Type = aMessageType;
        Data = new MessageData
        {
            myVector3 = aVector3
        };
    }

    public Message(MessageCategory aMessageType, int aIntValue, Vector3 aVector3)
    {
        Type = aMessageType;
        Data = new MessageData(aIntValue, aVector3);
    }

    public MessageCategory Type { get; }

    public MessageData Data { get; }
}