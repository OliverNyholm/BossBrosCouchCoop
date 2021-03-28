using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChickenFarmerSpawnChicken : Spell
{
    [SerializeField]
    private SpellOverTime myRequiredEggBuff = null;

    [SerializeField]
    private GameObject myChickenToSpawn = null;

    public override bool IsSpellAvailable(GameObject aCaster)
    {
        ChickenFarmerChickenHandler chickenHandler = aCaster.GetComponent<ChickenFarmerChickenHandler>();
        if (!chickenHandler)
            return false;

        if (chickenHandler.HasMaxAmountOfChickenActive())
            return false;

        Stats stats = aCaster.GetComponent<Stats>();
        if (!stats)
            return false;

        return stats.HasSpellOverTime(myRequiredEggBuff);
    }
    protected override void OnFirstUpdate()
    {
        base.OnFirstUpdate();

        Stats stats = myParent.GetComponent<Stats>();
        if (stats)
            stats.RemoveSpellOverTimeStack(myRequiredEggBuff);
    }

    protected override void DealSpellEffect()
    {
        ChickenFarmerChickenHandler chickenHandler = myParent.GetComponent<ChickenFarmerChickenHandler>();
        if (!chickenHandler)
            return;

        GameObject chickenGO = PoolManager.Instance.GetPooledObject(myChickenToSpawn.GetComponent<UniqueID>().GetID());
        if (!chickenGO)
            return;

        chickenHandler.AddChickenAndSetPosition(chickenGO);
    }

    public override void CreatePooledObjects(PoolManager aPoolManager, int aSpellMaxCount, GameObject aSpawner = null)
    {
        base.CreatePooledObjects(aPoolManager, aSpellMaxCount);

        ChickenFarmerChickenHandler chickenHandler = aSpawner.GetComponent<ChickenFarmerChickenHandler>();
        if (!chickenHandler)
            return;

        PoolManager.Instance.AddPoolableObjects(myChickenToSpawn, myChickenToSpawn.GetComponent<UniqueID>().GetID(), chickenHandler.GetMaxChickenCount());
    }
}
