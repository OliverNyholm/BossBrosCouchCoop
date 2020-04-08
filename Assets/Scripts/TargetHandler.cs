using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetHandler : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> myPlayers = new List<GameObject>();

    private List<int> myPlayersTargetIndex = new List<int>();

    [SerializeField]
    private List<GameObject> myEnemies = new List<GameObject>();

    private BossHudHandler myBossHudHandler;

    private void Start()
    {
        myBossHudHandler = FindObjectsOfType<BossHudHandler>()[0];

        for (int index = 0, count = myEnemies.Count; index < count; index++)
        {
            myBossHudHandler.AddBossHud(myEnemies[index]);
        }

        myPlayersTargetIndex = new List<int>(myPlayers.Count);
        for (int index = 0; index < myPlayersTargetIndex.Capacity; index++)
        {
            myPlayersTargetIndex.Add(0);
        }

        for (int index = 0; index < myEnemies.Count; index++)
        {
            for (int playerIndex = 0; playerIndex < myPlayers.Count; playerIndex++)
            {
                if (!myEnemies[index].GetComponent<Enemy>())
                    continue;
                myEnemies[index].GetComponent<Enemy>().AddPlayer(myPlayers[playerIndex]);
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
        myEnemies.Add(aGameObject);
        for (int playerIndex = 0; playerIndex < myPlayers.Count; playerIndex++)
        {
            if (!aGameObject.GetComponent<Enemy>())
                continue;
            aGameObject.GetComponent<Enemy>().AddPlayer(myPlayers[playerIndex]);
        }

        if(aShouldAddUI)
            myBossHudHandler.AddBossHud(aGameObject);
    }

    public void RemoveEnemy(GameObject aGameObject)
    {
        myEnemies.Remove(aGameObject);
        myBossHudHandler.RemoveBossHud(aGameObject);
    }
    
    public void ClearAllEnemies()
    {
        myEnemies.Clear();
    }

    public GameObject GetEnemy(int aPlayerIndex)
    {
        int playerIndex = aPlayerIndex - 1;

        int aTargetIndex = ++myPlayersTargetIndex[playerIndex];
        if (aTargetIndex >= myEnemies.Count)
        {
            myPlayersTargetIndex[playerIndex] = 0;
            aTargetIndex = 0;
        }

        if (aTargetIndex < 0 || aTargetIndex >= myEnemies.Count)
            return null;

        return myEnemies[aTargetIndex];
    }

    public string GetEnemyName(int aInstanceID)
    {
        for (int index = 0; index < myEnemies.Count; index++)
        {
            if (myEnemies[index].GetInstanceID() == aInstanceID)
                return myEnemies[index].GetComponent<Character>().name;
        }

        return "Null";
    }

    public List<GameObject> GetAllEnemies()
    {
        return myEnemies;
    }
}
