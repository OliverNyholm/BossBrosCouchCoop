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

    [SerializeField]
    private int mySolarFlareInterval = 3;
    private int myTickCount = 0;

    private List<GameObject> myPlayers;

    protected override void Awake()
    {
        base.Awake();
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

            DealSpellEffect(); //Heal Self
            myTickCount++;
            if (myTickCount % mySolarFlareInterval == 0)
            {
                PlayerCastingComponent playerCastingComponent = myParent.GetComponent<PlayerCastingComponent>();
                if (playerCastingComponent)
                {
                    GuidingStarPulse guidingStarPulse = playerCastingComponent.SpawnSpellExternal(myPulseSpell.GetComponent<Spell>(), transform.position) as GuidingStarPulse;
                    if (guidingStarPulse)
                        guidingStarPulse.SetPlayers(myPlayers);
                }
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
