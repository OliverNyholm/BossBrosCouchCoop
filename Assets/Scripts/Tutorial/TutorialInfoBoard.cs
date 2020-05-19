using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialInfoBoard : MonoBehaviour
{
    [Header("Data to show on tutorial UI")]
    [SerializeField]
    [TextArea(3, 6)]
    private string myTutorialText = "No tutorial text set!";

    [SerializeField]
    private List<TutorialPanelImage> myTutorialImages = new List<TutorialPanelImage>();

    private int myPlayersByBoardCount = 0;
    private TutorialPanelManager myTutorialPanelManager;

    private void Awake()
    {
        myTutorialPanelManager = FindObjectOfType<TutorialPanelManager>();
    }

    private void Update()
    {
        if (myPlayersByBoardCount == 0)
            return;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.GetComponent<PlayerMovementComponent>())
            return;

        other.GetComponent<PlayerCastingComponent>().enabled = false;

        myPlayersByBoardCount++;
        if(myPlayersByBoardCount == 1)
        {
            myTutorialPanelManager.GetComponent<Canvas>().enabled = true;
            myTutorialPanelManager.SetTutorialPanel(myTutorialImages, myTutorialText);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.GetComponent<PlayerMovementComponent>())
            return;

        other.GetComponent<PlayerCastingComponent>().enabled = true;

        myPlayersByBoardCount--;
        if(myPlayersByBoardCount == 0)
            myTutorialPanelManager.GetComponent<Canvas>().enabled = false;
    }
}
