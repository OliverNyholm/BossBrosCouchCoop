using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ActionKey : MonoBehaviour
{
    [SerializeField]
    private Button myButton;

    [Header("Text to show cooldown")]
    [SerializeField]
    private Text myCooldownText;

    [Header("Text to show info about spell")]
    [SerializeField]
    private Text myInfoText;

    [Header("The target scale multiplier of spellIcon on use")]
    [SerializeField]
    private float mySizeMultiplier = 1.1f;
    private Vector3 myLerpInitScale;

    [Header("The target scale multiplier of spellIcon when holding shift/trigger")]
    [SerializeField]
    private float myShiftMultiplier = 1.2f;
    private Vector3 myStartScale;

    [Header("The target size where the button will scale in size on use")]
    [SerializeField]
    private Color myTargetColor = Color.grey;
    private Color myStartColor;

    private Coroutine myCoroutine;

    public bool IsSpellOnCooldown { get; set; }

    private void Start()
    {
        myStartScale = GetComponent<RectTransform>().localScale;
        myLerpInitScale = myStartScale;

        myStartColor = GetComponent<Image>().color;
    }

    public void SetCooldown(float aDuration)
    {
        if (aDuration > 0.0f)
        {
            myCooldownText.text = aDuration.ToString("0.0");
            GetComponent<Image>().color = new Color(0.8f, 0.3f, 0.3f, 0.5f);
            IsSpellOnCooldown = true;
        }
        else
        {
            myCooldownText.text = "";
            GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            IsSpellOnCooldown = false;
        }
    }

    public void ToggleInfo()
    {
        myInfoText.enabled = !myInfoText.enabled;
    }

    public void SetSpellInfo(string aInfo)
    {
        myInfoText.text = aInfo;
    }

    public void SpellPressed()
    {
        if (myCoroutine != null)
            StopCoroutine(myCoroutine);

        myCoroutine = StartCoroutine(ButtonPressedCoroutine());
    }

    public void ShiftInteracted(bool aIsDown)
    {
        RectTransform spellRect = GetComponent<RectTransform>();
        if (aIsDown)
            spellRect.localScale = myStartScale * myShiftMultiplier;
        else
            spellRect.localScale = myStartScale;

        myLerpInitScale = spellRect.localScale;
    }

    private IEnumerator ButtonPressedCoroutine()
    {
        Image spellIcon = GetComponent<Image>();
        if (!IsSpellOnCooldown)
            spellIcon.color = myStartColor;

        RectTransform spellRect = GetComponent<RectTransform>();
        spellRect.localScale = myLerpInitScale;

        float myScaleUpDuration = 0.05f;
        float myTimer = myScaleUpDuration;

        while (myTimer > 0.0f)
        {
            float interpolation = 1.0f - (myTimer / myScaleUpDuration);

            float multiplier = Mathf.Lerp(1.0f, mySizeMultiplier, interpolation);
            spellRect.localScale = myLerpInitScale * multiplier;

            if (!IsSpellOnCooldown)
                spellIcon.color = Color.Lerp(myStartColor, myTargetColor, interpolation);

            myTimer -= Time.deltaTime;
            yield return null;
        }

        float myScaleDownDuration = 0.1f;
        myTimer = myScaleDownDuration;

        while (myTimer > 0.0f)
        {
            float interpolation = 1.0f - (myTimer / myScaleDownDuration);

            float multiplier = Mathf.Lerp(mySizeMultiplier, 1.0f, interpolation);
            spellRect.localScale = myLerpInitScale * multiplier;

            if (!IsSpellOnCooldown)
                spellIcon.color = Color.Lerp(myTargetColor, myStartColor, interpolation);

            myTimer -= Time.deltaTime;
            yield return null;
        }

        if (!IsSpellOnCooldown)
            spellIcon.color = myStartColor;
        spellRect.localScale = myLerpInitScale;
    }
}
