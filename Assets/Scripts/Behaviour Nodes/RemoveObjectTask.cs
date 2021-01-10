using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class ReturnToPool : Action
{
    public GameObject myObjectToReturn = null;

    public override TaskStatus OnUpdate()
    {
        if (!myObjectToReturn)
            myObjectToReturn = gameObject;

        PoolableObject poolableObject = myObjectToReturn.GetComponent<PoolableObject>();
        poolableObject.ReturnToPool();

        return TaskStatus.Success;
    }
}
