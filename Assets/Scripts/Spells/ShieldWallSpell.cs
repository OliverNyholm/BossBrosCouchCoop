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

    private void SetSpellParent(GameObject aParent, GameObject aChild)
    {
        aChild.transform.parent = aParent.transform;
        aChild.transform.localPosition = new Vector3(0.0f, 1.4f, 1.2f);
        aChild.transform.localRotation = Quaternion.identity;
    }

    public override void CreatePooledObjects(PoolManager aPoolManager, int aSpellMaxCount)
    {
        base.CreatePooledObjects(aPoolManager, aSpellMaxCount);

        aPoolManager.AddPoolableObjects(myShieldWallPrefab, myShieldWallPrefab.GetComponent<UniqueID>().GetID(), aSpellMaxCount);
    }
}
