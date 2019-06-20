using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldWallSpell : Spell
{

    [SerializeField]
    private GameObject myShieldWallPrefab = null;

    protected override void DealSpellEffect()
    {
        GameObject shield = Instantiate(myShieldWallPrefab, myParent.transform);

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
}
