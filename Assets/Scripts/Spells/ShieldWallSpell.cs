using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ShieldWallSpell : Spell
{

    [SerializeField]
    private GameObject myShieldWallPrefab;

    protected override void DealSpellEffect()
    {
        if (!isServer)
            return;

        GameObject shield = Instantiate(myShieldWallPrefab, myTarget.transform);
        NetworkServer.Spawn(shield);
    }

    [ClientRpc]
    private void RpcLeapPlayer(Vector3 aVelocity)
    {
        myParent.GetComponent<PlayerCharacter>().GiveImpulse(aVelocity, true);
    }

    protected override string GetSpellDetail()
    {
        string detail = "to hold up his shield and block all incoming projectiles";

        return detail;
    }
}
