using UnityEngine;
using UnityEngine.Networking;

public class ChannelSpell : NetworkBehaviour {
    
    [SerializeField]
    protected Sprite mySpellIcon;

    protected float myTimerBeforeDestroy = -1.0f;

    [ClientRpc]
    public virtual void RpcSetToDestroy()
    {
        myTimerBeforeDestroy = 0.0f;
    }

    public void Update()
    {
        if (!isServer)
            return;

        if(myTimerBeforeDestroy >= 0.0f)
        {
            myTimerBeforeDestroy -= Time.deltaTime;
            if (myTimerBeforeDestroy <= 0.0f)
                NetworkServer.Destroy(gameObject);
        }
    }
}
