using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldWall : MonoBehaviour
{
    public float myLifeTime;
    private float myCurrentLifeTime = 0.0f;

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Spell")
            Destroy(other.gameObject);        
    }

    void Update()
    {

        myCurrentLifeTime += Time.deltaTime;

        if (myCurrentLifeTime >= myLifeTime)
            Destroy(gameObject);
    }
}
