using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMessageData
{
    public int myInt;
    public float myFloat;
    public Vector2 myVector2;
    public Vector3 myVector3;

    public AIMessageData(int anInt)
    {
        myInt = anInt;
    }
    public AIMessageData(int anObjectID, int anInt)
    {
        myVector2 = new Vector2(anObjectID, anInt);
    }
    public AIMessageData(int anInt, Vector3 aVector3)
    {
        myInt = anInt;
        myVector3 = aVector3;
    }

    public AIMessageData() { }
}

public class AIMessage
{
    public AIMessage(AIMessageType anAIMessageType)
    {
        Type = anAIMessageType;
    }

    public AIMessage(AIMessageType anAIMessageType, AIMessageData aAIMessageData)
    {
        Type = anAIMessageType;
        Data = aAIMessageData;
    }

    public AIMessage(AIMessageType anAIMessageType, int aIntValue)
    {
        Type = anAIMessageType;
        Data = new AIMessageData
        {
            myInt = aIntValue
        };
    }

    public AIMessage(AIMessageType anAIMessageType, float aFloatValue)
    {
        Type = anAIMessageType;
        Data = new AIMessageData
        {
            myFloat = aFloatValue
        };
    }

    public AIMessage(AIMessageType anAIMessageType, Vector2 aVector2)
    {
        Type = anAIMessageType;
        Data = new AIMessageData
        {
            myVector2 = aVector2
        };
    }

    public AIMessage(AIMessageType anAIMessageType, Vector3 aVector3)
    {
        Type = anAIMessageType;
        Data = new AIMessageData
        {
            myVector3 = aVector3
        };
    }

    public AIMessage(AIMessageType anAIMessageType, int aIntValue, Vector3 aVector3)
    {
        Type = anAIMessageType;
        Data = new AIMessageData(aIntValue, aVector3);
    }

    public AIMessageType Type { get; }

    public AIMessageData Data { get; }
}