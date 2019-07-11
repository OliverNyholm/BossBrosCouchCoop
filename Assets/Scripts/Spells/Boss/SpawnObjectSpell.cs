using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnObjectSpell : Spell
{

    [Header("Spawn Details")]
    [SerializeField]
    private GameObject myObjectToSpawn;

    [SerializeField]
    private float mySpawnObjectLifetime;
    protected override void DealSpellEffect()
    {
        GameObject spawnObject = Instantiate(myObjectToSpawn, transform.position, Quaternion.identity);
        Destroy(spawnObject, mySpawnObjectLifetime);
    }
}
