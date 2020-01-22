using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [Header("The Object to pool")]
    [SerializeField]
    private GameObject myPrefabObject = null;

    [Header("The size of the pool")]
    [SerializeField]
    private int myPoolSize = 10;

    private Queue<GameObject> myPool;

    private void Start()
    {
        myPool = new Queue<GameObject>(myPoolSize);
        for (int index = 0; index < myPoolSize; index++)
        {
            GameObject instance = Instantiate(myPrefabObject, transform);
            instance.GetComponent<PoolableObject>().SetParentPool(this);
            instance.SetActive(false);

            myPool.Enqueue(instance);
        }
    }

    public GameObject GetPooled()
    {
        if (myPool.Count == 0)
        {
            Debug.Log("Pool is empty! For GameObject with name: " + myPrefabObject.name);
            return null;
        }

        GameObject pooled = myPool.Peek();
        myPool.Dequeue();

        pooled.SetActive(true);
        pooled.GetComponent<PoolableObject>().Reset();

        return pooled;
    }

    public void ReturnObject(GameObject aGameObject)
    {
        myPool.Enqueue(aGameObject);
        aGameObject.transform.parent = transform;
    }

    public void SetPrefab(GameObject aGameObject)
    {
        myPrefabObject = aGameObject;
    }

    public void SetPoolSize(int aSize)
    {
        myPoolSize = aSize;
    }
}
