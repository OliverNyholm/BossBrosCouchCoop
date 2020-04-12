using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialHeal : TutorialCompletion
{
    [Header("The class to try out healing with")]
    [SerializeField]
    private GameObject myTutorialHealer = null;

    [SerializeField]
    private GameManager myGameManager = null;

    [SerializeField]
    private DynamicCamera myDynamicCamera = null;

    protected override bool StartTutorial()
    {
        if (!base.StartTutorial())
            return false;

        myGameManager.ChangeClassInGame(myTutorialHealer);
        myDynamicCamera.ReplacePlayersToTarget(myTargetHandler);

        Players = new List<GameObject>(myTargetHandler.GetAllPlayers());

        foreach (GameObject player in Players)
        {
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
                SetPlayerCompleted(player);
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
