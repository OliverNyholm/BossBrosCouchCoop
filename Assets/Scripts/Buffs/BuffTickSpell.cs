using UnityEngine;

public class BuffTickSpell : BuffSpell
{
    private float myInterval;
    private float myIntervalTimer;
    private int myTickDamage;

    private bool myShouldTickSpellEffect;

    private GameObject myTarget;

    public BuffTickSpell(Buff aBuff, GameObject aParent, GameObject aTarget) : base(aBuff, aParent)
    {
        TickBuff buff = aBuff as TickBuff;

        myTickDamage = buff.myTotalDamage / buff.myNrOfTicks;
        myInterval = buff.myDuration / buff.myNrOfTicks;
        myIntervalTimer = 0.0f;

        myTarget = aTarget;
        myShouldTickSpellEffect = false;
    }

    public override void Tick()
    {
        base.Tick();

        myIntervalTimer += Time.deltaTime;

        if (myIntervalTimer >= myInterval)
        {
            myIntervalTimer -= myInterval;
            myShouldTickSpellEffect = true;
        }
    }

    public bool ShouldDealTickSpellEffect
    {
        get { return myShouldTickSpellEffect; }
        set { myShouldTickSpellEffect = value; }
    }

    public int GetTickValue()
    {
        return myTickDamage;
    }

    public GameObject GetTarget()
    {
        return myTarget;
    }
}
