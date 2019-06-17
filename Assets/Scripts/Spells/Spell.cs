using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell : MonoBehaviour
{
    public string myName;
    public GameObject myTextMesh;
    public SpellType mySpellType;
    public SpellTarget mySpellTarget;

    public int myDamage;
    public int myResourceCost;

    public float myThreatModifier = 1.0f;
    public float mySpeed;
    public float myCastTime;
    public float myCooldown;
    public float myRange;

    public Color myCastbarColor;
    public Sprite mySpellIcon;

    public bool myIsCastableWhileMoving;
    public bool myCanCastOnSelf;
    public bool myIsOnlySelfCast;

    public Buff myBuff;

    protected GameObject myParent;
    protected GameObject myTarget;

    protected virtual void Update()
    {
        float distance = 0.0f;
        if (mySpeed > 0.0f)
            distance = Vector3.Distance(transform.position, myTarget.transform.position);

        if (distance > 1.0f)
        {
            Vector3 direction = myTarget.transform.position - transform.position;

            transform.position += direction.normalized * mySpeed * Time.deltaTime;
        }
        else
        {
            DealSpellEffect();

            if (myBuff != null)
            {
                SpawnBuff();
            }

            Destroy();
        }
    }
    private void SpawnBuff()
    {
        if (myBuff.mySpellType == SpellType.DOT || myBuff.mySpellType == SpellType.HOT)
        {
            BuffTickSpell buffSpell;
            buffSpell = (myBuff as TickBuff).InitializeBuff(myParent, myTarget);
            if (myTarget.tag == "Player")
                myTarget.GetComponent<Player>().AddBuff(buffSpell, mySpellIcon);
            else if (myTarget.tag == "Enemy")
                myTarget.GetComponent<Enemy>().AddBuff(buffSpell, mySpellIcon);
        }
        else
        {
            BuffSpell buffSpell;
            buffSpell = myBuff.InitializeBuff(myParent);
            if (myTarget.tag == "Player")
                myTarget.GetComponent<Player>().AddBuff(buffSpell, mySpellIcon);
            else if (myTarget.tag == "Enemy")
                myTarget.GetComponent<Enemy>().AddBuff(buffSpell, mySpellIcon);

            if (buffSpell.GetBuff().mySpellType == SpellType.Shield)
                myTarget.GetComponent<Health>().AddShield(buffSpell as BuffShieldSpell);

        }
    }

    protected virtual void DealSpellEffect()
    {
        if (GetSpellTarget() == SpellTarget.Friend)
        {
            if (myDamage > 0.0f)
            {
                myTarget.GetComponent<Health>().GainHealth(myDamage);
                AIPostMaster.Instance.PostAIMessage(new AIMessage(AIMessageType.SpellSpawned, new AIMessageData(myParent.GetInstanceID(), myDamage)));
            }
        }
        else
        {
            if (myDamage > 0.0f)
            {
                myTarget.GetComponent<Health>().TakeDamage(myDamage);
                myTarget.GetComponent<Health>().GenerateThreat((int)(myDamage * myThreatModifier), myParent.GetInstanceID());
            }
        }

        if (mySpellType == SpellType.Interrupt)
        {
            Interrupt();
        }
        if (mySpellType == SpellType.Taunt)
        {
            myTarget.GetComponent<Enemy>().SetTaunt(myParent.GetInstanceID(), 3.0f);
        }
    }

    public virtual void AddDamageIncrease(float aDamageIncrease)
    {
        myDamage = (int)(myDamage * aDamageIncrease);
    }

    public void SetDamage(int aDamage)
    {
        myDamage = aDamage;
    }

    public void SetParent(GameObject aParent)
    {
        myParent = aParent;
    }

    public void SetTarget(GameObject aTarget)
    {
        myTarget = aTarget;
    }

    public SpellTarget GetSpellTarget()
    {
        return mySpellTarget;
    }

    public bool IsCastableWhileMoving()
    {
        return myIsCastableWhileMoving || myCastTime <= 0.0f;
    }

    public string GetSpellDescription()
    {
        string name = myName + "\n";

        string target = "Cast spell on ";
        if (myIsOnlySelfCast)
            target += "self ";
        else
            target += mySpellTarget.ToString() + " ";

        string detail = GetSpellDetail();

        string range = "";
        if (myRange != 0 && myIsOnlySelfCast)
            range = "Range: " + myRange;
        string cost = "\nCosts " + myResourceCost + " to cast spell. ";

        string castTime = "\nSpell ";
        if (myCastTime <= 0.0f)
        {
            castTime += "is instant cast.";
        }
        else
        {
            castTime += "takes " + myCastTime + " seconds to cast. ";
            if (myIsCastableWhileMoving)
                castTime += "Is castable while moving.";
            else
                castTime += "Is not castable while moving";
        }

        return name + target + detail + range + cost + castTime;
    }

    protected virtual string GetSpellDetail()
    {
        string detail = "to ";
        switch (mySpellType)
        {
            case SpellType.Damage:
                detail += "deal " + myDamage + " damage. ";
                break;
            case SpellType.Heal:
                detail += "heal " + myDamage + " damage. ";
                break;
            case SpellType.Interrupt:
                if (myDamage > 0.0f)
                    detail += "deal " + myDamage + " damage and ";
                detail += "interrupt any spellcast. ";
                break;
            case SpellType.Taunt:
                detail += "to make the target attack you for a few seconds.";
                break;
            case SpellType.Buff:
                detail += GetDefaultBuffDetails();

                if (myBuff.mySpellType == SpellType.DOT)
                {
                    if (detail[detail.Length - 1] == '%')
                        detail += " and ";

                    detail += "deal " + (myBuff as TickBuff).myTotalDamage + " damage over ";
                }
                else if (myBuff.mySpellType == SpellType.HOT)
                {
                    if (detail[detail.Length - 1] == '%')
                        detail += " and ";

                    detail += "heal " + (myBuff as TickBuff).myTotalDamage + " damage over ";
                }
                else if (myBuff.mySpellType == SpellType.Shield)
                {
                    if (detail[detail.Length - 1] == '%')
                        detail += " and ";

                    detail += "place a shield that will absorb " + (myBuff as ShieldBuff).myShieldValue + " damage for ";
                }

                detail += myBuff.myDuration.ToString("0") + " seconds.";
                break;
            case SpellType.Ressurect:
                detail += "ressurect the target. ";
                break;
            case SpellType.Special:
                //Override this function and add special text.
                break;
        }

        return detail;
    }

    private string GetDefaultBuffDetails()
    {
        string buffDetail = " ";

        bool shouldAddComma = false;

        if (myBuff.mySpeedMultiplier > 0.0f)
        {
            buffDetail += "increase movement speed by " + (myBuff.mySpeedMultiplier * 100).ToString("0") + "%";
            shouldAddComma = true;
        }
        else if (myBuff.mySpeedMultiplier < 0.0f)
        {
            buffDetail += "reduce movement speed by " + (myBuff.mySpeedMultiplier * 100).ToString("0") + "%";
            shouldAddComma = true;
        }

        if (myBuff.myAttackSpeed > 0.0f)
        {
            if (shouldAddComma)
                buffDetail += ", and ";

            buffDetail += "increase attack speed by " + (myBuff.myAttackSpeed * 100).ToString("0") + "%";
            shouldAddComma = true;
        }
        else if (myBuff.myAttackSpeed < 0.0f)
        {
            if (shouldAddComma)
                buffDetail += ", and ";

            buffDetail += "reduce attack speed by " + (myBuff.myAttackSpeed * 100).ToString("0") + "%";
            shouldAddComma = true;
        }

        if (myBuff.myDamageMitigator > 0.0f)
        {
            if (shouldAddComma)
                buffDetail += ", and ";

            buffDetail += "reduce damage taken by " + (myBuff.myDamageMitigator * 100).ToString("0") + "%";
            shouldAddComma = true;
        }
        else if (myBuff.myDamageMitigator < 0.0f)
        {
            if (shouldAddComma)
                buffDetail += ", and ";

            buffDetail += "increase damage taken by " + (myBuff.myDamageMitigator * 100).ToString("0") + "%";
            shouldAddComma = true;
        }

        if (myBuff.myDamageIncrease > 0.0f)
        {
            if (shouldAddComma)
                buffDetail += ", and ";

            buffDetail += "increase damage dealt by " + (myBuff.myDamageIncrease * 100).ToString("0") + "%";
        }
        else if (myBuff.myDamageIncrease < 0.0f)
        {
            if (shouldAddComma)
                buffDetail += ", and ";

            buffDetail += "reduce damage dealt by " + (myBuff.myDamageIncrease * 100).ToString("0") + "% ";
        }

        if (myBuff.mySpellType == SpellType.Buff)
            buffDetail += " for ";

        return buffDetail;
    }

    private string GetSpellHitText()
    {
        string text = string.Empty;

        if (myDamage > 0)
            text += myDamage.ToString();
        if (mySpellType == SpellType.DOT)
            text += " - DOT " + myName;
        if (mySpellType == SpellType.HOT)
            text += " - HOT " + myName;
        if (mySpellType == SpellType.Interrupt)
            text += " - Interrupt";
        if (mySpellType == SpellType.Slow)
            text += " - Slow";
        if (mySpellType == SpellType.Taunt)
            text += " - Taunt";
        if (mySpellType == SpellType.Ressurect)
            text += " - Ressurect";

        return text;
    }

    private void Interrupt()
    {
        if (myTarget.tag == "Player")
            myTarget.GetComponent<Player>().InterruptSpellCast();
        else if (myTarget.tag == "Enemy")
            myTarget.GetComponent<Enemy>().InterruptSpellCast();
    }

    private void Destroy()
    {
        Destroy(gameObject);
    }
}
