using UnityEngine;

[CreateAssetMenu(fileName = "New Buff", menuName = "Buff/Simple")]
public class Buff : ScriptableObject
{
    public SpellType mySpellType = SpellType.Buff;
    public float myDuration;

    public float mySpeedMultiplier;
    public float myAttackSpeed;
    public float myDamageIncrease;
    public float myDamageMitigator;

    public virtual void ApplyBuff(ref Stats aStats)
    {
        aStats.mySpeedMultiplier += mySpeedMultiplier;
        aStats.myAttackSpeed += myAttackSpeed;
        aStats.myDamageIncrease += myDamageIncrease;
        aStats.myDamageMitigator += myDamageMitigator;
    }

    public virtual void EndBuff(ref Stats aStats)
    {
        aStats.mySpeedMultiplier -= mySpeedMultiplier;
        aStats.myAttackSpeed -= myAttackSpeed;
        aStats.myDamageIncrease -= myDamageIncrease;
        aStats.myDamageMitigator -= myDamageMitigator;
    }

    public virtual BuffSpell InitializeBuff(GameObject aParent)
    {
        return new BuffSpell(this, aParent);
    }
}

[CreateAssetMenu(fileName = "New Buff", menuName = "Buff/Shield")]
public class ShieldBuff : Buff
{
    public new SpellType mySpellType = SpellType.Shield;

    public int myShieldValue;
    private int myCurrentShieldValue;

    public override void ApplyBuff(ref Stats aStats)
    {
        base.ApplyBuff(ref aStats);

        myCurrentShieldValue = myShieldValue;
    }

    public int SoakDamage(int aDamage)
    {
        myCurrentShieldValue -= aDamage;

        if (myCurrentShieldValue > 0)
            return 0;

        return Mathf.Abs(myCurrentShieldValue);
    }
}
