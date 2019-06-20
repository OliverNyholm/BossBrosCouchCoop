using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public static bool ourIsGamePaused = false;

    [Header("The parent gameobject to pause menu")]
    [SerializeField]
    private GameObject myPauseObject = null;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown("Start"))
        {
            ourIsGamePaused = !ourIsGamePaused;
            if (ourIsGamePaused)
                Pause();
            else
                Resume();
        }
    }

    private void Pause()
    {
        Time.timeScale = 0.0f;
        myPauseObject.SetActive(true);

        List<GameObject> players = GameObject.FindObjectOfType<TargetHandler>().GetAllPlayers();

        for (int index = 0; index < myPauseObject.transform.childCount; index++)
        {
            if (players.Count <= index)
            {
                myPauseObject.transform.GetChild(index).gameObject.SetActive(false);
            }
            else
            {
                myPauseObject.transform.GetChild(index).gameObject.SetActive(true);
                myPauseObject.transform.GetChild(index).GetComponent<PausePlayerUI>().SetClassDetails(players[index].GetComponent<Class>());
            }
        }
    }

    private void Resume()
    {
        Time.timeScale = 1.0f;
        myPauseObject.SetActive(false);
    }
}
