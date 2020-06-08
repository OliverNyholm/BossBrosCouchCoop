using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialHighlightCircle : MonoBehaviour
{
    public float myZoomOutDuration = 0.5f;
    public float myDelay = 1.0f;
    public float myZoomInDuration = 1.0f;

    public bool myIsRunning = false;

    private Image myImage;

    private void Awake()
    {
        myImage = GetComponent<Image>();
        myImage.enabled = false;
    }

    public void HighlightArea(Vector3 aPosition, Vector3 aTargetScale)
    {
        myIsRunning = true;

        myImage.enabled = true;
        transform.localPosition = aPosition;
        transform.localScale = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        LeanTween.scale(gameObject, aTargetScale, myZoomOutDuration).setOnComplete(ScaleDown).setEase(LeanTweenType.easeInOutCubic);
        LeanTween.rotateZ(gameObject, 180.0f, myZoomOutDuration + myZoomInDuration + myDelay).setEase(LeanTweenType.easeInOutQuad);
    }

    private void ScaleDown()
    {
        LeanTween.scale(gameObject, Vector3.zero, myZoomInDuration).setOnComplete(OnComplete).setEase(LeanTweenType.easeOutQuart).setDelay(myDelay);
    }

    public void OnComplete()
    {
        myImage.enabled = false;
        myIsRunning = false;
    }
}
