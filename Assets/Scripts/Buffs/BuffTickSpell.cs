using UnityEngine;
using UnityEngine.Networking;

public class BuffTickSpell : BuffSpell
{
    private float myInterval;
    private float myIntervalTimer;
    private int myTickDamage;

    [SyncVar]
    private GameObject myTarget;

    public BuffTickSpell(Buff aBuff, GameObject aParent, GameObject aTarget) : base(aBuff, aParent)
    {
        TickBuff buff = aBuff as TickBuff;

        myTickDamage = buff.myTotalDamage / buff.myNrOfTicks;
        myInterval = buff.myDuration / buff.myNrOfTicks;
        myIntervalTimer = 0.0f;

        myTarget = aTarget;
    }

    public override void Tick()
    {
        base.Tick();

        myIntervalTimer += Time.deltaTime;

        if (myIntervalTimer >= myInterval)
        {
            myIntervalTimer -= myInterval;

            if (myBuff.mySpellType == SpellType.HOT)
                myTarget.GetComponent<Health>().GainHealth(myTickDamage);
            else
                myTarget.GetComponent<Health>().TakeDamage(myTickDamage);
        }
    }
}
