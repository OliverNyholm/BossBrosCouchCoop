﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossTimer : MonoBehaviour
{
    [SerializeField]
    private Image myIcon = null;
    [SerializeField]
    private Image myTimerBar = null;
    [SerializeField]
    private Text myAttackName = null;
    [SerializeField]
    private Text myAttackTimer = null;

    private float myStartTimer;
    private float myTimer;

    public void SetData(string aName, float aDuration, Sprite aSprite, Color aColor)
    {
        myAttackName.text = aName;
        myIcon.sprite = aSprite;
        myAttackTimer.text = aDuration.ToString("0.0");
        myTimerBar.fillAmount = 1.0f;
        myTimerBar.color = aColor;

        myStartTimer = myTimer = aDuration;
    }

    public void Update()
    {
        myTimer -= Time.deltaTime;

        myTimerBar.fillAmount = myTimer / myStartTimer;
        myAttackTimer.text = myTimer.ToString("0.0");
    }
}