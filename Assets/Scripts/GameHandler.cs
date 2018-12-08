using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public struct PlayerInfo
{
    public NetworkInstanceId netId;
};

public class PlayerList : SyncListStruct<PlayerInfo> { }

public class GameHandler : NetworkBehaviour
{
    public AIPostMaster myPostmaster;
    public PlayerList myConnectedPlayers = new PlayerList();

    public Enemy myEnemy;

    private void Awake()
    {
        //DontDestroyOnLoad(gameObject);
        if (!isServer)
            return;

        AIPostMaster.Create();
    }

    private void Update()
    {
        AIPostMaster.Instance.DelegateMessages();
    }

    public void AddPlayer(GameObject aPlayer)
    {
        if (!isServer)
            return;

        PlayerInfo info;
        info.netId = aPlayer.GetComponent<NetworkIdentity>().netId;
        myConnectedPlayers.Add(info);
        RpcLoadFriendList(aPlayer);

        myEnemy.AddPlayerCharacter(aPlayer);
    }

    public void RemovePlayer(GameObject aPlayer)
    {
        if (!isServer)
            return;

        myEnemy.RemovePlayerCharacter(aPlayer);
        NetworkInstanceId playerID = aPlayer.GetComponent<NetworkIdentity>().netId;
        foreach (var player in myConnectedPlayers)
        {
            if (playerID == player.netId)
            {
                myConnectedPlayers.Remove(player);
                break;
            }
        }
    }

    public PlayerList GetPlayers()
    {
        return myConnectedPlayers;
    }

    [ClientRpc]
    public void RpcLoadFriendList(GameObject aPlayer)
    {
        PlayerConnection[] connections = FindObjectsOfType<PlayerConnection>();

        foreach (PlayerConnection connection in connections)
        {
            connection.NotifyNewConnection();
        }
    }
}
