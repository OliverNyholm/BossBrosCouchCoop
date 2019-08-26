using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaEffect : MonoBehaviour
{
    [SerializeField]
    SpellType mySpellType = SpellType.Damage;

    [Header("Damage per tick")]
    [SerializeField]
    private int myTickDamage = 100;

    [Header("Duration between each tick")]
    [SerializeField]
    private float myDurationPerTick = 1.0f;
    private float myTimer;

    [SerializeField]
    private Buff myBuff = null;
    [SerializeField]
    private Sprite myBuffSprite = null;

    private List<GameObject> myObjectsInTrigger;

    // Start is called before the first frame update
    void Start()
    {
        myObjectsInTrigger = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        myTimer -= Time.deltaTime;
        if (myTimer <= 0.0f)
        {
            myTimer = myDurationPerTick;
            for (int index = 0; index < myObjectsInTrigger.Count; index++)
            {
                if (mySpellType == SpellType.Damage)
                    DealDamage(ref index);
                else if (mySpellType == SpellType.Heal)
                    myObjectsInTrigger[index].GetComponent<Health>().GainHealth(myTickDamage);

                if (myBuff != null)
                    myObjectsInTrigger[index].GetComponent<Character>().AddBuff(myBuff.InitializeBuff(gameObject), myBuffSprite);
            }
        }
    }

    private void DealDamage(ref int aIndex)
    {
        Health healthComponent = myObjectsInTrigger[aIndex].GetComponent<Health>();
        healthComponent.TakeDamage(myTickDamage, Color.red);
        if (healthComponent.IsDead())
        {
            myObjectsInTrigger.RemoveAt(aIndex);
            aIndex--;
        }
    }

    private void OnTriggerEnter(Collider aOther)
    {
        if (aOther.tag == "Player")
        {
            if (aOther.gameObject.GetComponent<Health>().IsDead())
                return;

            myObjectsInTrigger.Add(aOther.gameObject);

            if (mySpellType == SpellType.Damage)
            {
                int index = myObjectsInTrigger.Count - 1;
                DealDamage(ref index);
            }
            else if (mySpellType == SpellType.Heal)
                aOther.GetComponent<Health>().GainHealth(myTickDamage);

            if (myBuff != null)
                aOther.GetComponent<Character>().AddBuff(myBuff.InitializeBuff(gameObject), myBuffSprite);
        }
    }

    private void OnTriggerExit(Collider aOther)
    {
        if (aOther.tag == "Player")
        {
            myObjectsInTrigger.Remove(aOther.gameObject);
        }
    }
}
