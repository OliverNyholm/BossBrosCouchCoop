using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialHighlightManager : MonoBehaviour
{
    public List<TutorialHighlightCircle> myHighlightCircles;
    private Camera myCamera;
    private Canvas myCanvas;
    private CanvasScaler myCanvasScaler;

    private void Awake()
    {
        myCamera = Camera.main;
        myCanvas = GetComponent<Canvas>();
        myCanvasScaler = GetComponent<CanvasScaler>();
    }

    public void HighlightArea(GameObject aGameObject, Vector3 aTargetScale)
    {
        for (int index = 0; index < myHighlightCircles.Count; index++)
        {
            if (myHighlightCircles[index].myIsRunning)
                continue;

            myHighlightCircles[index].HighlightArea(aGameObject, aTargetScale);
            break;
        }
    }

    public void HighlightArea(Vector3 aLocation, Vector3 aTargetScale)
    {
        for (int index = 0; index < myHighlightCircles.Count; index++)
        {
            if (myHighlightCircles[index].myIsRunning)
                continue;

            myHighlightCircles[index].HighlightArea(aLocation, aTargetScale);
            break;
        }
    }
}