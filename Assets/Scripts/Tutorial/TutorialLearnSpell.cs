using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;

public class TutorialLearnSpell : TutorialCompletion
{
    [SerializeField]
    private int mySpellIndexToCast = 0;

    [SerializeField]
    private List<GameObject> myTargetsToHit = new List<GameObject>();

    [SerializeField]
    private GameObject myBurningDummyVFX = null;


    protected override bool StartTutorial()
    {
        if (!base.StartTutorial())
            return false;

        for (int index = 0; index < Players.Count; index++)
        {
            GameObject player = Players[index];
            player.GetComponent<PlayerCastingComponent>().myEventOnSpellSpawned += OnSpellSpawned;
            myTutorialPanel.SetSpellData(player.GetComponent<Class>().mySpells[mySpellIndexToCast].GetComponent<Spell>(), player.GetComponent<Resource>(), index);
        }

        for (int index = 0; index < Players.Count; index++)
        {
            PlayerTargetingComponent targetingComponent = Players[index].GetComponent<PlayerTargetingComponent>();
            if (targetingComponent.Target != null && targetingComponent.Target.tag == "Enemy")
            {
                targetingComponent.SetTarget(null);
                break;
            }
        }

        myTargetHandler.ClearAllEnemies();
        for (int index = 0; index < myTargetsToHit.Count; index++)
        {
            myTargetHandler.AddEnemy(myTargetsToHit[index], true);
        }

        myTutorialPanel.SetSpellsHightlight(true, mySpellIndexToCast > 3);
        if (mySpellIndexToCast == 0)
            myTutorialPanel.SetErrorHightlight(true);

        return true;
    }

    public void OnSpellSpawned(GameObject aPlayer, GameObject aSpell, int aSpellIndex)
    {
        if (aSpellIndex != mySpellIndexToCast)
            return;

        if (myCompletedPlayers.Contains(aPlayer))
            return;

        myCompletedPlayers.Add(aPlayer);
        SetPlayerCompleted(aPlayer);
        if (myCompletedPlayers.Count == Players.Count)
        {
            foreach (GameObject player in Players)
            {
                PlayerCastingComponent character = player.GetComponent<PlayerCastingComponent>();
                character.myEventOnSpellSpawned -= OnSpellSpawned;
            }

            for (int index = 0; index < myTargetsToHit.Count; index++)
            {
                GameObject fireVFX = Instantiate(myBurningDummyVFX, myTargetsToHit[index].transform);
                fireVFX.transform.position += Vector3.up;
                fireVFX.transform.rotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
            }
            myTutorialPanel.SetSpellsHightlight(false, mySpellIndexToCast > 3);
            if (mySpellIndexToCast == 0)
                myTutorialPanel.SetErrorHightlight(true);

            DeactivateTargetDummies();
            EndTutorial();
        }
    }

    public void ActivateTargetDummies()
    {
        for (int index = 0; index < myTargetsToHit.Count; index++)
        {
            myTargetsToHit[index].GetComponent<BehaviorTree>().enabled = true;
            myTargetsToHit[index].GetComponent<BehaviorTree>().SendEvent("Activate");
        }
    }

    public void DeactivateTargetDummies()
    {
        for (int index = 0; index < myTargetsToHit.Count; index++)
        {
            myTargetsToHit[index].GetComponent<BehaviorTree>().SendEvent("Deactivate");
            myTargetsToHit[index].GetComponent<BehaviorTree>().enabled = false;
        }
    }
}
