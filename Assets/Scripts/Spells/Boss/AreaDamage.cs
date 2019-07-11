using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaDamage : MonoBehaviour
{
    [Header("Damage per tick")]
    [SerializeField]
    private int myTickDamage = 100;

    [Header("Duration between each tick")]
    [SerializeField]
    private float myDurationPerTick = 1.0f;
    private float myTimer;

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
                Health healthComponent = myObjectsInTrigger[index].GetComponent<Health>();
                healthComponent.TakeDamage(myTickDamage, Color.red);
                if (healthComponent.IsDead())
                {
                    myObjectsInTrigger.RemoveAt(index);
                    index--;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider aOther)
    {
        if (aOther.tag == "Player")
        {
            if (aOther.gameObject.GetComponent<Health>().IsDead())
                return;

            myObjectsInTrigger.Add(aOther.gameObject);
            aOther.gameObject.GetComponent<Health>().TakeDamage(myTickDamage, Color.red);
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
