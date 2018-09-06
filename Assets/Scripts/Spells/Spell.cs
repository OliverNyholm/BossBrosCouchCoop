using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Spell : NetworkBehaviour
{
    public string myName;
    public GameObject myTextMesh;
    public SpellType mySpellType;
    public SpellTarget mySpellTarget;

    public int myDamage;
    public int myResourceCost;

    public float mySpeed;
    public float myCastTime;
    public float myCooldown;
    public float myRange;

    public Color myCastbarColor;
    public Sprite myCastbarIcon;

    public bool myIsCastableWhileMoving;
    public bool myCanCastOnSelf;
    public bool myIsOnlySelfCast;

    public Buff myBuff;

    [SyncVar]
    protected GameObject myParent;
    [SyncVar]
    protected GameObject myTarget;

    void Update()
    {
        if (!isServer)
            return;


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
                RpcSpawnBuff();
            }

            RpcDestroy();
        }
    }

    [ClientRpc]
    private void RpcSpawnBuff()
    {
        if (myBuff.mySpellType == SpellType.DOT || myBuff.mySpellType == SpellType.HOT)
        {
            BuffTickSpell buffSpell;
            buffSpell = (myBuff as TickBuff).InitializeBuff(myParent, myTarget);
            myTarget.GetComponent<PlayerCharacter>().AddBuff(buffSpell, myCastbarIcon);
        }
        else
        {
            BuffSpell buffSpell;
            buffSpell = myBuff.InitializeBuff(myParent);
            myTarget.GetComponent<PlayerCharacter>().AddBuff(buffSpell, myCastbarIcon);
        }
    }

    protected virtual void DealSpellEffect()
    {
        if (GetSpellTarget() == SpellTarget.Friend)
        {
            if (myDamage > 0.0f)
                myTarget.GetComponent<Health>().GainHealth(myDamage);
        }
        else
        {
            if (myDamage > 0.0f)
                myTarget.GetComponent<Health>().TakeDamage(myDamage);
        }

        if (mySpellType == SpellType.Interrupt)
        {
            RpcInterrupt();
        }
    }

    public void AddDamageIncrease(float aDamageIncrease)
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
        if (mySpellType == SpellType.Silence)
            text += " - Silence";
        if (mySpellType == SpellType.Ressurect)
            text += " - Ressurect";

        return text;
    }

    [ClientRpc]
    private void RpcInterrupt()
    {
        myTarget.GetComponent<PlayerCharacter>().InterruptSpellCast();
    }

    [ClientRpc]
    private void RpcDestroy()
    {
        NetworkServer.Destroy(gameObject);
    }
}
