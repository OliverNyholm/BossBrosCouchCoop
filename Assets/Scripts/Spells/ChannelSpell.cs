using UnityEngine;
using UnityEngine.Networking;

public class ChannelSpell : NetworkBehaviour {
    
    [SerializeField]
    protected Sprite mySpellIcon;

    [ClientRpc]
    public void RpcSetToDestroy()
    {
        NetworkServer.Destroy(gameObject);
    }
}
