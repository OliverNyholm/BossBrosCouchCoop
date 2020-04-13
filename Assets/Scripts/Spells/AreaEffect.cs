using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaEffect : MonoBehaviour
{
    [SerializeField]
    SpellTypeToBeChanged mySpellType = SpellTypeToBeChanged.Damage;

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
    [SerializeField]
    private GameObject mySpellOverTime = null;

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
                if (mySpellType == SpellTypeToBeChanged.Damage)
                    DealDamage(ref index);
                else if (mySpellType == SpellTypeToBeChanged.Heal)
                    myObjectsInTrigger[index].GetComponent<Health>().GainHealth(myTickDamage);

                if (mySpellOverTime != null)
                    myObjectsInTrigger[index].GetComponent<Stats>().AddSpellOverTime(mySpellOverTime.GetComponent<SpellOverTime>());
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

            if (mySpellType == SpellTypeToBeChanged.Damage)
            {
                int index = myObjectsInTrigger.Count - 1;
                DealDamage(ref index);
            }
            else if (mySpellType == SpellTypeToBeChanged.Heal)
                aOther.GetComponent<Health>().GainHealth(myTickDamage);

            if (mySpellOverTime != null)
                aOther.GetComponent<Stats>().AddSpellOverTime(mySpellOverTime.GetComponent<SpellOverTime>());
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
