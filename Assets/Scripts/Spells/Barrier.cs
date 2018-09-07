using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Barrier : NetworkBehaviour
{
    [SerializeField]
    private Buff myBuff;

    private GameObject myParent;

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

        if(!myParent.GetComponent<PlayerCharacter>().IsCasting())
        {
            NetworkServer.Destroy(gameObject);
        }
    }

    public void SetParent(GameObject aGameObject)
    {
        myParent = aGameObject;
    }
}
