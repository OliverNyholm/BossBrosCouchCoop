using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialDamaging : TutorialCompletion
{
    [SerializeField]
    private GameObject myDamageSpell = null;

    [Header("The targets to kill")]
    [SerializeField]
    private GameObject myGroundTarget = null;
    [SerializeField]
    private GameObject myRangeTarget = null;

    int myKilledCount = 0;

    protected override void Awake()
    {
        base.Awake();

        Spell spell = myDamageSpell.GetComponent<Spell>();
        spell.CreatePooledObjects(PoolManager.Instance, 6);
    }

    protected override bool StartTutorial()
    {
        if (!base.StartTutorial())
            return false;

        foreach (GameObject player in Players)
            player.GetComponent<Class>().ReplaceSpell(myDamageSpell, 0);

        myTargetHandler.AddEnemy(myGroundTarget, true);
        myTargetHandler.AddEnemy(myRangeTarget, true);

        myGroundTarget.GetComponent<Health>().EventOnHealthZero += OnTargetDied;
        myRangeTarget.GetComponent<Health>().EventOnHealthZero += OnTargetDied;

        return true;
    }

    private void OnTargetDied()
    {
        myKilledCount++;
        if(myKilledCount == 2)
        {
            myTargetHandler.RemoveEnemy(myGroundTarget);
            myTargetHandler.RemoveEnemy(myRangeTarget);
            EndTutorial();
        }
    }
}
