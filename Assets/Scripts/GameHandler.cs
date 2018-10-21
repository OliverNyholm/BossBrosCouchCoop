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
    public PlayerList myConnectedPlayers = new PlayerList();

    private void Awake()
    {
        //DontDestroyOnLoad(gameObject);
        if (!isServer)
            return;
    }

    public void AddPlayer(GameObject aPlayer)
    {
        if (!isServer)
            return;

        PlayerInfo info;
        info.netId = aPlayer.GetComponent<NetworkIdentity>().netId;
        myConnectedPlayers.Add(info);
        RpcLoadFriendList(aPlayer);
    }

    public void RemovePlayer(GameObject aPlayer)
    {
        if (!isServer)
            return;
        Debug.Log("remvoe player");
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
