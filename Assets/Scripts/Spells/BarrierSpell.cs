using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierSpell : Spell
{

    [SerializeField]
    private GameObject myBarrierPrefab;

    [SerializeField]
    private float myChannelTime;

    protected override void DealSpellEffect()
    {
        GameObject barrier = Instantiate(myBarrierPrefab, myParent.transform);

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
        myParent.GetComponent<Player>().StartChannel(myChannelTime, this, aChannelSpell);
    }

    private void SetSpellParent(GameObject aParent, GameObject aChild)
    {
        aChild.transform.parent = aParent.transform;
    }
}
