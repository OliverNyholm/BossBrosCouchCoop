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

        GameObject shield = Instantiate(myShieldWallPrefab, myParent.transform);
        NetworkServer.Spawn(shield);

        RpcSetSpellParent(myParent, shield);
    }

    protected override string GetSpellDetail()
    {
        string detail = "to hold up your and block all incoming projectiles";

        return detail;
    }

    [ClientRpc]
    private void RpcSetSpellParent(GameObject aParent, GameObject aChild)
    {
        aChild.transform.parent = aParent.transform;
    }
}
