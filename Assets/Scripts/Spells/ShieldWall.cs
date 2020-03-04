using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldWall : MonoBehaviour
{
    public float myLifeTime;
    private float myCurrentLifeTime = 0.0f;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Spell")
            other.GetComponent<PoolableObject>().ReturnToPool();     
    }

    void Update()
    {

        myCurrentLifeTime += Time.deltaTime;

        if (myCurrentLifeTime >= myLifeTime)
            PoolManager.Instance.ReturnObject(gameObject, GetComponent<UniqueID>().GetID());
    }
}
