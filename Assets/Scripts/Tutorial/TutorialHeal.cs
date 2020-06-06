using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialHeal : TutorialCompletion
{
    [Header("The heal spell")]
    [SerializeField]
    private GameObject myHealSpell = null;

    protected override void Awake()
    {
        base.Awake();

        Spell spell = myHealSpell.GetComponent<Spell>();
        spell.CreatePooledObjects(PoolManager.Instance, 6);
    }

    protected override bool StartTutorial()
    {
        if (!base.StartTutorial())
            return false;

        foreach (GameObject player in Players)
        {
            player.GetComponent<Class>().ReplaceSpell(myHealSpell, 0);

            Health health = player.GetComponent<Health>();
                health.EventOnHealthChange += OnHealthChanged;
        }

        return true;
    }

    public void OnHealthChanged(float aHealthPercentage, string aHealthText, int aShieldValue, bool aIsDamage)
    {
        foreach (GameObject player in Players)
        {
            if (!myCompletedPlayers.Contains(player) && player.GetComponent<Health>().GetHealthPercentage() >= 1.0f)
            {
                myCompletedPlayers.Add(player);
                if (myCompletedPlayers.Count == Players.Count)
                {
                    foreach (GameObject character in Players)
                    {
                        character.GetComponent<Health>().EventOnHealthChange -= OnHealthChanged;
                    }
                    EndTutorial();
                }
            }
        }
    }
}
