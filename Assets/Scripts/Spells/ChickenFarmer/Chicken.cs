using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chicken : Character
{
    [SerializeField]
    private GameObject myParent = null;

    [SerializeField]
    private SpellOverTime myEggBuff = null;

    [SerializeField]
    private float myDropEggInterval = 2.0f;
    private float myDropEggTimer = 0.0f;

    public void SetParent(GameObject aParent)
    {
        myParent = aParent;
    }

    public override void Reset()
    {
        base.Reset();
        myDropEggTimer = myDropEggInterval;
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (!myParent || !myEggBuff)
            return;

        myDropEggTimer -= Time.deltaTime;
        if (myDropEggTimer <= 0.0f)
        {
            SpawnEgg();
            myDropEggTimer = myDropEggInterval;
        }
    }

    private void SpawnEgg()
    {
        GameObject eggBuff = PoolManager.Instance.GetPooledObject(myEggBuff.GetComponent<UniqueID>().GetID());
        if (!eggBuff)
            return;

        Spell spellScript = eggBuff.GetComponent<Spell>();
        spellScript.SetParent(gameObject);
        spellScript.AddDamageIncrease(myStats.myDamageIncrease);

        eggBuff.transform.position = myParent.transform.position;
        eggBuff.transform.rotation = myParent.transform.rotation;

        spellScript.SetTarget(myParent);
        spellScript.Restart();

        AudioClip spawnSound = spellScript.GetSpellSFX().mySpawnSound;
        if (spawnSound)
            GetComponent<AudioSource>().PlayOneShot(spawnSound);
    }
}
