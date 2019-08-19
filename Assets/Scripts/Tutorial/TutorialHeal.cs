using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialHeal : TutorialCompletion
{
    protected override bool StartTutorial()
    {
        if (!base.StartTutorial())
            return false;

        foreach (GameObject player in myPlayers)
        {
            Health health = player.GetComponent<Health>();
            health.TakeDamage(100, Color.red);
            health.EventOnHealthChange += OnHealthChanged;
        }

        return true;
    }

    public void OnHealthChanged(float aHealthPercentage, string aHealthText, int aShieldValue)
    {
        foreach (GameObject player in myPlayers)
        {
            if (!myCompletedPlayers.Contains(player) && player.GetComponent<Health>().GetHealthPercentage() >= 1.0f)
            {
                myCompletedPlayers.Add(player);
                SetPlayerCompleted(player);
                if (myCompletedPlayers.Count == myPlayers.Count)
                {
                    foreach (GameObject character in myPlayers)
                    {
                        character.GetComponent<Health>().EventOnHealthChange -= OnHealthChanged;
                    }
                    EndTutorial();
                }
            }
        }
    }
}
