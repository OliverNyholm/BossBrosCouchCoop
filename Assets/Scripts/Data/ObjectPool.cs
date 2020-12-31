using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(10)]
public class ObjectPool : MonoBehaviour
{
    [Header("The Object to pool")]
    [SerializeField]
    private GameObject myPrefabObject = null;

    [Header("The size of the pool")]
    [SerializeField]
    private int myPoolSize = 10;

    private Queue<GameObject> myPool;
    private bool myHasCreatedPool = false;

    private void Start()
    {
        TargetHandler targetHandler = FindObjectOfType<TargetHandler>();

        myPool = new Queue<GameObject>(myPoolSize);
        for (int index = 0; index < myPoolSize; index++)
        {
            GameObject instance = Instantiate(myPrefabObject, transform);

            PoolableObject poolableObject = instance.GetComponent<PoolableObject>();
            if(poolableObject)
                poolableObject.SetParentPool(this);

            Spell spell = instance.GetComponent<Spell>();
            if (spell)
                spell.SetTargetHandler(targetHandler);

            instance.SetActive(false);

            myPool.Enqueue(instance);
        }
        myHasCreatedPool = true;
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
        pooled.transform.localPosition = Vector3.zero;
        pooled.transform.localRotation = Quaternion.identity;

        PoolableObject poolableObject = pooled.GetComponent<PoolableObject>();
        if(poolableObject)
            poolableObject.Reset();

        return pooled;
    }

    public void ReturnObject(GameObject aGameObject)
    {
        myPool.Enqueue(aGameObject);
        aGameObject.transform.SetParent(transform, false);

        if (aGameObject.activeInHierarchy)
            aGameObject.SetActive(false);
    }

    public void SetPrefab(GameObject aGameObject)
    {
        myPrefabObject = aGameObject;
    }

    public void SetPoolSize(int aSize)
    {
        myPoolSize = aSize;
    }

    public void IncreasePoolSize(int aSize)
    {
        if(!myHasCreatedPool)
        {
            myPoolSize += aSize;
        }
        else
        {
            //Debug.LogWarning("Pool has already been created, size was not increased for: " + myPool.Peek().name);
        }
    }
}
