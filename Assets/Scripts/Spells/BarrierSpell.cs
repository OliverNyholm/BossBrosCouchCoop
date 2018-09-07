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

        GameObject shield = Instantiate(myBarrierPrefab, myTarget.transform);
        shield.GetComponent<Barrier>().SetParent(myTarget);

        NetworkServer.Spawn(shield);
        StartCoroutine(myTarget.GetComponent<PlayerCharacter>().SpellChannelRoutine(myChannelTime));
    }

    protected override string GetSpellDetail()
    {
        string detail = "to channel a barrier around yourself for " + myChannelTime + " seconds. Standing inside barrier will grant a buff to reduce damage taken by 50 % ";

        return detail;
    }
}
