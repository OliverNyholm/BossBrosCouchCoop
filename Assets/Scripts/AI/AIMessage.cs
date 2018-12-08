using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AIMessageData
{
    public NetworkInstanceId myNetworkID;
    public int myInt;
    public float myFloat;
    public Vector2 myVector2;
    public Vector3 myVector3;

    public AIMessageData(NetworkInstanceId anID, int anInt)
    {
        myNetworkID = anID;
        myInt = anInt;
    }

    public AIMessageData() { }
}

public class AIMessage
{
    private AIMessageType myAIMessageType;
    private AIMessageData myAIMessageData;

    public AIMessage(AIMessageType anAIMessageType)
    {
        myAIMessageType = anAIMessageType;
    }

    public AIMessage(AIMessageType anAIMessageType, AIMessageData aMessageData)
    {
        myAIMessageType = anAIMessageType;
        myAIMessageData = aMessageData;
    }

    public AIMessage(AIMessageType anAIMessageType, int aIntValue)
    {
        myAIMessageType = anAIMessageType;
        myAIMessageData.myInt = aIntValue;
    }

    public AIMessage(AIMessageType anAIMessageType, float aFloatValue)
    {
        myAIMessageType = anAIMessageType;
        myAIMessageData.myFloat = aFloatValue;
    }

    public AIMessage(AIMessageType anAIMessageType, Vector2 aVector2)
    {
        myAIMessageType = anAIMessageType;
        myAIMessageData.myVector2 = aVector2;
    }

    public AIMessage(AIMessageType anAIMessageType, Vector3 aVector3)
    {
        myAIMessageType = anAIMessageType;
        myAIMessageData.myVector3 = aVector3;
    }

    public AIMessageType Type
    {
        get { return myAIMessageType; }
    }

    public AIMessageData Data
    {
        get { return myAIMessageData; }
    }
}
