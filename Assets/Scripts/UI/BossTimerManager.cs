using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTimerManager : MonoBehaviour
{
    [Header("The timer prefab")]
    [SerializeField]
    private GameObject myTimerPrefab = null;

    private List<GameObject> myBossTimers = new List<GameObject>();

    private PoolManager myPoolManager;
    private uint myTimerPrefabID;

    private void Awake()
    {
        myTimerPrefabID = myTimerPrefab.GetComponent<UniqueID>().GetID();

        myPoolManager = PoolManager.Instance;
        myPoolManager.AddPoolableObjects(myTimerPrefab, myTimerPrefabID, 6);
    }

    public int AddBossTimer(string aName, float aDuration, Sprite aSprite, Color aColor)
    {
        GameObject instance = myPoolManager.GetPooledObject(myTimerPrefabID);
        instance.transform.SetParent(transform, false);

        BossTimer bossTimer = instance.GetComponent<BossTimer>();
        bossTimer.SetData(aName, aDuration, aSprite, aColor);

        myBossTimers.Add(instance);

        return instance.GetInstanceID();
    }

    public void RemoveBossTimer(int aInstanceID)
    {
        for (int index = 0; index < myBossTimers.Count; index++)
        {
            if(myBossTimers[index].GetInstanceID() == aInstanceID)
            {
                myPoolManager.ReturnObject(myBossTimers[index], myTimerPrefabID);
                myBossTimers.RemoveAt(index);
                break;
            }
        }
    }
}
