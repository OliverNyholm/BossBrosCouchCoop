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
    private List<GameObject> myWhirlPools = new List<GameObject>(24);
    private uint myWhirlPoolUniqueID;

    private TargetHandler myTargetHandler;
    private Subscriber mySubscriber;

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
                spawnedPool.transform.position = npc.transform.position;
                myWhirlPools.Add(spawnedPool);
            }
        }
    }
}
