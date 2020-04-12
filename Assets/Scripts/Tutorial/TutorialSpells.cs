using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialSpells : TutorialCompletion
{
    [SerializeField]
    private int mySpellIndexToExceed = 0;

    [SerializeField]
    private GameObject myAutoAttackPrefab = null;

    [SerializeField]
    private List<GameObject> myTargetsToHit = new List<GameObject>();

    protected override bool StartTutorial()
    {
        if (!base.StartTutorial())
            return false;

        foreach (GameObject player in Players)
        {
            player.GetComponent<PlayerCastingComponent>().myEventOnSpellSpawned += OnSpellSpawned;
            if (mySpellIndexToExceed < 3 && player.GetComponent<Class>().myClassName == "Loremaster")
            {
                myCompletedPlayers.Add(player);
                SetPlayerCompleted(player);
                if (Players.Count == 1)
                {
                    EndTutorial();
                    return true;
                }
            }
        }

        for (int index = 0; index < myTargetsToHit.Count; index++)
        {
            myTargetHandler.AddEnemy(myTargetsToHit[index], true);
        }

        myTutorialPanel.SetSpellsHightlight(true, mySpellIndexToExceed > 0);

        return true;
    }

    public void OnSpellSpawned(GameObject aPlayer, GameObject aSpell, int aSpellIndex)
    {
        if (aSpell == myAutoAttackPrefab || aSpellIndex <= mySpellIndexToExceed)
            return;

        if (myCompletedPlayers.Contains(aPlayer))
            return;

        myCompletedPlayers.Add(aPlayer);
        SetPlayerCompleted(aPlayer);
        if (myCompletedPlayers.Count == Players.Count)
        {
            foreach (GameObject player in Players)
            {
                player.GetComponent<PlayerCastingComponent>().myEventOnSpellSpawned -= OnSpellSpawned;
                player.GetComponent<PlayerTargetingComponent>().SetTarget(null);
            }
            for (int index = 0; index < myTargetsToHit.Count; index++)
            {
                myTargetHandler.RemoveEnemy(myTargetsToHit[index]);
            }
            myTutorialPanel.SetSpellsHightlight(false, mySpellIndexToExceed > 0);
            EndTutorial();
        }
    }
}
