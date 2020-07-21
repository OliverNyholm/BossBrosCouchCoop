using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ActionKey : MonoBehaviour
{
    [Header("Text to show cooldown")]
    [SerializeField]
    private TextMeshProUGUI myCooldownText = null;

    [Header("Text to show info about spell")]
    [SerializeField]
    private TextMeshProUGUI myNameText = null;
    [SerializeField]
    private TextMeshProUGUI myInfoText = null;

    [Header("The target scale multiplier of spellIcon on use")]
    [SerializeField]
    private float mySizeMultiplier = 1.1f;
    private Vector3 myLerpInitScale;
    private Vector3 myStartScale;

    [Header("The target color where the button will scale in size on use")]
    [SerializeField]
    private Color myTargetColor = Color.grey;
    private Color myStartColor;

    [SerializeField]
    private Color myPulseColor = Color.white;
    private bool myIsPulsing;

    private Image myImage;

    private Coroutine myCoroutine;

    public bool IsSpellOnCooldown { get; set; }

    private void Start()
    {
        myStartScale = GetComponent<RectTransform>().localScale;
        myLerpInitScale = myStartScale;

        myImage = GetComponent<Image>();
        myStartColor = myImage.color;
    }

    private void Update()
    {
        if (!myIsPulsing)
            return;

        float positiveSin = (Mathf.Sin(Time.time * 8.0f) + 1) * 0.5f;
        float lerpValue = Mathf.Clamp(positiveSin, 0.2f, 1.0f);
        myImage.color = Color.Lerp(myStartColor, myPulseColor, lerpValue);
    }

    public void SetCooldown(float aDuration)
    {
        if (aDuration > 0.0f)
        {
            if (aDuration > 2.0f)
                myCooldownText.text = aDuration.ToString("0");
            else
                myCooldownText.text = aDuration.ToString("0.0");

            myImage.color = new Color(0.8f, 0.3f, 0.3f, 0.5f);
            myStartColor = myImage.color;
            IsSpellOnCooldown = true;
        }
        else
        {
            myCooldownText.text = "";
            myImage.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            myStartColor = myImage.color;
            IsSpellOnCooldown = false;
        }
    }

    public void ToggleInfo()
    {
        myInfoText.enabled = !myInfoText.enabled;
    }

    public void SetSpellName(string aName)
    {
        myNameText.text = aName;
    }

    public void SetSpellInfo(string aInfo)
    {
        if(myInfoText)
            myInfoText.text = aInfo;
    }

    public void SetTextColor(Color aColor)
    {
        myNameText.color = aColor;
        if (myInfoText)
            myInfoText.color = aColor;
    }

    public void SetPulsation(bool aValue)
    {
        myIsPulsing = aValue;
    }

    public void SpellPressed()
    {
        if (myCoroutine != null)
            StopCoroutine(myCoroutine);

        myCoroutine = StartCoroutine(ButtonPressedCoroutine());
    }

    public void SpellHeldDown()
    {
        if (myCoroutine != null)
            StopCoroutine(myCoroutine);

        myCoroutine = StartCoroutine(ButtonSpellHeldDownCoroutine());
    }

    public void SpellReleased()
    {
        if (myCoroutine != null)
            StopCoroutine(myCoroutine);

        myCoroutine = StartCoroutine(ButtonSpellReleasedCoroutine());
    }

    private IEnumerator ButtonPressedCoroutine()
    {
        Image spellIcon = myImage;
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

    private IEnumerator ButtonSpellHeldDownCoroutine()
    {
        if (!IsSpellOnCooldown)
            myImage.color = myStartColor;

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
                myImage.color = Color.Lerp(myStartColor, myTargetColor, interpolation);

            myTimer -= Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator ButtonSpellReleasedCoroutine()
    {
        if (!IsSpellOnCooldown)
            myImage.color = myStartColor;

        RectTransform spellRect = GetComponent<RectTransform>();
        spellRect.localScale = myLerpInitScale;

        float myScaleDownDuration = 0.1f;
        float myTimer = myScaleDownDuration;

        while (myTimer > 0.0f)
        {
            float interpolation = 1.0f - (myTimer / myScaleDownDuration);

            float multiplier = Mathf.Lerp(mySizeMultiplier, 1.0f, interpolation);
            spellRect.localScale = myLerpInitScale * multiplier;

            if (!IsSpellOnCooldown)
                myImage.color = Color.Lerp(myTargetColor, myStartColor, interpolation);

            myTimer -= Time.deltaTime;
            yield return null;
        }

        if (!IsSpellOnCooldown)
            myImage.color = myStartColor;
        spellRect.localScale = myLerpInitScale;
    }
}
