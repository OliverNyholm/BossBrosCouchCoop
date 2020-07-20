﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public class Player : Character
{
    private PlayerControls myPlayerControls;
    private TargetHandler myTargetHandler;

    private SpellErrorHandler mySpellErrorHandler;


    public int PlayerIndex { get; set; }

    protected virtual void Start()
    {
        myTargetHandler = GameObject.Find("GameManager").GetComponent<TargetHandler>();

        Transform uiHud = GameObject.Find("PlayerHud" + PlayerIndex).transform;
        GetComponent<PlayerUIComponent>().SetupHud(uiHud);
    }

    protected override void Update()
    {
        if (PauseMenu.ourIsGamePaused)
            return;

        base.Update();

        DetectInput();
    }

    private void DetectInput()
    {

        if (myPlayerControls.ToggleInfo.WasPressed)
            GetComponent<PlayerUIComponent>().ToggleSpellInfo();

        if (myPlayerControls.ToggleUIText.WasPressed)
            GetComponent<UIComponent>().ToggleUIText(gameObject.GetInstanceID());

        if (myPlayerControls.Restart.WasPressed)
            FindObjectOfType<GameManager>().RestartLevel();
    }

    public void SetPosition(Vector3 aPosition)
    {
        transform.position = aPosition;
    }

    protected override void OnDeath()
    {
        base.OnDeath();

        PostMaster.Instance.PostMessage(new Message(MessageCategory.PlayerDied, gameObject.GetInstanceID()));
    }

    public void OnRevive()
    {
        GetComponent<AnimatorWrapper>().SetTrigger(SpellAnimationType.AutoAttack);
    }

    public int GetControllerIndex()
    {
        return PlayerIndex;
    }

    public void SetPlayerControls(PlayerControls aPlayerControls)
    {
        myPlayerControls = aPlayerControls;
        GetComponent<PlayerMovementComponent>().SetPlayerController(aPlayerControls);
        GetComponent<PlayerTargetingComponent>().SetPlayerController(aPlayerControls);
        GetComponent<PlayerCastingComponent>().SetPlayerController(aPlayerControls);
    }
}