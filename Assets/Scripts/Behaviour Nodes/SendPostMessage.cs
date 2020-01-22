using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class SendPostMessage : Action
{
    public MessageCategory myMessageType;
    public int myIntData;
    public float myFloatData;
    public Vector2 myVector2Data;
    public Vector3 myVector3Data;

    public override TaskStatus OnUpdate()
    {
        MessageData messageData = new MessageData();
        messageData.myInt = myIntData;
        messageData.myFloat = myFloatData;
        messageData.myVector2 = myVector2Data;
        messageData.myVector3 = myVector3Data;
        Message message = new Message(myMessageType, messageData);

        PostMaster.Instance.PostMessage(message);

        return TaskStatus.Success;
    }
}
