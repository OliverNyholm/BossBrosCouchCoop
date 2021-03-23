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

    [SerializeField]
    private bool myUseCastbarAboveCharacter = true;

    [SerializeField]
    private bool myUseMinionHud = false;
    private Canvas myMinionCanvas = null;

    [SerializeField]
    private CharacterHUD myCharacterHUD = null;

    private CharacterHUD myTargetHUD;
    protected Castbar myCastbar;

    private Health myHealth;
    private Resource myResource;

    private GameObject myTargetGO;

    protected virtual void Awake()
    {
        myHealth = GetComponent<Health>();
        myResource = GetComponent<Resource>();

        if (myUseMinionHud)
            myMinionCanvas = myCharacterHUD.GetComponent<Canvas>();
    }

    public virtual void SetupHud(Transform aUIParent)
    {
        if (!myUseMinionHud)
        {
            myCharacterHUD = aUIParent.Find("CharacterHud").GetComponent<CharacterHUD>();
            myTargetHUD = aUIParent.Find("TargetHud").GetComponent<CharacterHUD>();
            aUIParent.GetComponent<CanvasGroup>().alpha = 1.0f;
        }
        else
        {
            myMinionCanvas.worldCamera = Camera.main;
        }

        if (myUseCastbarAboveCharacter)
        {
            myCastbar = GetComponentInChildren<Castbar>();
            if(myCastbar)
                myCastbar.GetComponent<Canvas>().worldCamera = Camera.main;
            else
                myCastbar = aUIParent.Find("Castbar Background").GetComponent<Castbar>();
        }
        else
        {
            myCastbar = aUIParent.Find("Castbar Background").GetComponent<Castbar>();
        }

        myCharacterHUD.SetName(myName);
        myCharacterHUD.SetClassSprite(myClassSprite);
        myCharacterHUD.SetAvatarSprite(myAvatarSprite);
        myCharacterHUD.SetHudColor(myCharacterColor);

        myHealth.EventOnHealthChange += ChangeMyHudHealth;

        ChangeMyHudHealth(myHealth.GetHealthPercentage(), myHealth.myCurrentHealth.ToString() + "/" + myHealth.myMaxHealth.ToString(), GetComponent<Health>().GetTotalShieldValue(), false);

        if (myResource)
        {
            myResource.EventOnResourceChange += ChangeMyHudResource;
            ChangeMyHudResource(myResource.GetResourcePercentage(), myResource.myCurrentResource.ToString() + "/" + myResource.MaxResource.ToString());
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
        myCastbar.SetCastbarFillAmount(aFillAmount);
        myCastbar.SetCastTimeText(aText);
    }

    public void FadeCastbar(bool aWasInterruped)
    {
        if (aWasInterruped)
            myCastbar.SetSpellName("Interrupted");
        myCastbar.FadeOutCastbar();
    }

    public GameObject AddBuffAndGetUIRef(SpellOverTime aSpell)
    {
        if(myCharacterHUD)
            return myCharacterHUD.AddBuffAndGetRef(aSpell);

        return null;
    }

    public void UpdateBuffStackCount(int anIndex, int aStackCount)
    {
        if (myCharacterHUD)
            myCharacterHUD.UpdateBuffCount(anIndex, aStackCount);
    }

    public void RemoveBuff(GameObject aBuffWidget)
    {
        if(myCharacterHUD)
            myCharacterHUD.RemoveBuff(aBuffWidget);
    }

    public void SetTargetHUD(GameObject aTarget)
    {
        if (myTargetGO)
        {
            UnsubscribePreviousTargetHUD();
        }

        myTargetGO = aTarget;

        if (myTargetGO == null)
        {
            if (myTargetHUD)
                myTargetHUD.Hide();
            else
                myCharacterHUD.SetMinionTargetHudColor(Color.black);

            return;
        }


        UIComponent targetUIComponent = aTarget.GetComponent<UIComponent>();
        Health targetHealthComponent = aTarget.GetComponent<Health>();

        if (myTargetHUD)
        {
            myTargetHUD.Show();
            targetHealthComponent.EventOnHealthChange += ChangeTargetHudHealth;
            ChangeTargetHudHealth(targetHealthComponent.GetHealthPercentage(),
                targetHealthComponent.myCurrentHealth.ToString() + "/" + targetHealthComponent.MaxHealth,
                targetHealthComponent.GetTotalShieldValue(), false);

            myTargetHUD.SetAvatarSprite(targetUIComponent.myAvatarSprite);
            myTargetHUD.SetHudColor(targetUIComponent.myCharacterColor);
        }
        else
        {
            myCharacterHUD.SetMinionTargetHudColor(targetUIComponent.myCharacterColor);
        }
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

    public void ToggleUIText(int anInstanceID)
    {
        myCharacterHUD.ToggleUIText(anInstanceID);
    }

    public bool UseMinionHud()
    {
        return myUseMinionHud;
    }

    public void SetMinionHudEnable(bool anSetEnabled)
    {
        if (!myMinionCanvas)
            myMinionCanvas = myCharacterHUD.GetComponent<Canvas>();

        myMinionCanvas.enabled = anSetEnabled;
    }
}
