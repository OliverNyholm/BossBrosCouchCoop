using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControlsVibrationManager : MonoBehaviour
{
    PlayerControls myPlayerControls = null;

    Coroutine myCurrentVibrationCoroutine = null;

    public void SetPlayerControls(PlayerControls aPlayerControls)
    {
        myPlayerControls = aPlayerControls;
    }

    private void OnDisable()
    {
        StopVibrations();
    }

    public void VibratePlayerCastingError(SpellErrorHandler.SpellError aSpellError)
    {
        StopVibrations();

        switch (aSpellError)
        {
            case SpellErrorHandler.SpellError.WrongTargetEnemy:
            case SpellErrorHandler.SpellError.WrongTargetPlayer:
            case SpellErrorHandler.SpellError.NoTarget:
            case SpellErrorHandler.SpellError.NotDead:
            case SpellErrorHandler.SpellError.IsDead:
            case SpellErrorHandler.SpellError.NotSelfCast:
                myCurrentVibrationCoroutine = StartCoroutine(LongVibration());
                break;
            case SpellErrorHandler.SpellError.NoVision:
            case SpellErrorHandler.SpellError.OutOfRange:
                myCurrentVibrationCoroutine = StartCoroutine(OneSmallOneBig());
                break;
            case SpellErrorHandler.SpellError.OutOfResources:
            case SpellErrorHandler.SpellError.CantMoveWhileCasting:
            case SpellErrorHandler.SpellError.AlreadyCasting:
            case SpellErrorHandler.SpellError.Cooldown:
                myCurrentVibrationCoroutine = StartCoroutine(QuickVibration());
                break;
            default:
                break;
        }

    }

    IEnumerator QuickVibration()
    {
        Debug.Log("Quick Vibration");
        const float duration = 0.05f;
        float timeStamp = Time.time;

        myPlayerControls.Vibrate(40);

        while(Time.time - timeStamp < duration)
        {
            yield return null;
        }

        myPlayerControls.StopVibrating();
    }

    IEnumerator LongVibration()
    {
        Debug.Log("Long Vibration");
        const float duration = 0.15f;
        float timeStamp = Time.time;

        myPlayerControls.Vibrate(60);

        while (Time.time - timeStamp < duration)
        {
            yield return null;
        }

        myPlayerControls.StopVibrating();
    }

    IEnumerator OneSmallOneBig()
    {
        Debug.Log("OneSmallOneBig Vibration");
        float duration = 0.05f;
        float timeStamp = Time.time;
        myPlayerControls.Vibrate(60);

        while (Time.time - timeStamp < duration)
            yield return null;

        myPlayerControls.StopVibrating();

        duration = 0.1f;
        timeStamp = Time.time;
        while (Time.time - timeStamp < duration)
            yield return null;

        duration = 0.05f;
        timeStamp = Time.time;
        myPlayerControls.Vibrate(500);

        while (Time.time - timeStamp < duration)
            yield return null;

        myPlayerControls.StopVibrating();
    }

    private void StopVibrations()
    {
        if (myCurrentVibrationCoroutine != null)
            StopCoroutine(myCurrentVibrationCoroutine);

        myPlayerControls.StopVibrating();
        myCurrentVibrationCoroutine = null;
    }
}
