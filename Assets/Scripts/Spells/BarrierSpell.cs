using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BarrierSpell : Spell
{

    [SerializeField]
    private GameObject myBarrierPrefab;

    [SerializeField]
    private float myChannelTime;

    protected override void DealSpellEffect()
    {
        if (!isServer)
            return;

        GameObject barrier = Instantiate(myBarrierPrefab, myParent.transform);

        NetworkServer.Spawn(barrier);

        RpcSetSpellParent(myParent, barrier);
        RpcStartCoroutine(barrier);
    }

    protected override string GetSpellDetail()
    {
        string detail = "to channel a barrier around yourself for " + myChannelTime + " seconds. Standing inside barrier will grant a buff to reduce damage taken by 50 % ";

        return detail;
    }

    [ClientRpc]
    private void RpcStartCoroutine(GameObject aChannelSpell)
    {
        myParent.GetComponent<PlayerCharacter>().StartChannel(myChannelTime, this, aChannelSpell);
    }

    [ClientRpc]
    private void RpcSetSpellParent(GameObject aParent, GameObject aChild)
    {
        aChild.transform.parent = aParent.transform;
    }
}
