using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using InControl;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private PauseMenu myPauseMenu = null;

    [Header("The spawn points for player. Loops around when last reached")]
    [SerializeField]
    private List<Transform> mySpawnPoints = new List<Transform>();

    [Header("The player to be spawned if level started without character select")]
    public PlayerSelectData myDebugPlayerData;
    private Player myDebugPlayer = null;

    [SerializeField]
    private List<ColorScheme> myDebugColorSchemes = new List<ColorScheme>(4);

    private PlayerControls myControllerListener = null;

    private List<PlayerSelectData> myPlayerSelectData = new List<PlayerSelectData>(4);

    private void Awake()
    {
        PostMaster.Create();

        myPauseMenu = FindObjectOfType<PauseMenu>();
        myPauseMenu.gameObject.SetActive(false);

        TargetHandler targetHandler = GetComponent<TargetHandler>();
        GameObject characterGameDataGO = GameObject.Find("GameData");
        if (characterGameDataGO == null)
        {
            Debug.Log("No CharacterGameData to find, default player created.");
            myPlayerSelectData.Add(myDebugPlayerData);
            SpawnPlayer(targetHandler, 0);

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

    public void OpenPauseMenu(PlayerControls aPlayerControls)
    {
        myPauseMenu.Pause(this, aPlayerControls);
        myPauseMenu.gameObject.SetActive(true);
    }

    private void SpawnPlayer(TargetHandler aTargetHandler, PlayerSelectData aCharacter, int aIndex)
    {
        Vector3 spawnPoint = new Vector3(-1.5f + aIndex * 1.0f, 0.0f, -3.0f);
        if (mySpawnPoints.Count > 0)
            spawnPoint = mySpawnPoints[aIndex % mySpawnPoints.Count].position;

        if (UtilityFunctions.FindGroundFromLocation(spawnPoint, out Vector3 hitLocation, out MovablePlatform movablePlatform))
            spawnPoint = hitLocation;

        GameObject playerGO = Instantiate(aCharacter.myClassData.myGnome, spawnPoint, Quaternion.identity);
        playerGO.name = aCharacter.myName;
        playerGO.GetComponentInChildren<GnomeAppearance>().SetColorMaterial(aCharacter.myColorScheme.myMaterial);

        aCharacter.myClassData.AddRequiredComponents(playerGO);

        Player player = playerGO.GetComponent<Player>();
        player.SetClassData(aCharacter.myClassData);
        player.SetPlayerControls(aCharacter.myPlayerControls);
        player.PlayerIndex = aIndex + 1;

        PlayerUIComponent uiComponent = playerGO.GetComponent<PlayerUIComponent>();
        uiComponent.myName = aCharacter.myName;
        uiComponent.myCharacterColor = aCharacter.myColorScheme.myColor;
        uiComponent.myAvatarSprite = aCharacter.myColorScheme.myAvatar;

        PostMaster.Instance.PostMessage(new Message(MessageCategory.RegisterPlayer, playerGO.GetInstanceID(), ColorToRGBVector(uiComponent.myCharacterColor)));

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
            spawnPoint = mySpawnPoints[aPlayerIndex % mySpawnPoints.Count].position;

        if (UtilityFunctions.FindGroundFromLocation(spawnPoint, out Vector3 hitLocation, out MovablePlatform movablePlatform))
            spawnPoint = hitLocation;

        GameObject playerGO = Instantiate(myDebugPlayerData.myClassData.myGnome, spawnPoint, Quaternion.identity);
        playerGO.name = myDebugPlayerData.myName;

        PlayerControls keyboardListener = PlayerControls.CreateWithKeyboardBindings();

        //playerGO.GetComponent<Stats>().myDamageMitigator = 0.0f;
        playerGO.GetComponent<Health>().MaxHealth = 50000;
        playerGO.GetComponent<Health>().SetHealthPercentage(1.0f);

        myDebugPlayerData.myClassData.AddRequiredComponents(playerGO);

        myDebugPlayer = playerGO.GetComponent<Player>();
        myDebugPlayer.SetClassData(myDebugPlayerData.myClassData);
        myDebugPlayer.SetPlayerControls(keyboardListener);
        myDebugPlayer.PlayerIndex = aPlayerIndex + 1;

        PlayerUIComponent uiComponent = playerGO.GetComponent<PlayerUIComponent>();
        uiComponent.myName = myDebugPlayerData.myName;
        uiComponent.myCharacterColor = myDebugColorSchemes[aPlayerIndex].myColor;
        uiComponent.myAvatarSprite = myDebugColorSchemes[aPlayerIndex].myAvatar;

        playerGO.GetComponentInChildren<SkinnedMeshRenderer>().material = myDebugColorSchemes[aPlayerIndex].myMaterial;

        myPlayerSelectData[myPlayerSelectData.Count - 1].myPlayerControls = keyboardListener;

        PostMaster.Instance.PostMessage(new Message(MessageCategory.RegisterPlayer, playerGO.GetInstanceID(), ColorToRGBVector(uiComponent.myCharacterColor)));

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
        SpawnPlayer(targetHandler, targetHandler.GetAllPlayers().Count);

        GameObject newPlayer = targetHandler.GetPlayer(targetHandler.GetAllPlayers().Count - 1);
        foreach (GameObject npc in targetHandler.GetAllEnemies())
        {
            if (!npc.GetComponent<NPCThreatComponent>())
                continue;
            npc.GetComponent<NPCThreatComponent>().AddPlayer(newPlayer);
        }

        DynamicCamera dynamicCamera = FindObjectOfType<DynamicCamera>();
        if (dynamicCamera)
            dynamicCamera.myPlayerTransforms.Add(newPlayer.transform);
    }

    private Vector3 ColorToRGBVector(Color aColor)
    {
        return new Vector3(aColor.r, aColor.g, aColor.b);
    }
}