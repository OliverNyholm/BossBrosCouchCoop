using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuClassInfo : PauseMenuSubMenu
{
    [SerializeField]
    private List<PausePlayerUI> mySpellInfoUI = new List<PausePlayerUI>(4);

    protected override void Update()
    {
        base.Update();

        if (myPlayerControls == null|| !IsOpen())
            return;

        if (BackPressed())
            myBaseMenu.CloseSubmenu();
    }

    public override void Open()
    {
        base.Open();

        List<GameObject> players = FindObjectOfType<TargetHandler>().GetAllPlayers();
        for (int index = 0; index < players.Count; index++)
        {
            mySpellInfoUI[index].gameObject.SetActive(true);
            mySpellInfoUI[index].GetComponent<PausePlayerUI>().SetClassDetails(players[index].GetComponent<Class>());
        }
    }

    public override void Close()
    {
        base.Close();

        for (int index = 0; index < mySpellInfoUI.Count; index++)
            mySpellInfoUI[index].gameObject.SetActive(false);
    }
}
