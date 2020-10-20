using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class SpawnObject : Action
{
    public GameObject myObject = null;

    public SharedTransform mySpawnTransform = null;
    [Header("If only want to use position, use spawn position")]
    public SharedVector3 mySpawnPosition = null;

    public int mySpellMaxCount = 4;
    public float myLifetime = 0;

    private GameObject myEmptyTransformHolder;

    public override void OnAwake()
    {
        base.OnAwake();

        PoolManager poolManager = PoolManager.Instance;
        myEmptyTransformHolder = Object.Instantiate(new GameObject("Behaviour[" + ID + "]"), poolManager.GetEmptyTransformHolder());

        if (myObject.GetComponent<UniqueID>() == null)
            Debug.LogError(myObject.name + " is missing unique ID!");

        poolManager.AddPoolableObjects(myObject, myObject.GetComponent<UniqueID>().GetID(), mySpellMaxCount);
    }

    public override void OnStart()
    {
        PoolManager poolManager = PoolManager.Instance;
        GameObject gameObject = poolManager.GetPooledObject(myObject.GetComponent<UniqueID>().GetID());
        if (!gameObject)
            return;

        gameObject.transform.parent = myEmptyTransformHolder.transform;

        if (mySpawnTransform.Value != null)
        {
            gameObject.transform.position = mySpawnTransform.Value.position;
            gameObject.transform.rotation = mySpawnTransform.Value.rotation;
            gameObject.transform.localScale = mySpawnTransform.Value.localScale;
        }
        else if (mySpawnPosition.Value != null)
        {
            gameObject.transform.position = mySpawnPosition.Value;
        }
        else
        {
            gameObject.transform.position = transform.position;
        }

        if (myLifetime > 0.0f)
            poolManager.AddTemporaryObject(gameObject, myLifetime);
    }

    public override TaskStatus OnUpdate()
    {
        return TaskStatus.Success;
    }
}
