using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UniqueID))]
public abstract class PoolableObject : MonoBehaviour
{
    private ObjectPool myObjectPool;

    public abstract void Reset();

    public void SetParentPool(ObjectPool aObjectPool)
    {
        myObjectPool = aObjectPool;
    }

    public ObjectPool GetPool()
    {
        return myObjectPool;
    }

    public virtual void ReturnToPool()
    {
        gameObject.SetActive(false);
        if (myObjectPool)
            myObjectPool.ReturnObject(gameObject);
        else
        {
            Debug.LogError(gameObject.name + " is going to be deleted!");
            Destroy(gameObject);
        }
    }
}
