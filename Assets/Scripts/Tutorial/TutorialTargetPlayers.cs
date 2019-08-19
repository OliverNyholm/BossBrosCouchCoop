using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTargetPlayers : TutorialCompletion
{
    protected override bool StartTutorial()
    {
        if (!base.StartTutorial())
            return false;

        for (int index = 0; index < myPlayers.Count; index++)
        {
            GameObject player = myPlayers[index];
            player.GetComponent<Player>().myEventOnTargetPlayer += OnTargetPlayer;
        }

        return true;
    }

    public void OnTargetPlayer(GameObject aPlayer)
    {
        if (myCompletedPlayers.Contains(aPlayer))
            return;

        myCompletedPlayers.Add(aPlayer);
        SetPlayerCompleted(aPlayer);
        if (myCompletedPlayers.Count == myPlayers.Count)
        {
            foreach (GameObject player in myPlayers)
            {
                Player playerScript = player.GetComponent<Player>();
                playerScript.myEventOnTargetPlayer -= OnTargetPlayer;
            }

            EndTutorial();
        }
    }
}
