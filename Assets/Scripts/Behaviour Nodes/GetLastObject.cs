using UnityEngine;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class GetLastObject : Action
{
    public SharedGameObjectList myListOfGameObjects = null;
    public SharedGameObject myReturnObject = null;

    public override TaskStatus OnUpdate()
    {
        if (myListOfGameObjects == null || myListOfGameObjects.Value.Count == 0)
        {
            Debug.Log("No Gamobjects in list for GetRandomObjects.");
            return TaskStatus.Failure;
        }

        myReturnObject.Value = myListOfGameObjects.Value[myListOfGameObjects.Value.Count - 1];

        return TaskStatus.Success;
    }
}