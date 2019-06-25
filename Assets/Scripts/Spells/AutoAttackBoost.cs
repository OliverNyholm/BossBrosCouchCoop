using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoAttackBoost : Spell
{
    [Header("Duration which the boost will last until auto attack")]
    [SerializeField]
    private float myLifeTime = 4.0f;

    private Animator myParentAnimator;

    void Start()
    {
        myParentAnimator = myParent.GetComponent<Animator>();

        Transform weaponSlot = myParent.transform.FindInChildren("MainHandVFX");
        transform.parent = weaponSlot;
        transform.localPosition = Vector3.zero;
    }
    
    protected override void Update()
    {
        myLifeTime -= Time.deltaTime;
        if(myLifeTime <= 0.0f)
        {
            Destroy(gameObject);
        }

        if (myParentAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "AutoAtk")
        {

            myTarget = myParent.GetComponent<Character>().GetTarget();
            if (myTarget == null)
                return;

            DealSpellEffect();
            transform.parent = myTarget.transform;
            transform.localPosition = Vector3.zero;

            SpawnVFX(2.5f);
            
            Destroy(gameObject);
        }
    }

    protected override string GetSpellDetail()
    {
        string detail = "to empower your next auto attack within 4 seconds, dealing " + myDamage + " damage.";

        return detail;
    }
}
