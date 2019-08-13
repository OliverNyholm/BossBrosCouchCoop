using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialSpellInfo : TutorialCompletion
{
    private List<GameObject> myCompletedPlayers = new List<GameObject>();

    protected override bool StartTutorial()
    {
        if (!base.StartTutorial())
            return false;

        foreach (GameObject player in myPlayers)
        {
            player.GetComponent<Class>().myEventOnInfoToggle += OnInfoToggled;
        }

        myTutorialPanel.SetErrorHightlight(true);

        return true;
    }

    public void OnInfoToggled(GameObject aPlayer)
    {
        if (myCompletedPlayers.Contains(aPlayer))
            return;

        myCompletedPlayers.Add(aPlayer);
        SetPlayerCompleted(aPlayer);
        if (myCompletedPlayers.Count == myPlayers.Count)
        {
            foreach (GameObject player in myPlayers)
            {
                player.GetComponent<Class>().myEventOnInfoToggle -= OnInfoToggled;
            }

            myTutorialPanel.SetErrorHightlight(false);
            EndTutorial();
        }
    }
}
