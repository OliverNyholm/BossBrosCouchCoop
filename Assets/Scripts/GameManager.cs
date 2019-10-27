using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using InControl;

public class GameManager : MonoBehaviour
{
    [Header("The spawn points for player. Loops around when last reached")]
    [SerializeField]
    private List<Transform> mySpawnPoints = new List<Transform>();

    [Header("The player to be spawned if level started without character select")]
    public GameObject myPlayerPrefab;

    private void Awake()
    {
        PostMaster.Create();

        TargetHandler targetHandler = GetComponent<TargetHandler>();
        GameObject characterGameDataGO = GameObject.Find("GameData");
        if (characterGameDataGO == null)
        {
            Debug.Log("No CharacterGameData to find, default player created.");
            SpawnPlayer(targetHandler, myPlayerPrefab);
            return;
        }


        CharacterGameData characterGameData = characterGameDataGO.GetComponent<CharacterGameData>();
        List<PlayerSelectData> characters = characterGameData.GetPlayerData();
        for (int index = 0; index < characters.Count; index++)
        {
            SpawnPlayer(targetHandler, characters[index], index);
        }
    }

    private void Update()
    {
        PostMaster.Instance.DelegateMessages();
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void SpawnPlayer(TargetHandler aTargetHandler, PlayerSelectData aCharacter, int aIndex)
    {
        Vector3 spawnPoint = new Vector3(-1.5f + aIndex * 1.0f, 0.0f, -3.0f);
        if (mySpawnPoints.Count > 0)
            spawnPoint = mySpawnPoints[aIndex % mySpawnPoints.Count].position;

        GameObject playerGO = Instantiate(aCharacter.myClassData.myClass, spawnPoint, Quaternion.identity);
        playerGO.name = aCharacter.myName;
        playerGO.GetComponentInChildren<SkinnedMeshRenderer>().material = aCharacter.myColorScheme.myMaterial;

        Player player = playerGO.GetComponent<Player>();
        player.SetPlayerControls(aCharacter.myPlayerControls);
        player.myName = aCharacter.myName;
        player.PlayerIndex = aIndex + 1;
        player.myCharacterColor = aCharacter.myColorScheme.myColor;
        player.SetAvatar(aCharacter.myColorScheme.myAvatar);

        Vector3 rgb = new Vector3(player.myCharacterColor.r, player.myCharacterColor.g, player.myCharacterColor.b);
        PostMaster.Instance.PostMessage(new Message(MessageType.RegisterPlayer, playerGO.GetInstanceID(), rgb));

        aTargetHandler.AddPlayer(playerGO);
    }

    /// <summary>
    /// Spawns a player with the prefab assigned. Use when level started without character select
    /// </summary>
    /// <param name="aPrefab"></param>
    private void SpawnPlayer(TargetHandler aTargetHandler, GameObject aPrefab)
    {
        Vector3 spawnPoint = new Vector3(-1.5f + 0 * 1.0f, 0.0f, -3.0f);
        if (mySpawnPoints.Count > 0)
            spawnPoint = mySpawnPoints[0 % mySpawnPoints.Count].position;

        GameObject playerGO = Instantiate(aPrefab, spawnPoint, Quaternion.identity);
        playerGO.name = "DebugPlayer";

        PlayerControls keyboardListener = PlayerControls.CreateWithKeyboardBindings();

        playerGO.GetComponent<Stats>().myDamageMitigator = 0.0f;

        Player player = playerGO.GetComponent<Player>();
        player.SetPlayerControls(keyboardListener);
        player.myName = "DebugPlayer";
        player.PlayerIndex = 1;
        player.myCharacterColor = Color.red;

        Vector3 rgb = new Vector3(player.myCharacterColor.r, player.myCharacterColor.g, player.myCharacterColor.b);
        PostMaster.Instance.PostMessage(new Message(MessageType.RegisterPlayer, playerGO.GetInstanceID(), rgb));

        aTargetHandler.AddPlayer(playerGO);
    }
}