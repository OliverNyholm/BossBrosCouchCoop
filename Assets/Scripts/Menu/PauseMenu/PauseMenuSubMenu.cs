using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PauseMenuSubMenu : MonoBehaviour
{
    protected PlayerControls myPlayerControls;
    protected PlayerControlsJoystickMovedHelper myPlayerControlsMovement;

    protected PauseMenu myBaseMenu;
    protected PauseMenuSubMenu myPreviousMenu;

    private Canvas myCanvas;

    protected virtual void Update()
    {
        if (!IsOpen())
            return;

        if(myPlayerControlsMovement != null)
            myPlayerControlsMovement.Update();
    }

    public void PreviousMenu(PauseMenuSubMenu aSubMenu)
    {
        myPreviousMenu = aSubMenu;
    }

    public void OnPauseMenuOpened(PauseMenu aBaseMenu, PlayerControls aPlayerControls)
    {
        myBaseMenu = aBaseMenu;

        myCanvas = GetComponent<Canvas>();
        myCanvas.enabled = false;

        myPlayerControls = aPlayerControls;
        myPlayerControlsMovement = new PlayerControlsJoystickMovedHelper(aPlayerControls, 0.7f, 0.2f);
    }

    protected bool BackPressed()
    {
        return myPlayerControls.Action2.WasPressed || myPlayerControls.Action3.WasPressed || myPlayerControls.TargetEnemy.WasPressed;
    }

    public virtual void Open()
    {
        myCanvas.enabled = true;
    }

    public virtual void Close()
    {
        myCanvas.enabled = false;
    }

    public bool IsOpen()
    {
        return myCanvas && myCanvas.enabled;
    }
}
