using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialAutoAttack : TutorialCompletion
{
    [SerializeField]
    private GameObject myAutoAttackPrefab = null;

    [SerializeField]
    private List<GameObject> myTargetsToHit = new List<GameObject>();

    private List<GameObject> myCompletedPlayers = new List<GameObject>();

    protected override bool StartTutorial()
    {
        if (!base.StartTutorial())
            return false;

        foreach (GameObject player in myPlayers)
        {
            player.GetComponent<Character>().myEventOnSpellSpawned += OnSpellSpawned;
        }

        for (int index = 0; index < myTargetsToHit.Count; index++)
        {
            myTargetHandler.AddEnemy(myTargetsToHit[index]);
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
        SetPlayerCompleted(aPlayer);
        if (myCompletedPlayers.Count == myPlayers.Count)
        {
            foreach (GameObject player in myPlayers)
            {
                player.GetComponent<Character>().myEventOnSpellSpawned -= OnSpellSpawned;
                player.GetComponent<Character>().SetTarget(null);
            }
            for (int index = 0; index < myTargetsToHit.Count; index++)
            {
                myTargetHandler.RemoveEnemy(myTargetsToHit[index]);
            }
            EndTutorial();
        }
    }
}
