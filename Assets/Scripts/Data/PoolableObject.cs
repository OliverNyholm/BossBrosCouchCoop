using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PoolableObject : MonoBehaviour
{
    private ObjectPool myObjectPool;

    public abstract void Reset();

    public void SetParentPool(ObjectPool aObjectPool)
    {
        myObjectPool = aObjectPool;
    }

    public virtual void ReturnToPool()
    {
        gameObject.SetActive(false);
        myObjectPool.ReturnObject(gameObject);
    }
}
