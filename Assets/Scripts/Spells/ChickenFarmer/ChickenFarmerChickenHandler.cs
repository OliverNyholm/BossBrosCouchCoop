using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChickenFarmerChickenHandler : MonoBehaviour
{
    [SerializeField]
    private const int myMaxChickenCount = 8;
    private List<GameObject> myChickens = new List<GameObject>(myMaxChickenCount);

    [SerializeField]
    private Vector3[] myFlightFormation = new Vector3[myMaxChickenCount];

    [SerializeField]
    private float myInitialFlightTime = 2.0f;
    [SerializeField]
    private float myAddedFlightDurationPerChicken = 1.0f;

    [SerializeField]
    private SpellOverTime myEggBuff = null;
    private ChickenFarmerEggBomb myEggBomb = null;
    private Stats myStats = null;

    [SerializeField]
    private float myGoldenEggInterval = 10.0f;
    private float myGoldenEggTimer = 0.0f;

    private void Awake()
    {
        myFlightFormation[0] = new Vector3(1.0f, 2.5f, 0.0f);
        myFlightFormation[1] = new Vector3(-1.0f, 2.5f, 0.0f);
        myFlightFormation[2] = new Vector3(0.0f, 2.5f, -1.0f);
        myFlightFormation[3] = new Vector3(0.0f, 2.5f, 1.0f);
        myFlightFormation[4] = new Vector3(0.7f, 2.5f, 0.7f);
        myFlightFormation[5] = new Vector3(-0.7f, 2.5f, 0.7f);
        myFlightFormation[6] = new Vector3(0.7f, 2.5f, -0.7f);
        myFlightFormation[7] = new Vector3(-0.7f, 2.5f, -0.7f);

        myGoldenEggTimer = myGoldenEggInterval;
        myStats = GetComponent<Stats>();

        StartCoroutine(GiveFirstSpawnEggs());
    }

    private void Update()
    {
        myGoldenEggTimer -= Time.deltaTime;
        if (myGoldenEggTimer <= 0.0f)
        {
            if (!myStats.HasMaxSpellOverTimeStackCount(myEggBuff))
                SpawnEgg(gameObject);

            myGoldenEggTimer = myGoldenEggInterval;
        }
    }

    public float EnableFlightMode()
    {
        float flightDuration = myInitialFlightTime + myChickens.Count * myAddedFlightDurationPerChicken;
        for (int index = 0; index < myChickens.Count; index++)
            myChickens[index].GetComponent<ChickenMovementComponent>().SetFlightMode(myFlightFormation[index], flightDuration);

        return flightDuration;
    }

    public void SendBombChicken(GameObject aBombingTarget, ChickenFarmerEggBomb anEggBomb)
    {
        int lastIndex = myChickens.Count - 1;
        myChickens[lastIndex].GetComponent<ChickenMovementComponent>().EnableBombingMode(aBombingTarget);
        myChickens.RemoveAt(lastIndex);

        myEggBomb = anEggBomb;
    }

    public void ReturnChicken(GameObject aChicken)
    {
        myChickens.Remove(aChicken);
    }

    public void DisableFlightMode()
    {
        GetComponent<PlayerMovementComponent>().SetFlying(false);
    }

    public void AddChickenAndSetPosition(GameObject aChicken)
    {
        Vector3 spawnPosition = myFlightFormation[myChickens.Count];
        spawnPosition.y = 0.0f;

        aChicken.transform.position = transform.position + transform.rotation * spawnPosition;

        aChicken.GetComponent<Chicken>().SetParent(gameObject);
        aChicken.GetComponent<ChickenMovementComponent>().SetParentAndOffset(gameObject, spawnPosition);

        myChickens.Add(aChicken);
    }
    
    public bool HasMaxAmountOfChickenActive()
    {
        return myChickens.Count == myMaxChickenCount;
    }

    public int GetCurrentChickenCount()
    {
        return myChickens.Count;
    }

    public int GetMaxChickenCount()
    {
        return myMaxChickenCount;
    }

    public void SetEggBuff(SpellOverTime anEggBuff)
    {
        myEggBuff = anEggBuff;
        myEggBuff.CreatePooledObjects(PoolManager.Instance, myEggBuff.myMaxStackCount + 2, gameObject);
    }

    public void SpawnEgg(GameObject aSpawner)
    {
        GameObject eggBuff = PoolManager.Instance.GetPooledObject(myEggBuff.GetComponent<UniqueID>().GetID());
        if (!eggBuff)
            return;

        Spell spellScript = eggBuff.GetComponent<Spell>();
        spellScript.SetParent(aSpawner);
        spellScript.AddDamageIncrease(myStats.myDamageIncrease);

        eggBuff.transform.position = transform.position;
        eggBuff.transform.rotation = transform.rotation;

        spellScript.SetTarget(gameObject);
        spellScript.Restart();

        AudioClip spawnSound = spellScript.GetSpellSFX().mySpawnSound;
        if (spawnSound)
            GetComponent<AudioSource>().PlayOneShot(spawnSound);
    }

    public void DropBomb(GameObject aChicken, GameObject aBombTarget)
    {
        GameObject eggBomb = PoolManager.Instance.GetPooledObject(myEggBomb.GetComponent<UniqueID>().GetID());
        if (!eggBomb)
            return;

        ChickenFarmerEggBomb spellScript = eggBomb.GetComponent<ChickenFarmerEggBomb>();
        spellScript.SetParent(gameObject);
        spellScript.AddDamageIncrease(myStats.myDamageIncrease);

        eggBomb.transform.position = aChicken.transform.position;
        eggBomb.transform.rotation = aChicken.transform.rotation;

        spellScript.SetTargetPosition(aBombTarget.transform.position);

        spellScript.SetTarget(aBombTarget);
        spellScript.Restart();

        AudioClip spawnSound = spellScript.GetSpellSFX().mySpawnSound;
        if (spawnSound)
            GetComponent<AudioSource>().PlayOneShot(spawnSound);
    }

    private IEnumerator GiveFirstSpawnEggs()
    {
        float waitDuration = 0.1f;
        while (waitDuration > 0.0f)
        {
            waitDuration -= Time.deltaTime;
            yield return null;
        }

        for (int index = 0; index < myEggBuff.myMaxStackCount; index++)
            SpawnEgg(gameObject);
    }
}
