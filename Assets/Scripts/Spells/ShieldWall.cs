using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ShieldWall : NetworkBehaviour
{
    public float myLifeTime;
    private float myCurrentLifeTime = 0.0f;

    void OnTriggerEnter(Collider other)
    {
        if (!isServer)
            return;
        Debug.Log("TriggerEnter!");
        if(other.tag == "Spell")
            NetworkServer.Destroy(other.gameObject);
    }

    void Update()
    {
        if (!isServer)
            return;

        myCurrentLifeTime += Time.deltaTime;

        if (myCurrentLifeTime >= myLifeTime)
            NetworkServer.Destroy(gameObject);
    }
}
