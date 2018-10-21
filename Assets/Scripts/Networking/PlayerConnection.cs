using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerConnection : NetworkBehaviour
{
    public GameObject myCharacterPrefab = null;

    [SyncVar(hook = "OnCharacterLoaded")]
    private GameObject myCharacter;

    private Friendlist myFriendList;

    public List<NetworkConnection> myFriendConnections;

    void Start()
    {
        myFriendList = GameObject.Find("Friendlist").GetComponent<Friendlist>();
        if (!isLocalPlayer)
            return;


        CharacterSelect characterSelect = GameObject.Find("CharacterSelect").GetComponent<CharacterSelect>();
        characterSelect.GetComponent<CanvasGroup>().alpha = 1;
        characterSelect.GetComponent<CanvasGroup>().interactable = true;
        characterSelect.SetPlayerCharacter(this);

        CmdSetFriendlist();
    }

    void Update()
    {
        if (!isLocalPlayer)
            return;
    }

    public void SpawnCharacterPrefab(int anIndex, string aName)
    {
        if (!isLocalPlayer)
            return;

        CmdSpawnCharacter(anIndex, aName);
    }

    [Command]
    private void CmdSpawnCharacter(int aCharacterID, string aName)
    {
        CharacterSelect characterSelect = GameObject.Find("CharacterSelect").GetComponent<CharacterSelect>();
        myCharacterPrefab = characterSelect.GetCharacterPrefab(aCharacterID);
        GameObject go = Instantiate(myCharacterPrefab, this.transform);

        NetworkServer.SpawnWithClientAuthority(go, connectionToClient);

        RpcSetChildParent(gameObject, go, aName);
    }

    [ClientRpc]
    private void RpcSetChildParent(GameObject aParent, GameObject aChild, string aName)
    {
        aChild.transform.parent = aParent.transform;
        aChild.GetComponent<PlayerCharacter>().Name = aName;
        myCharacter = aChild;
    }

    [Command]
    private void CmdAddPlayerToGameHandler(GameObject aPlayer)
    {
        GameObject.Find("GameHandler").GetComponent<GameHandler>().AddPlayer(aPlayer);
    }

    public void NotifyNewConnection()
    {
        if (!isLocalPlayer)
            return;

        GameHandler gameHandler = GameObject.Find("GameHandler").GetComponent<GameHandler>();
        SetFriendHud(gameHandler.GetPlayers());
    }

    public void SetFriendHud(PlayerList someConnectedPlayers)
    {
        if (!isLocalPlayer)
            return;

        myFriendList.Clear();
        foreach (Transform child in myFriendList.gameObject.transform)
        {
            CmdUnsubscribe(child.gameObject);
            Destroy(child.gameObject);
        }

        
        for (int index = 0; index < someConnectedPlayers.Count; index++)
        {
            if (someConnectedPlayers[index].netId == myCharacter.GetComponent<NetworkIdentity>().netId)            
                continue;

            GameObject player = ClientScene.FindLocalObject(someConnectedPlayers[index].netId);

            GameObject friendHud = myFriendList.myFriendlistHudPrefab;
            GameObject friend = Instantiate(friendHud, myFriendList.transform);

            PlayerCharacter playerCharacter = player.GetComponent<PlayerCharacter>();
            CharacterHUD hud = friend.GetComponent<CharacterHUD>();
            hud.SetName(playerCharacter.Name);
            hud.SetHealthBarFillAmount(player.GetComponent<Health>().GetHealthPercentage());
            myFriendList.AddHud(hud, someConnectedPlayers[index].netId, myCharacter.GetComponent<PlayerCharacter>().Name);

            CmdSubscribe(player);
        }
    }

    [Command]
    private void CmdSetFriendlist()
    {
        myFriendList = GameObject.Find("Friendlist").GetComponent<Friendlist>();
    }

    [Command]
    private void CmdSubscribe(GameObject aPlayer)
    {
        aPlayer.GetComponent<Health>().EventOnHealthChangeParty += AlertFriendListHealthChanged;
    }

    [Command]
    private void CmdUnsubscribe(GameObject aPlayer)
    {
        aPlayer.GetComponent<Health>().EventOnHealthChangeParty -= AlertFriendListHealthChanged;
    }

    private void OnCharacterLoaded(GameObject aGameObject)
    {
        if (!isLocalPlayer)
            return;

        myCharacter = aGameObject;
        CmdAddPlayerToGameHandler(aGameObject);
    }

    private void AlertFriendListHealthChanged(float aHealthPercentage, NetworkInstanceId anID)
    {
        RpcChangePartyHealth(aHealthPercentage, anID);
    }

    [ClientRpc]
    private void RpcChangePartyHealth(float aHealthPercentage, NetworkInstanceId anID)
    {
        if (anID == myCharacter.GetComponent<NetworkIdentity>().netId)
        {
            return;
        }
        
        myFriendList.ChangePartyHudHealth(aHealthPercentage, anID, myCharacter.GetComponent<PlayerCharacter>().Name);
    }
}
