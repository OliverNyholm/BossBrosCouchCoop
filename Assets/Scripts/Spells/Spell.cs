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

    public GameObject OnTargetSpell;
    [SyncVar]
    protected GameObject myParent;
    [SyncVar]
    public GameObject myTarget;

    void Start()
    {

    }

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

            Vector3 startPosition = myTarget.transform.position + new Vector3(Random.Range(-2.0f, 2.0f), 2.0f, Random.Range(-2.0f, 2.0f));
            GameObject textMesh = Instantiate(myTextMesh, startPosition, myTarget.transform.rotation, myTarget.transform);
            Color textColor = GetSpellTarget() == SpellTarget.Friend ? Color.yellow : Color.red;
            textMesh.GetComponent<FloatingHealth>().SetText(GetSpellHitText(), textColor);


            if (OnTargetSpell != null)
            {
                CmdSpawnOnTargetSpell();
            }

            RpcDestroy();
        }
    }

    [Command]
    private void CmdSpawnOnTargetSpell()
    {
        GameObject onTarget = Instantiate(OnTargetSpell, myTarget.transform.position, myTarget.transform.rotation, myTarget.transform);
        onTarget.GetComponent<OnTargetSpell>().SetParent(transform.gameObject);
        onTarget.GetComponent<OnTargetSpell>().SetTarget(myTarget.transform);

        NetworkServer.Spawn(onTarget);
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

        if (mySpellType == SpellType.Interrupt) //TODO - Remove this later on to enemy spellTarget
        {
            RpcInterrupt();
        }
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
