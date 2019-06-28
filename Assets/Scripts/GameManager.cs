using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("The spawn points for player. Loops around when last reached")]
    [SerializeField]
    private List<Transform> mySpawnPoints = new List<Transform>();

    private void Awake()
    {
        PostMaster.Create();

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
            Vector3 spawnPoint = new Vector3(-1.5f + index * 1.0f, 0.0f, -3.0f);
            if (mySpawnPoints.Count > 0)
                spawnPoint = mySpawnPoints[index % mySpawnPoints.Count].position;

            GameObject playerGO = Instantiate(characters[index].myClassData.myClass, spawnPoint, Quaternion.identity);
            playerGO.name = characters[index].myName;
            playerGO.GetComponentInChildren<SkinnedMeshRenderer>().material.mainTexture = characters[index].myColorScheme.myTexture;

            Player player = playerGO.GetComponent<Player>();
            player.SetPlayerControls(characters[index].myPlayerControls);
            player.myName = characters[index].myName;
            player.PlayerIndex = index + 1;
            player.myCharacterColor = characters[index].myColorScheme.myColor;
            player.SetAvatar(characters[index].myColorScheme.myAvatar);

            Vector3 rgb = new Vector3(player.myCharacterColor.r, player.myCharacterColor.g, player.myCharacterColor.b);
            PostMaster.Instance.PostMessage(new Message(MessageType.RegisterPlayer, playerGO.GetInstanceID(), rgb));

            targetHandler.AddPlayer(playerGO);
        }
    }

    private void Update()
    {
        PostMaster.Instance.DelegateMessages();

        if (Input.GetButtonDown("Restart"))
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}