using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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

        const int playerMax = 4;
        for (int index = 0; index < playerMax; index++)
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

    public void LoadCharacterSelect()
    {
        ourIsGamePaused = false;
        Resume();
        SceneManager.LoadScene("CharacterSelect");
    }
}
