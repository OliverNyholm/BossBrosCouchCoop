using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpellErrorHandler : MonoBehaviour
{
    [Header("Image on HUD to show icons on")]
    [SerializeField]
    private Image mySpellErrorIcon = null;

    [Header("Durations")]
    [SerializeField]
    private float myHightlightDuration = 0.4f;
    [SerializeField]
    private float myFadeDuration = 0.6f;

    public float mySizeMultiplier = 1.2f;

    [Header("Error Icons")]
    public Sprite myNoVisionIcon = null;
    public Sprite myOutOfResourcesIcon = null;
    public Sprite myWrongTargetEnemyIcon = null;
    public Sprite myWrongTargetPlayerIcon = null;
    public Sprite myNoTargetIcon = null;
    public Sprite myOutOfRangeIcon = null;
    public Sprite myCantMoveWhileCastingIcon = null;
    public Sprite myAlreadyCastingIcon = null;
    public Sprite myCooldownIcon = null;
    public Sprite myNotDeadIcon = null;
    public Sprite myIsDeadIcon = null;
    public Sprite myIsNotAvailableIcon = null;
    public Sprite myIsNotSelfCastIcon = null;

    private TextMeshProUGUI myErrorText = null;
    private Color myErrorTextColor;

    private Coroutine myHighlightErrorIconRoutine;
    private Coroutine myHighlightErrorTextRoutine;

    private void Awake()
    {
        myErrorText = GetComponent<TextMeshProUGUI>();
        myErrorTextColor = myErrorText.color;
        myErrorText.color = new Color(0f, 0f, 0f, 0f);
    }

    public enum SpellError
    {
        NoVision,
        OutOfResources,
        WrongTargetEnemy,
        WrongTargetPlayer,
        NoTarget,
        OutOfRange,
        CantMoveWhileCasting,
        AlreadyCasting,
        Cooldown,
        NotDead,
        IsDead,
        NotAvailable,
        NotSelfCast
    };

    public void HighLightError(SpellError aSpellError)
    {
        switch (aSpellError)
        {
            case SpellError.NoVision:
                mySpellErrorIcon.sprite = myNoVisionIcon;
                myErrorText.text = "Target not in line of sight!";
                break;
            case SpellError.OutOfResources:
                mySpellErrorIcon.sprite = myOutOfResourcesIcon;
                myErrorText.text = "Not enough resource to cast";
                break;
            case SpellError.WrongTargetEnemy:
                mySpellErrorIcon.sprite = myWrongTargetEnemyIcon;
                myErrorText.text = "Can't cast friendly spells on enemies";
                break;
            case SpellError.WrongTargetPlayer:
                mySpellErrorIcon.sprite = myWrongTargetPlayerIcon;
                myErrorText.text = "Can't cast hostile spells on friends.";
                break;
            case SpellError.NoTarget:
                mySpellErrorIcon.sprite = myNoTargetIcon;
                myErrorText.text = "No Target!";
                break;
            case SpellError.OutOfRange:
                mySpellErrorIcon.sprite = myOutOfRangeIcon;
                myErrorText.text = "Out of range!";
                break;
            case SpellError.CantMoveWhileCasting:
                mySpellErrorIcon.sprite = myCantMoveWhileCastingIcon;
                myErrorText.text = "Can't cast while moving!";
                break;
            case SpellError.AlreadyCasting:
                mySpellErrorIcon.sprite = myAlreadyCastingIcon;
                myErrorText.text = "Already casting another spell!";
                break;
            case SpellError.Cooldown:
                mySpellErrorIcon.sprite = myCooldownIcon;
                myErrorText.text = "Can't cast that spell yet";
                break;
            case SpellError.NotDead:
                mySpellErrorIcon.sprite = myNotDeadIcon;
                myErrorText.text = "That target is not dead!";
                break;
            case SpellError.IsDead:
                mySpellErrorIcon.sprite = myIsDeadIcon;
                myErrorText.text = "Can't cast spell on dead target!";
                break;
            case SpellError.NotAvailable:
                mySpellErrorIcon.sprite = myIsNotAvailableIcon;
                myErrorText.text = "Spell not available!";
                break;
            case SpellError.NotSelfCast:
                mySpellErrorIcon.sprite = myIsNotSelfCastIcon;
                myErrorText.text = "Can't be cast on self!";
                break;
            default:
                break;
        }

        mySpellErrorIcon.enabled = true;
        mySpellErrorIcon.color = Color.white;

        if (myHighlightErrorIconRoutine != null)
            StopCoroutine(myHighlightErrorIconRoutine);

        if (myHighlightErrorTextRoutine != null)
            StopCoroutine(myHighlightErrorTextRoutine);

        if (myHighlightErrorIconRoutine != null || myHighlightErrorTextRoutine != null)
            StopAllCoroutines();

        myHighlightErrorIconRoutine = StartCoroutine(HighightErrorIconRoutine());
        myHighlightErrorTextRoutine = StartCoroutine(HighightErrorTextRoutine());
    }

    private IEnumerator HighightErrorIconRoutine()
    {
        float duration = myHightlightDuration;

        RectTransform spellRect = mySpellErrorIcon.GetComponent<RectTransform>();
        spellRect.localScale = Vector3.one * mySizeMultiplier;

        while (duration > 0)
        {
            duration -= Time.deltaTime;

            float interpolation = 1.0f - (duration / myHightlightDuration);

            float multiplier = Mathf.Lerp(mySizeMultiplier, 1.0f, interpolation);
            spellRect.localScale = Vector3.one * multiplier;

            yield return null;
        }

        duration = myFadeDuration;

        while (duration > 0)
        {
            duration -= Time.deltaTime;

            Color color = mySpellErrorIcon.color;
            color.a = duration / myFadeDuration;
            mySpellErrorIcon.color = color;

            yield return null;
        }

        mySpellErrorIcon.enabled = false;
    }

    private IEnumerator HighightErrorTextRoutine()
    {
        myErrorText.color = myErrorTextColor;

        float showDuration = 0.8f;

        while (showDuration > 0)
        {
            showDuration -= Time.deltaTime;
            yield return null;
        }

        float fadeDuration = 0.5f;

        while (fadeDuration > 0)
        {
            fadeDuration -= Time.deltaTime;

            Color color = myErrorText.color;
            color.a = fadeDuration / myFadeDuration;
            myErrorText.color = color;

            yield return null;
        }

        mySpellErrorIcon.enabled = false;
    }
}
