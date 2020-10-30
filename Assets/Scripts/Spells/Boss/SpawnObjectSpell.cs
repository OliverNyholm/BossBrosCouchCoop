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

    [SerializeField]
    private bool myShouldSnapObjectToGround = false;

    [SerializeField]
    private Vector3 mySpawnOffset = Vector3.zero;

    protected override void DealSpellEffect()
    {
        PoolManager poolManager = PoolManager.Instance;
        GameObject spawnObject = poolManager.GetPooledObject(myObjectToSpawn.GetComponent<UniqueID>().GetID());

        if (!spawnObject)
            return;

        Vector3 spawnPosition;
        if (myIsOnlySelfCast)
            spawnPosition = myParent.transform.position + myParent.transform.rotation * mySpawnOffset;
        else
            spawnPosition = transform.position + transform.rotation * mySpawnOffset;

        spawnObject.transform.localPosition = spawnPosition;
        spawnObject.transform.localRotation = Quaternion.identity;

        if(myShouldSnapObjectToGround)
        {
            float distance = 5.0f;
            Ray ray = new Ray(spawnObject.transform.position + Vector3.up, Vector3.down);
            LayerMask layerMask = LayerMask.GetMask("Terrain");

            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, distance, layerMask))
            {
                spawnObject.transform.localPosition = hitInfo.point;
                MovablePlatformObject movableObject = spawnObject.GetComponent<MovablePlatformObject>();
                if (movableObject)
                {
                    MovablePlatform movablePlatform = hitInfo.collider.gameObject.GetComponent<MovablePlatform>();
                    if (movablePlatform)
                        movableObject.AddSelfToPlatform(movablePlatform);
                }
            }
        }

        poolManager.AddTemporaryObject(spawnObject, mySpawnObjectLifetime);
    }

    public override void CreatePooledObjects(PoolManager aPoolManager, int aSpellMaxCount)
    {
        base.CreatePooledObjects(aPoolManager, aSpellMaxCount);

        aPoolManager.AddPoolableObjects(myObjectToSpawn, myObjectToSpawn.GetComponent<UniqueID>().GetID(), aSpellMaxCount);
    }
}
