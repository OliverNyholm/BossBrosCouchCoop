using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialAutoAttack : TutorialCompletion
{
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
        }

        for (int index = 0; index < myTargetsToHit.Count; index++)
        {
            myTargetHandler.AddEnemy(myTargetsToHit[index], true);
        }

        return true;
    }

    public void OnSpellSpawned(GameObject aPlayer, GameObject aSpell, int aSpellIndex)
    {
        if (aSpell != myAutoAttackPrefab)
            return;

        if (myCompletedPlayers.Contains(aPlayer))
            return;

        myCompletedPlayers.Add(aPlayer);
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
            EndTutorial();
        }
    }
}
