using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewSpellChannel : ChannelSpell
{
    [SerializeField]
    private GameObject mySpellToSpawn = null;

    public override void Reset()
    {
        base.Reset();
    }

    public override void Restart()
    {
        StartCoroutine();
        //PlayerUIComponent uiComponent = myParent.GetComponent<PlayerUIComponent>();
        //HealTargetArrow healTargetArrow = myParent.GetComponentInChildren<HealTargetArrow>();
        //if (uiComponent && healTargetArrow)
        //    healTargetArrow.EnableHealTarget(uiComponent.myCharacterColor);
    }

    protected override void Update()
    {
        transform.position = myParent.transform.position;
        transform.rotation = myParent.transform.rotation;
    }

    private void StartCoroutine()
    {
        myParent.GetComponent<CastingComponent>().StartChannel(myChannelTime, this, gameObject, 0.0f);
    }

    public override void OnStoppedChannel()
    {
        base.OnStoppedChannel();

        //HealTargetArrow healTargetArrow = myParent.GetComponentInChildren<HealTargetArrow>();
        //if (healTargetArrow)
        //    healTargetArrow.DisableHealTarget();

        PlayerCastingComponent playerCastingComponent = myParent.GetComponent<PlayerCastingComponent>();
        if (playerCastingComponent)
            playerCastingComponent.SpawnSpellExternal(mySpellToSpawn.GetComponent<Spell>(), transform.position);
    }

    public override bool IsCastableWhileMoving()
    {
        return myIsCastableWhileMoving;
    }

    public override void CreatePooledObjects(PoolManager aPoolManager, int aSpellMaxCount)
    {
        base.CreatePooledObjects(aPoolManager, aSpellMaxCount);

        Spell mySpellToSpawnSpellComponent = mySpellToSpawn.GetComponent<Spell>();
        if (mySpellToSpawnSpellComponent)
            mySpellToSpawnSpellComponent.CreatePooledObjects(aPoolManager, mySpellToSpawnSpellComponent.myPoolSize);
    }
}
