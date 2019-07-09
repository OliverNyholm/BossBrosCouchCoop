using UnityEngine;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class GetRandomGameObjects : Action
{
    public List<GameObject> myListOfGameObjects = new List<GameObject>();
    public SharedGameObjectList myReturnObjects = null;

    public int myRandomSize;

    public override TaskStatus OnUpdate()
    {
        if (myListOfGameObjects.Count == 0)
        {
            Debug.Log("No Gamobjects in list for GetRandomObjects.");
            return TaskStatus.Failure;
        }

        if (myListOfGameObjects.Count < myRandomSize)
        {
            Debug.Log("List of Gameobjects must have more objects than myRandomSize.");
            return TaskStatus.Failure;
        }

        List<GameObject> randomList = new List<GameObject>(myListOfGameObjects.Count);
        for (int index = 0; index < myListOfGameObjects.Count; index++)
            randomList.Add(myListOfGameObjects[index]);

        myReturnObjects.Value = new List<GameObject>(myRandomSize);
        for (int index = 0; index < myRandomSize; index++)
        {
            int randomIndex = Random.Range(0, randomList.Count);
            myReturnObjects.Value.Add(randomList[randomIndex]);

            randomList.RemoveAt(randomIndex);
        }

        return TaskStatus.Success;
    }
}
