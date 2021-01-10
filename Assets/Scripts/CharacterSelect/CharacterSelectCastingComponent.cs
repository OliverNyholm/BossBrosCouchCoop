using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectCastingComponent : PlayerCastingComponent
{
    private CharacterSelector myCharacterSelector;
    private GameObject myAttackObject = null;

    private float myHeldDownTimeStamp = float.MaxValue;
    private float myTimeUntilShowSpellDetails = 1.0f;
    private int myHeldDownSpellIndex = -1;
    private bool myIsShowingInfo = false;

    private int myHasChangedClassTickCounter = -1;

    public void SetCharacterSelector(CharacterSelector aCharacterSelector)
    {
        myCharacterSelector = aCharacterSelector;
        myPlayerControls = myCharacterSelector.PlayerControls;
    }

    private void Start()
    {
        myAttackObject = GameObject.Find("AttackObject");
    }

    void Update()
    {
        DetectSpellInput();

        if (myHasChangedClassTickCounter > -1)
        {
            myHasChangedClassTickCounter++; //Dirty hacking here to make channel spells be interrupted in the channel
            if (myHasChangedClassTickCounter == 2)
                myHasChangedClassTickCounter = -1;
        }

        if (myHeldDownSpellIndex == -1)
            return;

        if (!myIsShowingInfo && Time.time - myHeldDownTimeStamp > myTimeUntilShowSpellDetails)
        {
            myIsShowingInfo = true;
            myCharacterSelector.ShowSpellInfo(myClass.GetSpell(myHeldDownSpellIndex).GetComponent<Spell>());
        }
    }

    private void DetectSpellInput()
    {
        if (myPlayerControls.Action1.WasPressed)
        {
            myHeldDownSpellIndex = 0;
            myHeldDownTimeStamp = Time.time;
        }
        else if (myPlayerControls.Action2.WasPressed)
        {
            myHeldDownSpellIndex = 1;
            myHeldDownTimeStamp = Time.time;
        }
        else if (myPlayerControls.Action3.WasPressed)
        {
            myHeldDownSpellIndex = 2;
            myHeldDownTimeStamp = Time.time;
        }
        else if (myPlayerControls.Action4.WasPressed)
        {
            myHeldDownSpellIndex = 3;
            myHeldDownTimeStamp = Time.time;
        }

        if(myHeldDownTimeStamp == Time.time)
        {
            myUIComponent.SpellHeldDown(myHeldDownSpellIndex);

            if (IsChannelSpell(myHeldDownSpellIndex))
                CastSpell(myHeldDownSpellIndex);
        }

        if (myPlayerControls.Action1.WasReleased)
            ButtonReleased(0);
        else if (myPlayerControls.Action2.WasReleased)
            ButtonReleased(1);
        else if (myPlayerControls.Action3.WasReleased)
            ButtonReleased(2);
        else if (myPlayerControls.Action4.WasReleased)
            ButtonReleased(3);
    }

    private void ButtonReleased(int aSpellIndex, bool anIsSwappingClass = false)
    {
        if (myIsShowingInfo)
        {
            myCharacterSelector.HideSpellInfo();
        }
        else
        {
            if(!IsChannelSpell(aSpellIndex) && !anIsSwappingClass)
                CastSpell(aSpellIndex);
        }

        myUIComponent.SpellReleased(aSpellIndex);
        myIsShowingInfo = false;
        myHeldDownSpellIndex = -1;
    }

    public void OnClassChanged()
    {
        for (int index = 0; index < 4; index++)
        {
            GameObject spellGO = myClass.GetSpell(index);
            if (!spellGO)
                continue;

            ToggleSpell toggleSpell = spellGO.GetComponent<ToggleSpell>();
            if (!toggleSpell)
                continue;

            if (myClass.IsSpellToggled(toggleSpell))
                myClass.ToggleSpell(toggleSpell);

            ButtonReleased(index, true);
        }

        myHasChangedClassTickCounter = 0;
    }

    private void CastSpell(int aKeyIndex)
    {
        GameObject spell = myClass.GetSpell(aKeyIndex);
        Spell spellScript = spell.GetComponent<Spell>();

        if (spellScript.IsCastOnFriends())
            myTargetingComponent.SetTarget(gameObject);
        else
            myTargetingComponent.SetTarget(myAttackObject);

        if (!spellScript.myIsOnlySelfCast)
            myTargetingComponent.SpellTarget = myTargetingComponent.Target;

        if (myIsCasting)
            return;

        myAnimatorWrapper.SetTrigger(spellScript.GetAnimationType());

        if (spellScript.myCastTime <= 0.0f)
        {
            SpawnSpell(spellScript, GetSpellSpawnPosition(spellScript));
            myClass.SetSpellOnCooldown(aKeyIndex);
            myAnimatorWrapper.SetTrigger(spellScript.myAnimationType);
            return;
        }

        myAnimatorWrapper.SetBool(AnimationVariable.IsCasting, true);
        myUIComponent.SetCastbarStartValues(spellScript);
        myTargetingComponent.SetSpellTarget(myTargetingComponent.Target);
        myCastingRoutine = StartCoroutine(CastbarProgress(aKeyIndex));
    }

    protected override bool IsAbleToCastSpell(Spell aSpellScript)
    {
        if (myIsCasting)
            return false;

        return true;
    }

    private bool IsChannelSpell(int aSpellIndex)
    {
        if (!myClass.HasSpell(aSpellIndex))
            return false;

        GameObject spell = myClass.GetSpell(aSpellIndex);
        return spell.GetComponent<ChannelSpell>() != null;
    }

    protected override bool WasSpellChannelButtonReleased(int aSpellIndex)
    {
        if (myHasChangedClassTickCounter > -1)
            return true;

        return base.WasSpellChannelButtonReleased(aSpellIndex);
    }
}
