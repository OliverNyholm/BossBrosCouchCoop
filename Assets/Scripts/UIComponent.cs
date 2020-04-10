using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIComponent : MonoBehaviour
{
    [Header("Visuals used by character")]
    public Sprite myAvatarSprite;
    public Sprite myClassSprite;
    public Color myCharacterColor;
    public string myName;

    private SpellErrorHandler mySpellErrorHandler;

    private CharacterHUD myCharacterHUD;
    private CharacterHUD myTargetHUD;
    private Castbar myCastbar;

    private GameObject myActionBar;
    private GameObject[] myActionButtons;

    private Health myHealth;
    private Resource myResource;

    private GameObject myTargetGO;

    private void Awake()
    {
        myHealth = GetComponent<Health>();
        myResource = GetComponent<Resource>();
    }

    void Start()
    {
        Transform uiHud = GameObject.Find("PlayerHud" + GetComponent<Player>().PlayerIndex).transform;
        SetupHud(uiHud);

        mySpellErrorHandler = uiHud.GetComponentInChildren<SpellErrorHandler>();
    }

    public void SetupHud(Transform aUIParent)
    {
        aUIParent.GetComponent<CanvasGroup>().alpha = 1.0f;

        myCharacterHUD = aUIParent.Find("CharacterHud").GetComponent<CharacterHUD>();
        myTargetHUD = aUIParent.Find("TargetHud").GetComponent<CharacterHUD>();
        myCastbar = aUIParent.Find("Castbar Background").GetComponent<Castbar>();

        myCharacterHUD.SetName(myName);
        myCharacterHUD.SetClassSprite(myClassSprite);
        myCharacterHUD.SetAvatarSprite(myAvatarSprite);
        myCharacterHUD.SetHudColor(myCharacterColor);

        Health health = GetComponent<Health>();
        health.EventOnHealthChange += ChangeMyHudHealth;

        ChangeMyHudHealth(myHealth.GetHealthPercentage(), myHealth.myCurrentHealth.ToString() + "/" + myHealth.myMaxHealth.ToString(), GetComponent<Health>().GetTotalShieldValue(), false);

        if (myResource)
        {
            myResource.EventOnResourceChange += ChangeMyHudResource;
            ChangeMyHudResource(myResource.GetResourcePercentage(), myResource.myCurrentResource.ToString() + "/" + myResource.MaxResource.ToString());
        }

        Class playerClass = GetComponent<Class>();
        if (playerClass)
        {
            GetComponentInChildren<TargetProjector>().SetPlayerColor(myCharacterColor);

            myActionBar = aUIParent.Find("ActionBar").gameObject;
            myActionBar.GetComponent<CanvasGroup>().alpha = 1.0f;
            myActionButtons = new GameObject[playerClass.mySpellSize];
        }
    }

    public void SetCastbarChannelingStartValues(Spell aSpell, float aDuration)
    {
        myCastbar.ShowCastbar();
        myCastbar.SetCastbarFillAmount(1.0f);
        myCastbar.SetSpellName(aSpell.myName);
        myCastbar.SetCastbarColor(aSpell.myCastbarColor);
        myCastbar.SetSpellIcon(aSpell.mySpellIcon);
        myCastbar.SetCastTimeText(aDuration.ToString());
    }

    public void SetCastbarStartValues(Spell aSpell)
    {
        myCastbar.ShowCastbar();
        myCastbar.SetCastbarFillAmount(0.0f);
        myCastbar.SetSpellName(aSpell.myName);
        myCastbar.SetCastbarColor(aSpell.myCastbarColor);
        myCastbar.SetSpellIcon(aSpell.mySpellIcon);
        myCastbar.SetCastTimeText(aSpell.myCastTime.ToString());
    }

    public void SetCastbarStartValues(Spell aSpell, float aDuration)
    {
        myCastbar.ShowCastbar();
        myCastbar.SetCastbarFillAmount(1.0f);
        myCastbar.SetSpellName(aSpell.myName);
        myCastbar.SetCastbarColor(aSpell.myCastbarColor);
        myCastbar.SetSpellIcon(aSpell.mySpellIcon);
        myCastbar.SetCastTimeText(aDuration.ToString());
    }

    public void SetCastbarValues(float aFillAmount, string aText)
    {

    }

    public void SetCastbarInterrupted()
    {
        myCastbar.SetSpellName("Interrupted");
        myCastbar.FadeOutCastbar();
    }

    public void AddBuff(Sprite aBuffIcon)
    {
        myCharacterHUD.AddBuff(aBuffIcon);
    }

    public void RemoveBuff(int anIndex)
    {
        myCharacterHUD.RemoveBuff(anIndex);
    }

    public void SetTargetHUD(GameObject aTarget)
    {
        if(myTargetGO)
        {
            UnsubscribePreviousTargetHUD();
        }

        myTargetGO = aTarget;

        if(myTargetGO == null)
        {
            myTargetHUD.Hide();
            return;
        }

        myTargetHUD.Show();
        aTarget.GetComponent<Health>().EventOnHealthChange += ChangeTargetHudHealth;
        ChangeTargetHudHealth(aTarget.GetComponent<Health>().GetHealthPercentage(),
            aTarget.GetComponent<Health>().myCurrentHealth.ToString() + "/" + aTarget.GetComponent<Health>().MaxHealth,
            aTarget.GetComponent<Health>().GetTotalShieldValue(), false);

        myTargetHUD.SetAvatarSprite(aTarget.GetComponent<UIComponent>().myAvatarSprite);
        myTargetHUD.SetHudColor(aTarget.GetComponent<UIComponent>().myCharacterColor);
    }

    public void UnsubscribePreviousTargetHUD()
    {
        myTargetGO.GetComponent<Health>().EventOnHealthChange -= ChangeTargetHudHealth;
        if (myTargetGO.GetComponent<Resource>() != null)
            myTargetGO.GetComponent<Resource>().EventOnResourceChange -= ChangeTargetHudResource;
    }

    private void ChangeMyHudHealth(float aHealthPercentage, string aHealthText, int aShieldValue, bool aIsDamage)
    {
        myCharacterHUD.SetHealthBarFillAmount(aHealthPercentage);
        myCharacterHUD.SetHealthText(aHealthText);
        myCharacterHUD.SetShieldBar(aShieldValue, myHealth.myCurrentHealth);
    }

    private void ChangeMyHudResource(float aResourcePercentage, string aResourceText)
    {
        myCharacterHUD.SetResourceBarFillAmount(aResourcePercentage);
        myCharacterHUD.SetResourceText(aResourceText);
        myCharacterHUD.SetResourceBarColor(myResource.myResourceColor);
    }

    private void ChangeTargetHudHealth(float aHealthPercentage, string aHealthText, int aShieldValue, bool aIsDamage)
    {
        myTargetHUD.SetHealthBarFillAmount(aHealthPercentage);
        myTargetHUD.SetHealthText(aHealthText);
        if (myTargetGO && myTargetGO.GetComponent<Health>() != null)
            myTargetHUD.SetShieldBar(aShieldValue, myTargetGO.GetComponent<Health>().myCurrentHealth);
    }
    private void ChangeTargetHudResource(float aResourcePercentage, string aResourceText)
    {
        myTargetHUD.SetResourceBarFillAmount(aResourcePercentage);
        myTargetHUD.SetResourceText(aResourceText);
        if (myTargetGO && myTargetGO.GetComponent<Resource>() != null)
            myTargetHUD.SetResourceBarColor(myTargetGO.GetComponent<Resource>().myResourceColor);
    }

    public void SetSpellCooldownText(int anIndex, float aDuration)
    {
        if (myActionButtons[anIndex] == null)
            return;

        if (aDuration > 0.0f)
        {
            myActionButtons[anIndex].GetComponentInChildren<Text>().text = aDuration.ToString("0.0");
        }
        else
        {
            myActionButtons[anIndex].GetComponent<ActionKey>().SetCooldown(0.0f);
        }
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
        myActionButtons[anIndex].GetComponent<Image>().sprite = aSpell.mySpellIcon;
        myActionButtons[anIndex].GetComponent<ActionKey>().SetSpellInfo(aSpell.myQuickInfo);
    }

    public void HighlightSpellError(SpellErrorHandler.SpellError aSpellError)
    {
        mySpellErrorHandler.HighLightError(aSpellError);
    }

    public void ToggleUIText(int anInstanceID)
    {
        myCharacterHUD.ToggleUIText(anInstanceID);
    }
}
