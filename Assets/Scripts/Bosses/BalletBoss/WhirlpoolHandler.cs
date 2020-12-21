using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhirlpoolHandler : MonoBehaviour
{
    [SerializeField]
    private GameObject myBoss = null;
    private int myIgnoreID;

    [SerializeField]
    private GameObject myWhirlPoolPrefab = null;
    private List<Whirlpool> myWhirlPools = new List<Whirlpool>(24);
    private uint myWhirlPoolUniqueID;

    private TargetHandler myTargetHandler;
    private Subscriber mySubscriber;

    private List<int> myAvailablePoolIndexes = new List<int>(24);

    private void Awake()
    {
        myTargetHandler = FindObjectOfType<TargetHandler>();

        myIgnoreID = myBoss.GetInstanceID();
        mySubscriber = new Subscriber();
        mySubscriber.EventOnReceivedMessage += ReceiveMessage;

        myWhirlPoolUniqueID = myWhirlPoolPrefab.GetComponent<UniqueID>().GetID();
        PoolManager.Instance.AddPoolableObjects(myWhirlPoolPrefab, myWhirlPoolUniqueID, 30);
    }

    private void Start()
    {
        PostMaster.Instance.RegisterSubscriber(ref mySubscriber, MessageCategory.EnemyDied);
    }

    private void OnDestroy()
    {
        PostMaster.Instance.UnregisterSubscriber(ref mySubscriber, MessageCategory.EnemyDied);
    }

    private void ReceiveMessage(Message aMessage)
    {
        if (aMessage.Data.myInt == myIgnoreID)
            return;

        GameObject npc = myTargetHandler.GetEnemy(aMessage.Data.myInt);
        if(npc)
        {
            GameObject spawnedPool = PoolManager.Instance.GetPooledObject(myWhirlPoolUniqueID);
            if (spawnedPool)
            {
                spawnedPool.transform.position = npc.transform.position + Vector3.up * 2.5f;
                Whirlpool whirlPool = spawnedPool.GetComponent<Whirlpool>();
                whirlPool.SetHandler(this);
                myWhirlPools.Add(whirlPool);
            }
        }
    }

    public Whirlpool OnPlayerEnterWhirlpool(Whirlpool aWhirlPool)
    {
        if(myWhirlPools.Count < 2)
            return null;

        myAvailablePoolIndexes.Clear();
        for (int index = 0; index < myWhirlPools.Count; index++)
        {
            Whirlpool whirlPool = myWhirlPools[index];
            if (whirlPool == aWhirlPool)
                continue;

            if (whirlPool.IsAvailable())
                myAvailablePoolIndexes.Add(index);
        }

        if (myAvailablePoolIndexes.Count == 0)
            return null;

        int randomIndex = myAvailablePoolIndexes[Random.Range(0, myAvailablePoolIndexes.Count)];
        return myWhirlPools[randomIndex];
    }

    public void RemovePool(Whirlpool aWhirlPool)
    {
        myWhirlPools.Remove(aWhirlPool);
        aWhirlPool.ReturnToPool();
    }
}
