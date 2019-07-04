using UnityEngine;

public class Stats : MonoBehaviour
{
    public float mySpeedMultiplier = 1.0f;
    public float myAttackSpeed = 1.0f;
    public float myDamageIncrease = 1.0f;
    public float myDamageMitigator = 1.0f;
    public float myAutoAttackRange = 5;
    public int myAutoAttackDamage = 20;

    public float AttackRange { get { return myAutoAttackRange; } }
}