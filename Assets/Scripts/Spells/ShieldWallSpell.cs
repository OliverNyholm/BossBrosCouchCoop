using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldWallSpell : Spell
{

    [SerializeField]
    private GameObject myShieldWallPrefab = null;

    protected override void DealSpellEffect()
    {
        GameObject shield = PoolManager.Instance.GetPooledObject(myShieldWallPrefab.GetComponent<UniqueID>().GetID());

        SetSpellParent(myParent, shield);
    }

    protected override string GetSpellDetail()
    {
        string detail = "to hold up your and block all incoming projectiles";

        return detail;
    }

    private void SetSpellParent(GameObject aParent, GameObject aChild)
    {
        aChild.transform.parent = aParent.transform;
    }

    public override void CreatePooledObjects(PoolManager aPoolManager, int aSpellMaxCount)
    {
        base.CreatePooledObjects(aPoolManager, aSpellMaxCount);

        aPoolManager.AddPoolableObjects(myShieldWallPrefab, myShieldWallPrefab.GetComponent<UniqueID>().GetID(), aSpellMaxCount);
    }
}
