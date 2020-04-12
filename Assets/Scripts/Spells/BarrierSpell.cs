using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierSpell : Spell
{

    [SerializeField]
    private GameObject myBarrierPrefab = null;

    [SerializeField]
    private float myChannelTime = 0.0f;

    protected override void DealSpellEffect()
    {
        GameObject barrier = PoolManager.Instance.GetPooledObject(myBarrierPrefab.GetComponent<UniqueID>().GetID());

        SetSpellParent(myParent, barrier);
        StartCoroutine(barrier);
    }

    protected override string GetSpellDetail()
    {
        string detail = "to channel a barrier around yourself for " + myChannelTime + " seconds. Standing inside barrier will grant a buff to reduce damage taken by 50 % ";

        return detail;
    }

    private void StartCoroutine(GameObject aChannelSpell)
    {
        myParent.GetComponent<CastingComponent>().StartChannel(myChannelTime, this, aChannelSpell);
    }

    private void SetSpellParent(GameObject aParent, GameObject aChild)
    {
        aChild.transform.parent = aParent.transform;
        aChild.transform.localPosition = Vector3.zero;
        aChild.transform.localRotation = Quaternion.identity;
    }

    public override void CreatePooledObjects(PoolManager aPoolManager, int aSpellMaxCount)
    {
        base.CreatePooledObjects(aPoolManager, aSpellMaxCount);

        aPoolManager.AddPoolableObjects(myBarrierPrefab, myBarrierPrefab.GetComponent<UniqueID>().GetID(), aSpellMaxCount);
    }
}
