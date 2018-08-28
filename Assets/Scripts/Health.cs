using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Health : NetworkBehaviour
{
    [SyncVar]
    public int myMaxHealth = 100;
    [SyncVar]
    public int myCurrentHealth = 100;

    public delegate void HealthChanged(float aHealthPercentage, string aHealthText);

    [SyncEvent]
    public event HealthChanged EventOnHealthChange;

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

    [Command]
    private void CmdHealthChanged()
    {
        EventOnHealthChange?.Invoke(GetHealthPercentage(), myCurrentHealth.ToString() + "/" + MaxHealth);
    }

    public int CalculateMitigations(int anIncomingDamageValue)
    {
        Stats parentStats = GetComponent<Stats>();

        int damage = (int)(anIncomingDamageValue * parentStats.myDamageMitigator);

        Debug.Log("Original: " + anIncomingDamageValue + "    New: " + damage);
        return damage;
    }
}
