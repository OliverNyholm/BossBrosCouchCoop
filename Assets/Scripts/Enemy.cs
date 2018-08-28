using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Enemy : NetworkBehaviour
{

    public string myName;
    public float myDamage;

    public GameObject[] mySpells;

    private Vector3 myStartPosition;
    private Vector3 myEndPosition;
    private Vector3 myTargetPosition;

    public float mySpeed;

    private Stats myStats;

    // Use this for initialization
    void Start()
    {
        myStartPosition = transform.position;
        myEndPosition = myStartPosition + new Vector3(10.0f, 0.0f, 0.0f);
        myTargetPosition = myEndPosition;

        myStats = GetComponent<Stats>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isServer)
            return;

        Vector3 direction = myTargetPosition - transform.position;
        transform.position += direction.normalized * mySpeed * Time.deltaTime;

        if (Vector3.Distance(transform.position, myTargetPosition) <= 0.1f)
        {
            if (myTargetPosition == myStartPosition)
                myTargetPosition = myEndPosition;
            else
                myTargetPosition = myStartPosition;
        }
    }
}
