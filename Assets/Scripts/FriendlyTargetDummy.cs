using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendlyTargetDummy : Player
{

    protected override void Start()
    {
        myAnimator = GetComponent<Animator>();

        myHealth = GetComponent<Health>();
        myResource = GetComponent<Resource>();
        myStats = GetComponent<Stats>();
        myClass = GetComponent<Class>();

        myBuffs = new List<BuffSpell>();

        Target = null;
        myIsCasting = false;

        //SetupHud(transform.GetComponentInChildren<Canvas>().transform.Find("EnemyUI").transform);
    }

    protected override void Update()
    {
        HandleBuffs();
    }

    protected override bool IsAbleToCastSpell(Spell aSpellScript)
    {
        return false;
    }

    protected override bool IsMoving()
    {
        return false;
    }

    public void SetBossHud(GameObject aBossHudGameObject)
    {
        SetupHud(aBossHudGameObject.transform);
    }
}
