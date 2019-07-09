using UnityEngine;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class GetRandomGameObject : Action
{
    public List<GameObject> myListOfGameObjects = new List<GameObject>();
    public SharedGameObject myReturnObject = null;

    public override TaskStatus OnUpdate()
    {
        if (myListOfGameObjects.Count == 0)
        {
            Debug.Log("No Gamobjects in list for GetRandomObjects.");
            return TaskStatus.Failure;
        }

        int randomIndex = Random.Range(0, myListOfGameObjects.Count);
        myReturnObject.Value = myListOfGameObjects[randomIndex];

        return TaskStatus.Success;
    }
}
