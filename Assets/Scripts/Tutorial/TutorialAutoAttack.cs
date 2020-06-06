using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialAutoAttack : TutorialCompletion
{
    [SerializeField]
    private GameObject myBirchToHit = null;
    [SerializeField]
    private BoxCollider myColliderToDisable = null;
    [SerializeField]
    private GameObject myBirchMesh = null;
    [SerializeField]
    private ParticleSystem myBirchDestroyedVFX = null;

    protected override bool StartTutorial()
    {
        if (!base.StartTutorial())
            return false;

        myTargetHandler.AddEnemy(myBirchToHit, true);
        myBirchToHit.GetComponent<Health>().EventOnHealthZero += OnTargetDied;

        return true;
    }

    private void OnTargetDied()
    {
        myBirchToHit.GetComponent<Health>().EventOnHealthZero -= OnTargetDied;
        myBirchMesh.SetActive(false);
        myColliderToDisable.enabled = false;
        myTargetHandler.RemoveEnemy(myBirchToHit);
        myBirchDestroyedVFX.Play();
        EndTutorial();
    }

    IEnumerator RemoveHudEnumerator()
    {
        float duration = 0.5f;
        while(duration > 0.0f)
        {
            duration -= Time.deltaTime;
            yield return null;
        }

        myTargetHandler.RemoveEnemy(myBirchToHit);
    }
}
