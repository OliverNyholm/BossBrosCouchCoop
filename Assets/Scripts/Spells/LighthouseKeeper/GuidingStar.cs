using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuidingStar : ToggleSpell
{
    [SerializeField]
    private GameObject myPulseSpell = null;

    [SerializeField]
    private float myTickInterval = 2.0f;
    private float myIntervalTimer;

    private List<GameObject> myPlayers;

    private void Awake()
    {
        myPlayers = FindObjectOfType<TargetHandler>().GetAllPlayers();
    }

    public override void Restart()
    {
        base.Restart();

        myIntervalTimer = myTickInterval;
    }

    protected override void Update()
    {
        base.Update();

        myIntervalTimer -= Time.deltaTime;
        if (myIntervalTimer <= 0.0f)
        {
            myIntervalTimer += myTickInterval;

            PlayerCastingComponent playerCastingComponent = myParent.GetComponent<PlayerCastingComponent>();
            if (playerCastingComponent)
            {
                DealSpellEffect(); //Heal Self
                GuidingStarPulse guidingStarPulse = playerCastingComponent.SpawnSpellExternal(myPulseSpell.GetComponent<Spell>(), transform.position) as GuidingStarPulse;
                if (guidingStarPulse)
                    guidingStarPulse.SetPlayers(myPlayers);
            }
        }
    }

    public override void CreatePooledObjects(PoolManager aPoolManager, int aSpellMaxCount)
    {
        base.CreatePooledObjects(aPoolManager, aSpellMaxCount);

        Spell pulseSpell = myPulseSpell.GetComponent<Spell>();
        if (pulseSpell)
            pulseSpell.CreatePooledObjects(aPoolManager, pulseSpell.myPoolSize);
    }
}
