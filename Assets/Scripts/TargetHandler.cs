using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetHandler : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> myPlayers = new List<GameObject>();

    private List<int> myPlayersTargetIndex = new List<int>();

    [SerializeField]
    private List<GameObject> myNPCs = new List<GameObject>();

    private BossHudHandler myBossHudHandler;

    private void Start()
    {
        myBossHudHandler = FindObjectsOfType<BossHudHandler>()[0];

        for (int index = 0, count = myNPCs.Count; index < count; index++)
        {
            myBossHudHandler.AddBossHud(myNPCs[index]);
        }

        myPlayersTargetIndex = new List<int>(myPlayers.Count);
        for (int index = 0; index < myPlayersTargetIndex.Capacity; index++)
        {
            myPlayersTargetIndex.Add(0);
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
        myPlayersTargetIndex.Add(0);
    }

    public GameObject GetPlayer(int aIndex)
    {
        if (aIndex < 0 || aIndex >= myPlayers.Count)
            return null;

        return myPlayers[aIndex];
    }

    public void AddEnemy(GameObject aGameObject, bool aShouldAddUI)
    {
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

    public GameObject GetEnemy(int aPlayerIndex)
    {
        int playerIndex = aPlayerIndex - 1;

        int aTargetIndex = ++myPlayersTargetIndex[playerIndex];
        if (aTargetIndex >= myNPCs.Count)
        {
            myPlayersTargetIndex[playerIndex] = 0;
            aTargetIndex = 0;
        }

        if (aTargetIndex < 0 || aTargetIndex >= myNPCs.Count)
            return null;

        return myNPCs[aTargetIndex];
    }

    public string GetEnemyName(int aInstanceID)
    {
        for (int index = 0; index < myNPCs.Count; index++)
        {
            if (myNPCs[index].GetInstanceID() == aInstanceID)
                return myNPCs[index].GetComponent<Character>().name;
        }

        return "Null";
    }

    public List<GameObject> GetAllEnemies()
    {
        return myNPCs;
    }
}
