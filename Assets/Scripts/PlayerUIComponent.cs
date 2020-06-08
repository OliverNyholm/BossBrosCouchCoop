using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIComponent : UIComponent
{
    private GameObject myActionBar;
    private GameObject[] myActionButtons;

    private SpellErrorHandler mySpellErrorHandler;

    [SerializeField]
    private bool myUseCastbarAbovePlayer = false;

    [SerializeField]
    private Sprite myNoSpellSprite = null;

    protected override void Awake()
    {
        base.Awake();

        myActionButtons = new GameObject[GetComponent<Class>().mySpellSize];
    }

    void Start()
    {
        Transform uiHud = GameObject.Find("PlayerHud" + GetComponent<Player>().PlayerIndex).transform;
        SetupHud(uiHud);

        mySpellErrorHandler = uiHud.GetComponentInChildren<SpellErrorHandler>();
    }

    public override void SetupHud(Transform aUIParent)
    {
        base.SetupHud(aUIParent);

        if (myUseCastbarAbovePlayer)
        {
            myCastbar = GetComponentInChildren<Castbar>();
            myCastbar.GetComponent<Canvas>().worldCamera = Camera.main;
        }

        GetComponentInChildren<TargetProjector>().SetPlayerColor(myCharacterColor);

        myActionBar = aUIParent.Find("ActionBar").gameObject;
        myActionBar.GetComponent<CanvasGroup>().alpha = 1.0f;
    }

    public void SetSpellCooldownText(int anIndex, float aDuration)
    {
        if (myActionButtons[anIndex] == null)
            return;

        myActionButtons[anIndex].GetComponent<ActionKey>().SetCooldown(aDuration);
    }

    public void SpellPressed(int anIndex)
    {
        myActionButtons[anIndex].GetComponent<ActionKey>().SpellPressed();
    }

    public void SpellHeldDown(int anIndex)
    {
        myActionButtons[anIndex].GetComponent<ActionKey>().SpellHeldDown();
    }

    public void SpellReleased(int anIndex)
    {
        myActionButtons[anIndex].GetComponent<ActionKey>().SpellReleased();
    }

    public void HightlightHealTargeting(int anIndex, bool aShouldPulsate)
    {
        myActionButtons[anIndex].GetComponent<ActionKey>().SetPulsation(aShouldPulsate);
    }

    public void ToggleSpellInfo()
    {
        for (int index = 0; index < myActionButtons.Length; index++)
        {
            myActionButtons[index].GetComponent<ActionKey>().ToggleInfo();
        }
    }

    public void SetSpellHud(Spell aSpell, int anIndex)
    {
        myActionButtons[anIndex] = myActionBar.transform.GetChild(anIndex).gameObject;
        if (aSpell == null)
        {
            myActionButtons[anIndex].GetComponent<Image>().sprite = myNoSpellSprite;
            myActionButtons[anIndex].GetComponent<ActionKey>().SetSpellInfo("");
        }
        else
        {
            myActionButtons[anIndex].GetComponent<Image>().sprite = aSpell.mySpellIcon;
            myActionButtons[anIndex].GetComponent<ActionKey>().SetSpellInfo(aSpell.myQuickInfo);

            Vector3 inversePosition = myActionButtons[anIndex].GetComponentInParent<Canvas>().transform.InverseTransformPoint(myActionButtons[anIndex].transform.position);
            FindObjectOfType<TutorialHighlightManager>().HighlightArea(inversePosition, Vector3.one * 0.4f);
        }
    }

    public void HighlightSpellError(SpellErrorHandler.SpellError aSpellError)
    {
        mySpellErrorHandler.HighLightError(aSpellError);
    }
}
