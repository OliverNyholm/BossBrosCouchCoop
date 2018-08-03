using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerConnection : NetworkBehaviour {

    public GameObject myCharacterPrefab;

	void Start () {

        GameObject go = Instantiate(myCharacterPrefab, this.transform);

        NetworkServer.SpawnWithClientAuthority(go, connectionToClient);
	}
	
	void Update () {
        if (!isLocalPlayer)
            return;
	}
}
