using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialBoulder : MonoBehaviour
{
    private Quaternion myStartRotation;
    private Quaternion myTargetRotation;

    private float myInterpolation = 0.0f;
    private Coroutine myNudgeRoutine = null;

    public Vector3 myNudgeAmount = new Vector3(7.0f, 0.0f, 0.0f);
    public float myNudgeAwayTimer = 0.4f;
    public float myNudgeBackTimer = 0.2f;
    

    private void Awake()
    {
        myStartRotation = transform.rotation;
        myTargetRotation = transform.rotation * Quaternion.Euler(myNudgeAmount);
    }

    public void EnableBoulder()
    {
        GetComponentInParent<Health>().EventOnHealthChange += OnHealthChange;
    }

    public void DisableBoulder()
    {
        if (myNudgeRoutine != null)
            StopCoroutine(myNudgeRoutine);

        GetComponentInParent<Health>().EventOnHealthChange -= OnHealthChange;
    }

    private void OnHealthChange(float aHealthPercentage, string aHealthText, int aShieldValue, bool aIsDamage)
    {
        if (!aIsDamage)
            return;

        if (myNudgeRoutine != null)
            StopCoroutine(myNudgeRoutine);

        myNudgeRoutine = StartCoroutine(NudgeBoulder());
    }

    private IEnumerator NudgeBoulder()
    {
        float durationToEndRotationTimer = myNudgeAwayTimer - myInterpolation * myNudgeAwayTimer;

        while(durationToEndRotationTimer > 0.0f)
        {
            durationToEndRotationTimer -= Time.deltaTime;
            myInterpolation = 1.0f - durationToEndRotationTimer / myNudgeAwayTimer;

            transform.rotation = Quaternion.Slerp(myStartRotation, myTargetRotation, myInterpolation);
            yield return null;
        }

        float durationToStartRotationTimer = 0.0f;
        while (durationToStartRotationTimer < myNudgeBackTimer)
        {
            durationToStartRotationTimer += Time.deltaTime;
            myInterpolation = 1.0f - durationToStartRotationTimer / myNudgeBackTimer;

            transform.rotation = Quaternion.Slerp(myStartRotation, myTargetRotation, myInterpolation);
            yield return null;
        }
    }
}
