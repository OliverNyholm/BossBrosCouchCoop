using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChickenFarmerThrowEgg : Spell
{
    [SerializeField]
    private SpellOverTime myRequiredEggBuff = null;

    public override bool IsSpellAvailable(GameObject aCaster)
    {
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

    public override void CreatePooledObjects(PoolManager aPoolManager, int aSpellMaxCount, GameObject aSpawner = null)
    {
        base.CreatePooledObjects(aPoolManager, aSpellMaxCount, aSpawner);

        //This is quite weird, but was an easy solution since I can set eggbuff property here, but not in the script as it is added via code.
        aSpawner.GetComponent<ChickenFarmerChickenHandler>().SetEggBuff(myRequiredEggBuff);
    }
}
