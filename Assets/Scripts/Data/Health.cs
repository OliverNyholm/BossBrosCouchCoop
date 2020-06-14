using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int myMaxHealth = 100;

    public int myCurrentHealth = 100;

    public int myHealthPerTick = 0;
    public float myHealthTickInterval = 1.0f;
    private float myHealthTickTimestamp;
    public float myTimeBeforeHealthRegenerationAfterDamage = 5.0f;
    private float myTimeBeforeHealthRegenerationTimestamp;
    public bool myShouldSpawnFloatingHealthOnTick = false;

    public delegate void HealthChanged(float aHealthPercentage, string aHealthText, int aShieldValue, bool aIsDamage);
    public delegate void ThreatGenerated(int aThreatPercentage, int anID, bool anIsDamage);
    public delegate void HealthZero();

    public event HealthChanged EventOnHealthChange;
    public event ThreatGenerated EventOnThreatGenerated;
    public event HealthZero EventOnHealthZero;

    private List<SpellOverTime> myShields = new List<SpellOverTime>();

    private void Update()
    {
        if (myHealthPerTick == 0)
            return;

        if (myCurrentHealth == myMaxHealth)
            return;

        if (IsDead())
            return;

        if (Time.time < myTimeBeforeHealthRegenerationTimestamp)
            return;

        if (Time.time > myHealthTickTimestamp)
        {
            myHealthTickTimestamp = Time.time;
            GainHealth(myHealthPerTick, myShouldSpawnFloatingHealthOnTick);
        }
    }

    public int TakeDamage(int aValue, Color aDamagerColor, Vector3 aDamagePosition)
    {
        if (IsDead())
            return aValue;

        int damage = CalculateMitigations(aValue);
        string damageText = damage.ToString();
        if (aValue != damage)
        {
            int absorbed = aValue - damage;
            damageText = damage.ToString() + " (-" + absorbed.ToString() + ")";
        }

        if (damage <= 0.0f)
        {
            if (aValue != damage)
                SpawnFloatingText("Absorbed", aDamagerColor, aDamagePosition);

            return damage;
        }

        myCurrentHealth -= damage;
        if (myCurrentHealth <= 0)
        {
            myCurrentHealth = 0;
            OnHealthZero();
        }

        myTimeBeforeHealthRegenerationTimestamp = Time.time + myTimeBeforeHealthRegenerationAfterDamage;

        OnHealthChanged(true);

        if (aValue > 2000 && IsDead())
            SpawnFloatingText("Instant Kill", aDamagerColor, aDamagePosition);
        else
            SpawnFloatingText(damageText, aDamagerColor, aDamagePosition);

        return damage;
    }

    public void GainHealth(int aValue, bool aShouldSpawnFloatingText = true)
    {
        myCurrentHealth += aValue;
        if (myCurrentHealth > myMaxHealth)
        {
            myCurrentHealth = myMaxHealth;
        }

        OnHealthChanged(false);

        if (aShouldSpawnFloatingText)
            SpawnFloatingText(aValue.ToString(), Color.yellow, transform.position);
    }

    public void ReviveToFullHealth()
    {
        myCurrentHealth = MaxHealth;
        OnHealthChanged(false);

        SpawnFloatingText("Revived", Color.yellow, transform.position);
    }

    public bool IsDead()
    {
        return myCurrentHealth <= 0;
    }

    public float GetHealthPercentage()
    {
        return (float)myCurrentHealth / myMaxHealth;
    }

    public void SetHealthPercentage(float aHealthPercentage)
    {
        myCurrentHealth = (int)(myMaxHealth * aHealthPercentage);
    }

    public int MaxHealth
    {
        get { return myMaxHealth; }
        set
        {
            myMaxHealth = value;
            OnHealthChanged(false);
        }
    }

    public void AddShield(SpellOverTime aShield)
    {
        myShields.Add(aShield);
        OnHealthChanged(false);
        SpawnFloatingText("Shield, " + aShield.GetRemainingShieldHealth().ToString(), Color.yellow, transform.position);
    }

    public void RemoveShield(SpellOverTime aSpell)
    {
        uint spellID = aSpell.GetComponent<UniqueID>().GetID();
        for (int index = 0; index < myShields.Count; index++)
        {
            if (myShields[index].GetComponent<UniqueID>().GetID() == spellID)
            {
                myShields.RemoveAt(index);
                break;
            }
        }

        OnHealthChanged(false);
        SpawnFloatingText("Shield faded", Color.yellow, transform.position);
    }

    public void GenerateThreat(int aThreatValue, int anID, bool anIgnoreCombatState)
    {
        EventOnThreatGenerated?.Invoke(aThreatValue, anID, anIgnoreCombatState);
    }

    private void OnHealthChanged(bool aIsDamage)
    {
        EventOnHealthChange?.Invoke(GetHealthPercentage(), myCurrentHealth.ToString() + "/" + MaxHealth, GetTotalShieldValue(), aIsDamage);
    }

    private void OnHealthZero()
    {
        EventOnHealthZero?.Invoke();
    }

    private void SpawnFloatingText(string aText, Color aColor, Vector3 aSpawnLocation)
    {
        GameObject floatingHealthGO = PoolManager.Instance.GetFloatingHealth();
        floatingHealthGO.transform.parent = transform;

        FloatingHealth floatingHealth = floatingHealthGO.GetComponent<FloatingHealth>();
        floatingHealth.SetText(aText, aColor, aSpawnLocation);
    }

    public float CalculateSizeModifier(int aDamage)
    {
        const float startDamage = 100;
        const float modifier = 0.002f;
        if (aDamage > startDamage)
            return 1.0f + aDamage * modifier;

        return 1.0f;
    }

    public int CalculateMitigations(int anIncomingDamageValue)
    {
        Stats parentStats = GetComponent<Stats>();

        int damage = (int)(anIncomingDamageValue * parentStats.myDamageMitigator);

        for (int index = 0; index < myShields.Count; index++)
        {
            damage = myShields[index].SoakDamage(damage);

            if (damage <= 0)
                break;
        }

        return damage;
    }

    public int GetTotalShieldValue()
    {
        int shieldValue = 0;

        for (int index = 0; index < myShields.Count; index++)
        {
            shieldValue += myShields[index].GetRemainingShieldHealth();
        }

        return shieldValue;
    }

    private void OnValidate()
    {
        OnHealthChanged(false);
    }
}
