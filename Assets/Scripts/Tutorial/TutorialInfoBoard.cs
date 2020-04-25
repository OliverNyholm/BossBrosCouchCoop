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
    private Sprite myTutorialImageSprite = null;

    private int myPlayersByBoardCount = 0;
    private TutorialPanel myTutorialPanel;

    private void Awake()
    {
        myTutorialPanel = FindObjectOfType<TutorialPanel>();
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

        myPlayersByBoardCount++;
        if(myPlayersByBoardCount == 1)
        {
            myTutorialPanel.gameObject.SetActive(true);
            myTutorialPanel.SetData(myTutorialText, myTutorialImageSprite);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.GetComponent<PlayerMovementComponent>())
            return;

        myPlayersByBoardCount--;
        if(myPlayersByBoardCount == 0)
            myTutorialPanel.gameObject.SetActive(false);
    }
}
