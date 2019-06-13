using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetHandler : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> myPlayers = new List<GameObject>();

    [SerializeField]
    private List<GameObject> myEnemies = new List<GameObject>();

    public GameObject GetPlayer(int aIndex)
    {
        if (myPlayers.Count <= aIndex)
            return null;

        return myPlayers[aIndex];
    }

    public GameObject GetEnemy(int aIndex)
    {
        if (myEnemies.Count >= aIndex)
            return null;

        return myEnemies[aIndex];
    }
}
