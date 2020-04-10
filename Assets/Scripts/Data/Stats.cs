using UnityEngine;

public class Stats : MonoBehaviour
{
    public float mySpeedMultiplier = 1.0f;
    public float myAttackSpeed = 1.0f;
    public float myDamageIncrease = 1.0f;
    public float myDamageMitigator = 1.0f;
    public float myAutoAttackRange = 5;
    public int myAutoAttackDamage = 20;

    public bool myCanBeStunned = true;
    private bool myIsStunned = false;
    private float myStunDuration = 0.0f;

    public float AttackRange { get { return myAutoAttackRange; } }
    public bool IsStunned() { return myIsStunned; }
    public void SetStunned(float aDuration)
    {
        myStunDuration = aDuration;
        if (myStunDuration > 0.0f)
        {
            myIsStunned = true;
        }
        else
            myIsStunned = false;
    }

    public void Update()
    {
        if (myStunDuration > 0.0f)
        {
            myStunDuration -= Time.deltaTime;
            if (myStunDuration <= 0.0f)
                myIsStunned = false;
        }
    }
}