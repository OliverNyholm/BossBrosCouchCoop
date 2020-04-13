using UnityEngine;
public class BuffShieldSpell : BuffSpell {

    private int myCurrentShieldValue;

    public BuffShieldSpell(Buff aBuff, GameObject aParent) : base(aBuff, aParent)
    {
        ShieldBuff buff = aBuff as ShieldBuff;

        myCurrentShieldValue = buff.myShieldValue;
    }

    public int SoakDamage(int aDamage)
    {
        myCurrentShieldValue -= aDamage;

        if (myCurrentShieldValue > 0)
            return 0;

        return Mathf.Abs(myCurrentShieldValue);
    }
    public int GetRemainingShieldHealth()
    {
        return myCurrentShieldValue;
    }

    public override bool IsFinished()
    {
        if (HasShieldRunOut())
            return true;

        return base.IsFinished();
    }

    public bool HasShieldRunOut()
    {
        return myCurrentShieldValue <= 0.0f;
    }

}
