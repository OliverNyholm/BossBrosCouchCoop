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

    [SerializeField]
    private Transform myNextTutorial = null;

    private List<GameObject> myCompletedPlayers = new List<GameObject>();


    protected override bool StartTutorial()
    {
        if (!base.StartTutorial())
            return false;

        if (mySpellIndexToCast < 7)
            myFinishRoutine = RaiseNextTutorialRoutine;

        for (int index = 0; index < myPlayers.Count; index++)
        {
            GameObject player = myPlayers[index];
            player.GetComponent<Character>().myEventOnSpellSpawned += OnSpellSpawned;
            myTutorialPanel.SetSpellData(player.GetComponent<Class>().mySpells[mySpellIndexToCast].GetComponent<Spell>(), player.GetComponent<Resource>(), index);
        }

        for (int index = 0; index < myTargetsToHit.Count; index++)
        {
            myTargetHandler.AddEnemy(myTargetsToHit[index]);
        }

        myTutorialPanel.SetSpellsHightlight(true, mySpellIndexToCast > 3);

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
        if (myCompletedPlayers.Count == myPlayers.Count)
        {
            foreach (GameObject player in myPlayers)
            {
                Character character = player.GetComponent<Character>();
                character.myEventOnSpellSpawned -= OnSpellSpawned;

                for (int targetIndex = 0; targetIndex < myTargetsToHit.Count; targetIndex++)
                {
                    if (character.Target == myTargetsToHit[targetIndex])
                    {
                        player.GetComponent<Character>().SetTarget(null);
                        break;
                    }
                }
            }

            for (int index = 0; index < myTargetsToHit.Count; index++)
            {
                myTargetHandler.RemoveEnemy(myTargetsToHit[index]);

                GameObject fireVFX = Instantiate(myBurningDummyVFX, myTargetsToHit[index].transform);
                fireVFX.transform.position += Vector3.up;
                fireVFX.transform.rotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
            }
            myTutorialPanel.SetSpellsHightlight(false, mySpellIndexToCast > 3);
            EndTutorial();
            DeactivateTargetDummies();
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

    private IEnumerator RaiseNextTutorialRoutine()
    {
        Vector3 targetOffset = new Vector3(0.0f, 20.0f, 0.0f);
        Vector3 startPosition = myNextTutorial.position;
        Vector3 endPosition = startPosition + targetOffset;
        float duration = 4.0f;
        float timer = duration;

        while (timer > 0.0f)
        {
            timer -= Time.deltaTime;

            float interpolation = 1.0f - (timer / duration);
            myNextTutorial.position = Vector3.Lerp(startPosition, endPosition, interpolation);
            transform.position = Vector3.Lerp(startPosition + targetOffset, endPosition + targetOffset, interpolation);

            yield return null;
        }

        myNextTutorial.GetComponent<TutorialLearnSpell>().ActivateTargetDummies();
    }
}
