﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;

public class TutorialHealWithCombat : TutorialCompletion
{
    [Header("The Totem")]
    [SerializeField]
    private GameObject myTotemBoss = null;
    [SerializeField]
    private List<ParticleSystem> myBurningEyes = new List<ParticleSystem>();

    [SerializeField]
    private Collider myStartFightCollider = null;

    private Subscriber mySubscriber;

    protected override bool StartTutorial()
    {
        if (!base.StartTutorial())
            return false;

        return true;
    }

    private void StartFight()
    {
        myTotemBoss.GetComponent<BehaviorTree>().enabled = true;
        myTotemBoss.GetComponent<BehaviorTree>().SendEvent("Activate");
        foreach (ParticleSystem burningEyes in myBurningEyes)
        {
            burningEyes.Clear();
            burningEyes.Play();
        }

        mySubscriber = new Subscriber();
        mySubscriber.EventOnReceivedMessage += ReceiveMessage;

        PostMaster.Instance.RegisterSubscriber(ref mySubscriber, MessageCategory.TutorialHealFightComplete);
        PostMaster.Instance.RegisterSubscriber(ref mySubscriber, MessageCategory.Wipe);
    }

    private void ReceiveMessage(Message aMessage)
    {
        PostMaster.Instance.UnregisterSubscriber(ref mySubscriber, MessageCategory.TutorialHealFightComplete);
        PostMaster.Instance.UnregisterSubscriber(ref mySubscriber, MessageCategory.Wipe);
        mySubscriber.EventOnReceivedMessage -= ReceiveMessage;

        myTotemBoss.GetComponent<BehaviorTree>().enabled = true;
        myTotemBoss.GetComponent<BehaviorTree>().SendEvent("Deactivate");

        foreach (ParticleSystem burningEyes in myBurningEyes)
            burningEyes.Stop();

        if (aMessage.Type == MessageCategory.TutorialHealFightComplete)
            EndTutorial();
        else if(aMessage.Type == MessageCategory.Wipe)
            Restart();
    }

    public override void OnChildTriggerEnter(Collider aChildCollider, Collider aHit)
    {
        if (aChildCollider == myStartCollider)
            StartTutorial();

        if (aChildCollider == myStartFightCollider)
        {
            myTutorialPanel.gameObject.SetActive(false);
            StartFight();
        }
    }
}