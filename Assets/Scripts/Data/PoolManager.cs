using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-200)]
public class PoolManager : MonoBehaviour
{
    [Header("The Pool prefab that others can use to pool objects")]
    [SerializeField]
    private GameObject myPoolPrefab = null;
    
    [Header("GameObject for HealthPool")]
    [SerializeField]
    private ObjectPool myHealthPool = null;

    [Header("AutoAttack prefab")]
    [SerializeField]
    private GameObject myAutoAttackPrefab = null;

    [Header("GameObject for Empty Transform Holder")]
    [SerializeField]
    private GameObject myEmptyTransformHolder = null;

    private Dictionary<uint, ObjectPool> myObjectPoolDictionary = new Dictionary<uint, ObjectPool>();

    class TemporaryObjectData
    {
       public float myLifeTime;
       public uint myID;
       public GameObject myGameObject;
    }

    private List<TemporaryObjectData> myTemporaryObject = new List<TemporaryObjectData>(32);

    public static PoolManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        myAutoAttackPrefab.GetComponent<Spell>().CreatePooledObjects(this, 20);
    }

    private void Update()
    {
        float deltaTime = Time.deltaTime;
        for (int index = 0; index < myTemporaryObject.Count; index++)
        {
            myTemporaryObject[index].myLifeTime -= deltaTime;
            if(myTemporaryObject[index].myLifeTime <= 0.0f)
            {
                myObjectPoolDictionary[myTemporaryObject[index].myID].ReturnObject(myTemporaryObject[index].myGameObject);
                myTemporaryObject.RemoveAt(index);
                index--;
            }
        }
    }

    public GameObject GetFloatingHealth()
    {
        return myHealthPool.GetPooled();
    }

    public Transform GetEmptyTransformHolder()
    {
        return myEmptyTransformHolder.transform;
    }

    public GameObject GetAutoAttackPrefab()
    {
        return myAutoAttackPrefab;
    }

    public GameObject GetPooledAutoAttack()
    {
        if (myObjectPoolDictionary.TryGetValue(myAutoAttackPrefab.GetComponent<UniqueID>().GetID(), out ObjectPool pool))
        {
            return pool.GetPooled();
        }
        else
        {
            Debug.LogError("Auto Attack pool is not created for some reason.");
            return null;
        }
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

    public void ReturnObject(GameObject aGameObject, uint anObjectID)
    {
        if (myObjectPoolDictionary.TryGetValue(anObjectID, out ObjectPool pool))
        {
            pool.ReturnObject(aGameObject);
        }
        else
        {
            Debug.LogError("Can't return " + aGameObject.name + " to pool. There is no pool with id: " + anObjectID);
        }
    }

    public void AddPoolableObjects(GameObject aPrefab, uint anID, int aSize)
    {
        ObjectPool objectPool;
        if(!myObjectPoolDictionary.TryGetValue(anID, out objectPool))
        { 
            GameObject objectPoolGO = Instantiate(myPoolPrefab, transform);
            objectPool = objectPoolGO.GetComponent<ObjectPool>();

            objectPool.name = aPrefab.name + " Pool";
            objectPool.SetPoolSize(aSize);
            objectPool.SetPrefab(aPrefab);

            myObjectPoolDictionary.Add(anID, objectPool);
        }
        else
        {
            objectPool.IncreasePoolSize(aSize);
        }
    }

    public void AddTemporaryObject(GameObject aGameObject, float aDuration)
    {
        TemporaryObjectData data = new TemporaryObjectData();
        data.myLifeTime = aDuration;
        data.myGameObject = aGameObject;
        data.myID = aGameObject.GetComponent<UniqueID>().GetID();

        myTemporaryObject.Add(data);
    }

    public ObjectPool FindObjectPool(uint aUniqueId)
    {
        myObjectPoolDictionary.TryGetValue(aUniqueId, out ObjectPool pool);
        return pool;
    }
}
