using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Health : NetworkBehaviour
{
    [SyncVar]
    public int myMaxHealth = 100;
    [SyncVar]
    public int myCurrentHealth = 100;

    public delegate void HealthChanged(float aHealthPercentage, string aHealthText, int aShieldValue);

    [SyncEvent]
    public event HealthChanged EventOnHealthChange;

    private List<BuffShieldSpell> myShields = new List<BuffShieldSpell>();

    public void TakeDamage(int aValue)
    {
        if (!isServer)
        {
            return;
        }

        myCurrentHealth -= CalculateMitigations(aValue);
        if(myCurrentHealth <= 0)
        {
            myCurrentHealth = 0;
        }

        CmdHealthChanged();
    }

    public void GainHealth(int aValue)
    {
        if (!isServer)
        {
            return;
        }

        myCurrentHealth += aValue;
        if (myCurrentHealth > myMaxHealth)
        {
            myCurrentHealth = myMaxHealth;
        }

        CmdHealthChanged();
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
            CmdHealthChanged();
        }
    }

    public void AddShield(BuffShieldSpell aShield)
    {
        if (!isServer)
            return;

        myShields.Add(aShield);
        CmdHealthChanged();
    }

    public void RemoveShield(float aBuffOriginalDuration)
    {
        if (!isServer)
            return;

        for (int index = 0; index < myShields.Count; index++)
        {
            if(myShields[index].IsFinished())
            {
                myShields.RemoveAt(index);
                break;
            }
        }

        CmdHealthChanged();
    }

    [Command]
    private void CmdHealthChanged()
    {
        EventOnHealthChange?.Invoke(GetHealthPercentage(), myCurrentHealth.ToString() + "/" + MaxHealth, GetTotalShieldValue());
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
