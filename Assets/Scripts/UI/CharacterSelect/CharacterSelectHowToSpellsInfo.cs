using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CharacterSelectHowToSpellsInfo : MonoBehaviour
{
    [Header("Show Durations")]
    [SerializeField]
    private float myDelayTime = 3.0f;
    [SerializeField]
    private float myShowTime = 8.0f;

    [Header("Animation speeds")]
    [SerializeField]
    private float myZoomInDuration = 0.7f;
    [SerializeField]
    private float myZoomOutDuration = 0.2f;

    private void Awake()
    {
        CharacterGameData gameData = FindObjectOfType<CharacterGameData>();
        if (gameData.myCurrentLevelIndex > 3)
        {
            gameObject.SetActive(false);
            return;
        }

        gameObject.transform.localScale = Vector3.zero;
        gameObject.GetComponent<TextMeshProUGUI>().enabled = true;
        LeanTween.scale(gameObject, Vector3.one, myZoomInDuration).setOnComplete(OnZoomedIn).setEase(LeanTweenType.easeOutQuart).setDelay(myDelayTime);
    }

    private void OnZoomedIn()
    {
        LeanTween.scale(gameObject, Vector3.zero, myZoomOutDuration).setOnComplete(OnZoomedOut).setEase(LeanTweenType.easeOutQuart).setDelay(myShowTime);
    }

    private void OnZoomedOut()
    {
        gameObject.SetActive(false);
    }
}
