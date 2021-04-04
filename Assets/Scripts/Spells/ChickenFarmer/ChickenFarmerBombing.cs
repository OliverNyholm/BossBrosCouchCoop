using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChickenFarmerBombing : Spell
{
    [SerializeField]
    private ChickenFarmerEggBomb myEggBomb = null;

    public override bool IsSpellAvailable(GameObject aCaster)
    {
        ChickenFarmerChickenHandler chickenHandler = aCaster.GetComponent<ChickenFarmerChickenHandler>();
        if (!chickenHandler)
            return false;

        return chickenHandler.GetCurrentChickenCount() > 0;
    }

    protected override void DealSpellEffect()
    {
        base.DealSpellEffect();

        ChickenFarmerChickenHandler chickenHandler = myParent.GetComponent<ChickenFarmerChickenHandler>();
        if (!chickenHandler)
            return;

        chickenHandler.SendBombChicken(myTarget, myEggBomb);
    }

    public override void CreatePooledObjects(PoolManager aPoolManager, int aSpellMaxCount, GameObject aSpawner = null)
    {
        base.CreatePooledObjects(aPoolManager, aSpellMaxCount, aSpawner);

        myEggBomb.CreatePooledObjects(aPoolManager, aSpellMaxCount, aSpawner);
    }
}
