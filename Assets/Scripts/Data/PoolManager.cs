using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    [Header("The Pool prefab that others can use to pool objects")]
    [SerializeField]
    private GameObject myPoolPrefab = null;
    
    [Header("GameObject for HealthPool")]
    [SerializeField]
    private ObjectPool myHealthPool = null;

    [Header("GameObject for AutoAttack Pool")]
    [SerializeField]
    private ObjectPool myAutoAttackPool = null;

    private Dictionary<uint, ObjectPool> myObjectPoolDictionary = new Dictionary<uint, ObjectPool>();

    public static PoolManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public GameObject GetFloatingHealth()
    {
        return myHealthPool.GetPooled();
    }

    public ObjectPool GetAutoAttackPool()
    {
        return myAutoAttackPool;
    }

    public GameObject GetPoolPrefab()
    {
        return myPoolPrefab;
    }

    public GameObject GetPooledObject(uint anObjectID)
    {
        if(myObjectPoolDictionary.TryGetValue(anObjectID, out ObjectPool pool))
        {
            return pool.GetPooled();
        }
        else
        {
            Debug.LogError("There is no pool with id: " + anObjectID);
            return null;
        }
    }

    public void AddPoolableObjects(GameObject aPrefab, uint anID, int aSize)
    {
        ObjectPool objectPool;
        if(!myObjectPoolDictionary.ContainsKey(anID))
        { 
            GameObject objectPoolGO = Instantiate(myPoolPrefab, transform);
            objectPool = objectPoolGO.GetComponent<ObjectPool>();

            objectPool.name = aPrefab.name + " Pool";
            objectPool.SetPoolSize(aSize);
            objectPool.SetPrefab(aPrefab);

            myObjectPoolDictionary.Add(anID, objectPool);
        }
    }
}
