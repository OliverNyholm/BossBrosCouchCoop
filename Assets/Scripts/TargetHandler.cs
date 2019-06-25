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

    private void Start()
    {
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
}
