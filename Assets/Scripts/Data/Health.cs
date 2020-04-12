﻿using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int myMaxHealth = 100;

    public int myCurrentHealth = 100;

    public delegate void HealthChanged(float aHealthPercentage, string aHealthText, int aShieldValue, bool aIsDamage);
    public delegate void ThreatGenerated(int aThreatPercentage, int anID, bool anIsDamage);
    public delegate void HealthZero();

    public event HealthChanged EventOnHealthChange;
    public event ThreatGenerated EventOnThreatGenerated;
    public event HealthZero EventOnHealthZero;

    private List<BuffShieldSpell> myShields = new List<BuffShieldSpell>();

    public int TakeDamage(int aValue, Color aDamagerColor)
    {
        int damage = CalculateMitigations(aValue);
        myCurrentHealth -= damage;
        if (myCurrentHealth <= 0)
        {
            myCurrentHealth = 0;
            OnHealthZero();
        }

        string damageText = damage.ToString();
        if(aValue != damage)
        {
            int absorbed = aValue - damage;
            damageText = damage.ToString() + " (-" + absorbed.ToString() +")";
        }

        OnHealthChanged(true);
        SpawnFloatingText(damageText, aDamagerColor, CalculateSizeModifier(damage));

        return damage;
    }

    public void GainHealth(int aValue)
    {
        myCurrentHealth += aValue;
        if (myCurrentHealth > myMaxHealth)
        {
            myCurrentHealth = myMaxHealth;
        }

        OnHealthChanged(false);
        SpawnFloatingText(aValue.ToString(), Color.yellow, CalculateSizeModifier(aValue));
    }

    public void ReviveToFullHealth()
    {
        myCurrentHealth = MaxHealth;
        OnHealthChanged(false);

        SpawnFloatingText("Revived", Color.yellow, 1.2f);
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

    public void AddShield(BuffShieldSpell aShield)
    {
        myShields.Add(aShield);
        OnHealthChanged(false);
        SpawnFloatingText("Shield, " + aShield.GetRemainingShieldHealth().ToString(), Color.yellow, 1.0f);
    }

    public void RemoveShield()
    {
        for (int index = 0; index < myShields.Count; index++)
        {
            if (myShields[index].IsFinished())
            {
                myShields.RemoveAt(index);
                break;
            }
        }

        OnHealthChanged(false);
        SpawnFloatingText("Shield faded", Color.yellow, 1.0f);
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

    private void SpawnFloatingText(string aText, Color aColor, float aSizeModifier)
    {
        Vector3 randomOffset = new Vector2(Random.Range(-2.0f, 2.0f), Random.Range(0.0f, 2.0f));

        GameObject floatingHealthGO = PoolManager.Instance.GetFloatingHealth();
        floatingHealthGO.transform.parent = transform;
        floatingHealthGO.transform.position = transform.position + randomOffset;

        FloatingHealth floatingHealth = floatingHealthGO.GetComponent<FloatingHealth>();
        floatingHealth.SetText(aText, aColor, aSizeModifier);
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
