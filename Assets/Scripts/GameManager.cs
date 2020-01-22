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
    public PlayerSelectData myDebugPlayerData;
    private Player myDebugPlayer = null;

    private PlayerControls myControllerListener = null;

    private List<PlayerSelectData> myPlayerSelectData = new List<PlayerSelectData>(4);
    private struct ChangeClassData
    {
        public Vector3 myPosition;
        public float myHealthPercentage;
    };
    private List<ChangeClassData> myChangeClassData = new List<ChangeClassData>(4);

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
            myPlayerSelectData.Add(myDebugPlayerData);
            SpawnPlayer(targetHandler, 1);

            myControllerListener = PlayerControls.CreateWithJoystickBindings();
            return;
        }


        CharacterGameData characterGameData = characterGameDataGO.GetComponent<CharacterGameData>();
        myPlayerSelectData = new List<PlayerSelectData>(characterGameData.GetPlayerData());
        for (int index = 0; index < myPlayerSelectData.Count; index++)
        {
            SpawnPlayer(targetHandler, myPlayerSelectData[index], index);
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

                myPlayerSelectData[myPlayerSelectData.Count - 1].myPlayerControls = playerControls;
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
        PostMaster.Instance.PostMessage(new Message(MessageCategory.RegisterPlayer, playerGO.GetInstanceID(), rgb));

        aTargetHandler.AddPlayer(playerGO);
    }

    /// <summary>
    /// Spawns a player with the prefab assigned. Use when level started without character select
    /// </summary>
    /// <param name="aPrefab"></param>
    private void SpawnPlayer(TargetHandler aTargetHandler, int aPlayerIndex)
    {
        Vector3 spawnPoint = new Vector3(-1.5f + 0 * 1.0f, 0.0f, -3.0f);
        if (mySpawnPoints.Count > 0)
            spawnPoint = mySpawnPoints[0 % mySpawnPoints.Count].position;

        GameObject playerGO = Instantiate(myDebugPlayerData.myClassData.myClass, spawnPoint, Quaternion.identity);
        playerGO.name = myDebugPlayerData.myName;

        PlayerControls keyboardListener = PlayerControls.CreateWithKeyboardBindings();

        playerGO.GetComponent<Stats>().myDamageMitigator = 0.0f;

        myDebugPlayer = playerGO.GetComponent<Player>();
        myDebugPlayer.SetPlayerControls(keyboardListener);
        myDebugPlayer.myName = myDebugPlayerData.myName;
        myDebugPlayer.PlayerIndex = aPlayerIndex;
        myDebugPlayer.myCharacterColor = myDebugPlayerData.myColorScheme.myColor;
        myDebugPlayer.SetAvatar(myDebugPlayerData.myColorScheme.myAvatar);

        myPlayerSelectData[myPlayerSelectData.Count - 1].myPlayerControls = keyboardListener;

        Vector3 rgb = new Vector3(myDebugPlayer.myCharacterColor.r, myDebugPlayer.myCharacterColor.g, myDebugPlayer.myCharacterColor.b);
        PostMaster.Instance.PostMessage(new Message(MessageCategory.RegisterPlayer, playerGO.GetInstanceID(), rgb));

        aTargetHandler.AddPlayer(playerGO);
    }

    public void ChangeClassInGame(GameObject aNewClass)
    {
        TargetHandler targetHandler = GetComponent<TargetHandler>();

        myChangeClassData.Clear();

        bool hadDebugPlayer = myDebugPlayer != null;

        foreach (GameObject player in targetHandler.GetAllPlayers())
        {
            ChangeClassData changeClassData;
            changeClassData.myHealthPercentage = player.GetComponent<Health>().GetHealthPercentage();
            changeClassData.myPosition = player.transform.position;
            myChangeClassData.Add(changeClassData);

            Destroy(player);
        }
        targetHandler.ClearAllPlayers();

        for (int index = 0; index < myPlayerSelectData.Count; index++)
        {
            ChangePlayerClass(targetHandler, aNewClass, index);
        }

        if (hadDebugPlayer)
            myDebugPlayer = targetHandler.GetPlayer(myPlayerSelectData.Count - 1).GetComponent<Player>();
    }

    private void ChangePlayerClass(TargetHandler aTargetHandler, GameObject aNewClass, int anIndex)
    {
        PlayerSelectData originalPlayerSelectData = myPlayerSelectData[anIndex];

        GameObject playerGO = Instantiate(aNewClass, myChangeClassData[anIndex].myPosition, Quaternion.identity);
        playerGO.name = originalPlayerSelectData.myName;
        playerGO.GetComponentInChildren<SkinnedMeshRenderer>().material = originalPlayerSelectData.myColorScheme.myMaterial;

        Player player = playerGO.GetComponent<Player>();
        player.SetPlayerControls(originalPlayerSelectData.myPlayerControls);
        player.myName = originalPlayerSelectData.myName;
        player.PlayerIndex = anIndex + 1;
        player.myCharacterColor = originalPlayerSelectData.myColorScheme.myColor;
        player.SetAvatar(originalPlayerSelectData.myColorScheme.myAvatar);

        playerGO.GetComponent<Health>().SetHealthPercentage(myChangeClassData[anIndex].myHealthPercentage);

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
        myPlayerSelectData.Add(myDebugPlayerData);
        SpawnPlayer(targetHandler, targetHandler.GetAllPlayers().Count + 1);
    }
}