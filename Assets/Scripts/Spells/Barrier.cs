using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Barrier : NetworkBehaviour
{
    [SerializeField]
    private Buff myBuff;

    void OnTriggerEnter(Collider other)
    {
        if (!isServer)
            return;

        if (other.tag == "Player")
        {
            //Add Buff
        }
    }

    void Update()
    {
        if (!isServer)
            return;

        if (!transform.parent.GetComponent<PlayerCharacter>().IsCasting())
        {
            NetworkServer.Destroy(gameObject);
        }
    }
}
