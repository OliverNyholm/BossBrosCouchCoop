using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialSpellUI : MonoBehaviour
{
    [Header("Children whose properties changes")]
    [SerializeField]
    private Image mySpellIcon = null;
    [SerializeField]
    private Text mySpellName = null;
    [SerializeField]
    private Text mySpellDescription = null;
    [SerializeField]
    private Text myCooldownText = null;
    [SerializeField]
    private Text myRangeText = null;
    [SerializeField]
    private Text myCastTimeText = null;
    [SerializeField]
    private Text myCostText = null;

    private Spell myCurrentSpell;

    public void SetDetails(Spell aSpell, Resource aResource)
    {
        myCurrentSpell = aSpell;

        mySpellIcon.sprite = aSpell.mySpellIcon;
        mySpellName.text = aSpell.myName;
        mySpellDescription.text = aSpell.myTutorialInfo;

        if (aSpell.myCooldown > 0.0f)
        {
            myCooldownText.text = aSpell.myCooldown.ToString("0.0") + " seconds cooldown";
        }
        else
        {
            myCooldownText.text = "No cooldown";
        }

        if (aSpell.myIsOnlySelfCast)
        {
            myRangeText.text = "Self cast";
        }
        else
        {
            myRangeText.text = aSpell.myRange.ToString("0") + " meter range";
        }

        if(aSpell.myCastTime > 0.0f)
        {
            myCastTimeText.text = aSpell.myCastTime.ToString("0.0") + " seconds to cast";
            if (aSpell.myIsCastableWhileMoving)
                myCastTimeText.text += "(Movable)";
        }
        else
        {
            myCastTimeText.text = "Instant cast";
        }

        float resourcePercentage = ((float)aSpell.myResourceCost / aResource.MaxResource) * 100;
        myCostText.text = "costs " + resourcePercentage.ToString("0.0") + "% resource";
    }

    public Spell GetCurrentSpell()
    {
        return myCurrentSpell;
    }
}
