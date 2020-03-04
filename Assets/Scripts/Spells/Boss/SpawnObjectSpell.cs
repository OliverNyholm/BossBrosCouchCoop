using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnObjectSpell : Spell
{

    [Header("Spawn Details")]
    [SerializeField]
    private GameObject myObjectToSpawn = null;

    [SerializeField]
    private float mySpawnObjectLifetime = 1.0f;

    protected override void DealSpellEffect()
    {
        PoolManager poolManager = PoolManager.Instance;
        GameObject spawnObject = poolManager.GetPooledObject(myObjectToSpawn.GetComponent<UniqueID>().GetID());

        spawnObject.transform.localPosition = transform.position;
        spawnObject.transform.localRotation = Quaternion.identity;

        poolManager.AddTemporaryObject(spawnObject, mySpawnObjectLifetime);
    }

    public override void CreatePooledObjects(PoolManager aPoolManager, int aSpellMaxCount)
    {
        base.CreatePooledObjects(aPoolManager, aSpellMaxCount);

        aPoolManager.AddPoolableObjects(myObjectToSpawn, myObjectToSpawn.GetComponent<UniqueID>().GetID(), aSpellMaxCount);
    }
}
