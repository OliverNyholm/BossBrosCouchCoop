using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectCastingComponent : PlayerCastingComponent
{
    private CharacterSelector myCharacterSelector;

    private float myHeldDownTimeStamp = float.MaxValue;
    private float myTimeUntilShowSpellDetails = 1.0f;
    private int myHeldDownSpellIndex = -1;
    private bool myIsShowingInfo = false;

    public void SetCharacterSelector(CharacterSelector aCharacterSelector)
    {
        myCharacterSelector = aCharacterSelector;
        myPlayerControls = myCharacterSelector.PlayerControls;
    }

    void Update()
    {
        DetectSpellInput();

        if (myHeldDownSpellIndex == -1)
            return;

        if (!myIsShowingInfo && Time.time - myHeldDownTimeStamp > myTimeUntilShowSpellDetails)
        {
            myIsShowingInfo = true;
            myCharacterSelector.ShowSpellInfo(myHeldDownSpellIndex);
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

        if (myPlayerControls.Action1.WasReleased)
            ButtonReleased(0);
        else if (myPlayerControls.Action2.WasReleased)
            ButtonReleased(1);
        else if (myPlayerControls.Action3.WasReleased)
            ButtonReleased(2);
        else if (myPlayerControls.Action4.WasReleased)
            ButtonReleased(3);
    }

    private void ButtonReleased(int aSpellIndex)
    {
        if (myIsShowingInfo)
            myCharacterSelector.HideSpellInfo(aSpellIndex);
        else
            CastSpell(aSpellIndex);

        myIsShowingInfo = false;
        myHeldDownSpellIndex = -1;
    }

    private void CastSpell(int aKeyIndex)
    {
        GameObject spell = myClass.GetSpell(aKeyIndex);
        Spell spellScript = spell.GetComponent<Spell>();

        if (GetComponent<Health>().IsDead())
            return;

        if (!spellScript.myIsOnlySelfCast)
            myTargetingComponent.SpellTarget = myTargetingComponent.Target;

        if (myIsCasting)
            return;

        myAnimatorWrapper.SetTrigger(spellScript.GetAnimationType());

        if (spellScript.myCastTime <= 0.0f)
        {
            SpawnSpell(aKeyIndex, GetSpellSpawnPosition(spellScript));
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
}
