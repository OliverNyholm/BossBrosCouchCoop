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
    public GameObject myDebugPlayerPrefab;
    public ColorScheme myDebugColorSchemePrefab;
    private Player myDebugPlayer = null;

    private PlayerControls myControllerListener = null;

    private void Awake()
    {
        PostMaster.Create();

        BossHudHandler bossHudHandler = FindObjectsOfType<BossHudHandler>()[0];
        TargetHandler targetHandler = GetComponent<TargetHandler>();

        List<GameObject> bosses = targetHandler.GetAllEnemies();
        for (int index = 0, count = bosses.Count; index < count; index++)
        {
            bossHudHandler.HandoutBossHud(bosses[index], count, index);
        }

        GameObject characterGameDataGO = GameObject.Find("GameData");
        if (characterGameDataGO == null)
        {
            Debug.Log("No CharacterGameData to find, default player created.");
            SpawnPlayer(targetHandler, myDebugPlayerPrefab, 1);

            myControllerListener = PlayerControls.CreateWithJoystickBindings();
            return;
        }


        CharacterGameData characterGameData = characterGameDataGO.GetComponent<CharacterGameData>();
        List<PlayerSelectData> characters = characterGameData.GetPlayerData();
        for (int index = 0; index < characters.Count; index++)
        {
            SpawnPlayer(targetHandler, characters[index], index);
        }
    }

    private void OnDisable()
    {
        if(myControllerListener != null)
            myControllerListener.Destroy();
    }

    private void Update()
    {
        PostMaster.Instance.DelegateMessages();

        if (myControllerListener != null)
        {
            if(ControllerInputDetected())
            {
                PlayerControls playerControls = PlayerControls.CreateWithJoystickBindings();
                playerControls.Device = InputManager.ActiveDevice;
                myDebugPlayer.SetPlayerControls(playerControls);

                myControllerListener = null;
            }
        }

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
    private void SpawnPlayer(TargetHandler aTargetHandler, GameObject aPrefab, int aPlayerIndex)
    {
        Vector3 spawnPoint = new Vector3(-1.5f + 0 * 1.0f, 0.0f, -3.0f);
        if (mySpawnPoints.Count > 0)
            spawnPoint = mySpawnPoints[0 % mySpawnPoints.Count].position;

        GameObject playerGO = Instantiate(aPrefab, spawnPoint, Quaternion.identity);
        playerGO.name = "DebugPlayer";

        PlayerControls keyboardListener = PlayerControls.CreateWithKeyboardBindings();

        playerGO.GetComponent<Stats>().myDamageMitigator = 0.0f;

        myDebugPlayer = playerGO.GetComponent<Player>();
        myDebugPlayer.SetPlayerControls(keyboardListener);
        myDebugPlayer.myName = "DebugPlayer";
        myDebugPlayer.PlayerIndex = aPlayerIndex;
        myDebugPlayer.myCharacterColor = myDebugColorSchemePrefab.myColor;
        myDebugPlayer.SetAvatar(myDebugColorSchemePrefab.myAvatar);

        Vector3 rgb = new Vector3(myDebugPlayer.myCharacterColor.r, myDebugPlayer.myCharacterColor.g, myDebugPlayer.myCharacterColor.b);
        PostMaster.Instance.PostMessage(new Message(MessageType.RegisterPlayer, playerGO.GetInstanceID(), rgb));

        aTargetHandler.AddPlayer(playerGO);
    }

    private bool ControllerInputDetected()
    {
        if (myControllerListener.Action1.WasPressed || myControllerListener.Action2.WasPressed || myControllerListener.Action3.WasPressed || myControllerListener.Action4.WasPressed)
            return true;

        if (myControllerListener.Movement.X != 0 || myControllerListener.Movement.Y != 0)
            return true;

        return false;
    }

    public void AddExtraDebugPlayer()
    {
        TargetHandler targetHandler = GetComponent<TargetHandler>();
        if (targetHandler.GetAllPlayers().Count == 4)
            return;

        Debug.Log("Adding another debug player.");
        SpawnPlayer(targetHandler, myDebugPlayerPrefab, targetHandler.GetAllPlayers().Count + 1);
    }
}