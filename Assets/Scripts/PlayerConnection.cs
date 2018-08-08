using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerConnection : NetworkBehaviour
{

    public GameObject myCharacterPrefab;
    public string myName;

    void Start()
    {
        if (!isLocalPlayer)
            return;

        myName = "Player" + Random.Range(0, 1000);
        CmdSpawnCharacter();
    }

    void Update()
    {
        if (!isLocalPlayer)
            return;
    }

    [Command]
    private void CmdSpawnCharacter()
    {
        GameObject go = Instantiate(myCharacterPrefab, this.transform);
        go.GetComponent<PlayerCharacter>().Name = myName;

        NetworkServer.SpawnWithClientAuthority(go, connectionToClient);
    }
}
