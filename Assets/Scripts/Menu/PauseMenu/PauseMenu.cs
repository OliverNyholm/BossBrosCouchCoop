using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool ourIsGamePaused = false;
    private bool myIsInTutorial = false;

    private PlayerControls myPlayerControls = null;
    private GameManager myGameManager = null;

    [Header("Screen to show when pause is opened")]
    [SerializeField]
    private List<PauseMenuSubMenu> myPauseSubMenues = new List<PauseMenuSubMenu>(4);
    private PauseMenuSubMenu myCurrentOpenSubMenu = null;

    private void Awake()
    {
        if (FindObjectOfType<TutorialEndLevel>())
            myIsInTutorial = true;
    }

    private void Update()
    {
        if (!ourIsGamePaused || myPlayerControls == null)
            return;

        if (myPlayerControls.Pause.WasPressed)
            Resume();
    }

    private void OnDestroy()
    {
        Resume();
    }

    public void Pause(GameManager aGameManager, PlayerControls aPlayerControls)
    {
        Time.timeScale = 0.0f;
        ourIsGamePaused = true;

        myGameManager = aGameManager;

        myPlayerControls = aPlayerControls;
        foreach (PauseMenuSubMenu subMenu in myPauseSubMenues)
            subMenu.OnPauseMenuOpened(this, aPlayerControls);

        OpenSubmenu(0);
    }

    public void Resume()
    {
        if (myCurrentOpenSubMenu)
            myCurrentOpenSubMenu.Close();

        myPlayerControls = null;

        Time.timeScale = 1.0f;
        ourIsGamePaused = false;

        gameObject.SetActive(false);
    }

    public void RestartLevel()
    {
        myGameManager.RestartLevel();
    }

    public void LoadCharacterSelect()
    {
        Resume();
        if (myIsInTutorial)
            SceneManager.LoadScene("LevelSelect");
        else
            SceneManager.LoadScene("CharacterSelect");
    }

    public void LoadLevelSelect()
    {
        SceneManager.LoadScene("LevelSelect");
    }

    public void OpenSubmenu(int aSubmenuIndex)
    {
        if (myCurrentOpenSubMenu && myCurrentOpenSubMenu.IsOpen())
            myCurrentOpenSubMenu.Close();

        myCurrentOpenSubMenu = myPauseSubMenues[aSubmenuIndex];
        myCurrentOpenSubMenu.Open();
    }

    public void CloseSubmenu()
    {
        myCurrentOpenSubMenu.Close();
        OpenSubmenu(0);
    }
}
