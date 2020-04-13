using UnityEngine;

[CreateAssetMenu(fileName = "New Buff", menuName = "Buff/Simple")]
public class Buff : ScriptableObject
{
    public SpellTypeToBeChanged mySpellType = SpellTypeToBeChanged.Buff;
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
