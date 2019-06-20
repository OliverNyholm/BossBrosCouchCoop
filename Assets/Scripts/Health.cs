using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField]
    private GameObject myFloatingHealthPrefab = null;


    public int myMaxHealth = 100;

    public int myCurrentHealth = 100;

    public delegate void HealthChanged(float aHealthPercentage, string aHealthText, int aShieldValue);
    public delegate void ThreatGenerated(int aThreatPercentage, int anID);
    public delegate void HealthZero();


    public event HealthChanged EventOnHealthChange;
    public event ThreatGenerated EventOnThreatGenerated;
    public event HealthZero EventOnHealthZero;

    private List<BuffShieldSpell> myShields = new List<BuffShieldSpell>();

    public void TakeDamage(int aValue)
    {
        int damage = CalculateMitigations(aValue);
        myCurrentHealth -= damage;
        if (myCurrentHealth <= 0)
        {
            myCurrentHealth = 0;
            EventOnHealthZero();
        }

        string damageText = damage.ToString();
        if(aValue != damage)
        {
            int absorbed = aValue - damage;
            damageText = damage.ToString() + " (" + absorbed.ToString() + " absorbed)";
        }

        OnHealthChanged();
        SpawnFloatingText(damageText, Color.red);
    }

    public void GainHealth(int aValue)
    {
        myCurrentHealth += aValue;
        if (myCurrentHealth > myMaxHealth)
        {
            myCurrentHealth = myMaxHealth;
        }

        OnHealthChanged();
        SpawnFloatingText(aValue.ToString(), Color.yellow);
    }

    public bool IsDead()
    {
        return myCurrentHealth <= 0;
    }

    public float GetHealthPercentage()
    {
        return (float)myCurrentHealth / myMaxHealth;
    }

    public int MaxHealth
    {
        get { return myMaxHealth; }
        set
        {
            myMaxHealth = value;
            OnHealthChanged();
        }
    }

    public void AddShield(BuffShieldSpell aShield)
    {
        myShields.Add(aShield);
        OnHealthChanged();
        SpawnFloatingText("Shield, " + aShield.GetRemainingShieldHealth().ToString(), Color.yellow);
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

        OnHealthChanged();
        SpawnFloatingText("Shield faded", Color.yellow);
    }

    public void GenerateThreat(int aThreatValue, int anID)
    {
        EventOnThreatGenerated?.Invoke(aThreatValue, anID);
    }

    private void OnHealthChanged()
    {
        EventOnHealthChange?.Invoke(GetHealthPercentage(), myCurrentHealth.ToString() + "/" + MaxHealth, GetTotalShieldValue());
    }

    private void OnHealthZero()
    {
        EventOnHealthZero?.Invoke();
    }

    private void SpawnFloatingText(string aText, Color aColor)
    {
        GameObject floatingHealthGO = Instantiate(myFloatingHealthPrefab, transform);
        FloatingHealth floatingHealth = floatingHealthGO.GetComponent<FloatingHealth>();
        floatingHealth.SetText(aText, aColor);
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
}
