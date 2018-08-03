using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class OnTargetSpell : NetworkBehaviour
{

    public GameObject myTextMesh;

    public SpellType mySpellType;
    public int myTotalDamage;
    public float myDuration;
    public float myModifier;
    public int myNrOfTicks;

    private GameObject myParent;

    [SyncVar]
    private Transform myTarget;

    private float myLifeTime;
    private float myInterval;
    private float myIntervalTimer;
    private int myTickDamage;

    private float myEnemyOriginalSpeed; //Used by slow

    void Start()
    {
        if (myTotalDamage > 0)
            myTickDamage = myTotalDamage / myNrOfTicks;

        if (mySpellType == SpellType.Slow)
        {
            myEnemyOriginalSpeed = myTarget.GetComponent<Enemy>().mySpeed;
            myTarget.GetComponent<Enemy>().mySpeed *= myModifier;
        }
        else if (mySpellType == SpellType.DOT || mySpellType == SpellType.HOT)
        {
            myInterval = myDuration / myNrOfTicks;
            myIntervalTimer = 0.0f;

            myDuration += 0.05f; //Ugly hack to get the last tick off before destruction
        }
        else if (mySpellType == SpellType.Shield)
        {
            //Do something later.
        }
    }

    void Update()
    {
        myLifeTime += Time.deltaTime;

        if (myLifeTime >= myDuration)
        {
            if (mySpellType == SpellType.Slow)
            {
                myTarget.GetComponent<Enemy>().mySpeed = myEnemyOriginalSpeed;
            }

            GameObject textMesh = Instantiate(myTextMesh, transform.position, transform.rotation, myTarget);
            textMesh.GetComponent<FloatingHealth>().SetText("Debuff Fade", Color.cyan);
            Destroy(transform.gameObject);
        }

        if (myTotalDamage == 0)
            return;

        myIntervalTimer += Time.deltaTime;

        if (myIntervalTimer >= myInterval)
        {
            myIntervalTimer -= myInterval;

            if (IsFriendly())
                myTarget.GetComponent<Health>().GainHealth(myTickDamage);
            else
                myTarget.GetComponent<Health>().TakeDamage(myTickDamage);

            GameObject textMesh = Instantiate(myTextMesh, transform.position, transform.rotation, myTarget);
            textMesh.GetComponent<FloatingHealth>().SetText(myTickDamage.ToString(), new Color(85f / 255f, 107f / 255f, 47f / 255f));
        }
    }

    public void SetParent(GameObject aParent)
    {
        myParent = aParent;
    }

    public void SetTarget(Transform aTarget)
    {
        myTarget = aTarget;
    }

    public bool IsFriendly()
    {
        if (mySpellType == SpellType.Heal
            || mySpellType == SpellType.HOT
            || mySpellType == SpellType.Shield
            || mySpellType == SpellType.Buff
            || mySpellType == SpellType.Ressurect)
            return true;

        return false;
    }
}
