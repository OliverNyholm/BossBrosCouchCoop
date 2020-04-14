using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaEffect : MonoBehaviour
{
    [SerializeField]
    AttackType mySpellType = AttackType.Damage;

    [Header("Damage per tick")]
    [SerializeField]
    private int myTickValue = 100;

    [Header("Duration between each tick")]
    [SerializeField]
    private float myDurationPerTick = 1.0f;
    private float myTimer;

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
                if (UtilityFunctions.HasSpellType(mySpellType, AttackType.Damage))
                    DealDamage(ref index);
                if (UtilityFunctions.HasSpellType(mySpellType, AttackType.Heal))
                    myObjectsInTrigger[index].GetComponent<Health>().GainHealth(myTickValue);

                if (mySpellOverTime != null)
                    AddBuff(myObjectsInTrigger[index]);
            }
        }
    }

    private void DealDamage(ref int aIndex)
    {
        Health healthComponent = myObjectsInTrigger[aIndex].GetComponent<Health>();
        healthComponent.TakeDamage(myTickValue, Color.red);
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

            if (UtilityFunctions.HasSpellType(mySpellType, AttackType.Damage))
            {
                int index = myObjectsInTrigger.Count - 1;
                DealDamage(ref index);
            }
            if (UtilityFunctions.HasSpellType(mySpellType, AttackType.Heal))
                aOther.GetComponent<Health>().GainHealth(myTickValue);

            if (mySpellOverTime != null)
                AddBuff(aOther.gameObject);
        }
    }

    private void OnTriggerExit(Collider aOther)
    {
        if (aOther.tag == "Player")
        {
            myObjectsInTrigger.Remove(aOther.gameObject);
        }
    }

    private void AddBuff(GameObject aPlayer)
    {
        GameObject pooledObject = PoolManager.Instance.GetPooledObject(mySpellOverTime.GetComponent<UniqueID>().GetID());
        SpellOverTime spellOverTime = pooledObject.GetComponent<SpellOverTime>();
        spellOverTime.SetTarget(aPlayer);
        spellOverTime.transform.parent = aPlayer.transform;
    }
}
