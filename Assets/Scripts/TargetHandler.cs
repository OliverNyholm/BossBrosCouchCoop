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

    private void Awake()
    {
        CharacterGameData characterGameData = GameObject.Find("CharacterGameData").GetComponent<CharacterGameData>();
        List<CharacterSelectData> characters = characterGameData.GetPlayerData();
        for (int index = 0; index < characters.Count; index++)
        {
            GameObject player = Instantiate(characters[index].myClassData.myClass, new Vector3(-1.5f + index * 1.0f, 0.0f, -3.0f), Quaternion.identity);
            player.GetComponentInChildren<SkinnedMeshRenderer>().material.mainTexture = characters[index].myColorScheme.myTexture;
            player.GetComponent<Player>().SetInputDevice(characters[index].myInputDevice);

            myPlayers.Add(player);
        }
    }

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
        myPlayers[myPlayers.Count - 1].GetComponent<Player>().enabled = false;
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
