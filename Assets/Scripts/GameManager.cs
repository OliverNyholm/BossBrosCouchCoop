using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private void Awake()
    {
        AIPostMaster.Create();

        TargetHandler targetHandler = GetComponent<TargetHandler>();
        GameObject characterGameDataGO = GameObject.Find("CharacterGameData");
        if (characterGameDataGO == null)
        {
            Debug.Log("No CharacterGameData to find, no character created");
            return;
        }


        CharacterGameData characterGameData = characterGameDataGO.GetComponent<CharacterGameData>();
        List<CharacterSelectData> characters = characterGameData.GetPlayerData();
        for (int index = 0; index < characters.Count; index++)
        {
            GameObject playerGO = Instantiate(characters[index].myClassData.myClass, new Vector3(-1.5f + index * 1.0f, 0.0f, -3.0f), Quaternion.identity);
            playerGO.name = characters[index].myName;
            playerGO.GetComponentInChildren<SkinnedMeshRenderer>().material.mainTexture = characters[index].myColorScheme.myTexture;

            Player player = playerGO.GetComponent<Player>();
            player.SetInputDevice(characters[index].myInputDevice);
            player.PlayerIndex = index + 1;
            player.PlayerColor = characters[index].myColorScheme.myColor;
            player.SetAvatar(characters[index].myColorScheme.myAvatar);

            targetHandler.AddPlayer(playerGO);
        }
    }

    private void Update()
    {
        AIPostMaster.Instance.DelegateMessages();

        if (Input.GetButtonDown("Restart"))
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}