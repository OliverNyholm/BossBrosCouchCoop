using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetHandler : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> myPlayers = new List<GameObject>();

    [SerializeField]
    private List<GameObject> myNPCs = new List<GameObject>();

    private BossHudHandler myBossHudHandler;

    private void Start()
    {
        myBossHudHandler = FindObjectOfType<BossHudHandler>();
        if(myBossHudHandler)
        {
            for (int index = 0, count = myNPCs.Count; index < count; index++)
            {
                NPCComponent npcComponent = myNPCs[index].GetComponent<NPCComponent>();
                if (!npcComponent)
                    return;

                if (npcComponent.CreateHudFromTargetHandler())
                    myBossHudHandler.AddBossHud(myNPCs[index]);
            }
        }

        for (int index = 0; index < myNPCs.Count; index++)
        {
            for (int playerIndex = 0; playerIndex < myPlayers.Count; playerIndex++)
            {
                if (!myNPCs[index].GetComponent<NPCThreatComponent>())
                    continue;
                myNPCs[index].GetComponent<NPCThreatComponent>().AddPlayer(myPlayers[playerIndex]);
            }
        }
    }

    public List<GameObject> GetAllPlayers()
    {
        return myPlayers;
    }

    public void ClearAllPlayers()
    {
        myPlayers.Clear();
    }

    public void AddPlayer(GameObject aGameObject)
    {
        myPlayers.Add(aGameObject);
    }

    public GameObject GetPlayer(int aIndex)
    {
        if (aIndex < 0 || aIndex >= myPlayers.Count)
            return null;

        return myPlayers[aIndex];
    }

    public void AddEnemy(GameObject aGameObject, bool aShouldAddUI)
    {
        if (myNPCs.Contains(aGameObject))
            return;

        myNPCs.Add(aGameObject);
        for (int playerIndex = 0; playerIndex < myPlayers.Count; playerIndex++)
        {
            if (!aGameObject.GetComponent<NPCThreatComponent>())
                continue;
            aGameObject.GetComponent<NPCThreatComponent>().AddPlayer(myPlayers[playerIndex]);
        }

        if(aShouldAddUI)
            myBossHudHandler.AddBossHud(aGameObject);
    }

    public void RemoveEnemy(GameObject aGameObject)
    {
        myNPCs.Remove(aGameObject);
        myBossHudHandler.RemoveBossHud(aGameObject);
    }
    
    public void ClearAllEnemies()
    {
        myNPCs.Clear();
    }

    public GameObject GetEnemy(int aInstanceID)
    {
        for (int index = 0; index < myNPCs.Count; index++)
        {
            if (myNPCs[index].GetInstanceID() == aInstanceID)
                return myNPCs[index];
        }

        return null;
    }

    public List<GameObject> GetAllEnemies()
    {
        return myNPCs;
    }
}
