using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    public string myName;
    public float myDamage;

    public GameObject[] mySpells;

    private Vector3 myStartPosition;
    private Vector3 myEndPosition;
    private Vector3 myTargetPosition;

    public float mySpeed;

    // Use this for initialization
    void Start()
    {
        myStartPosition = transform.position;
        myEndPosition = myStartPosition + new Vector3(10.0f, 0.0f, 0.0f);
        myTargetPosition = myEndPosition;
    }

    // Update is called once per frame
    void Update()
    {
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
