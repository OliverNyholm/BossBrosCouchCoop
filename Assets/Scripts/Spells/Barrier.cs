using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : ChannelSpell
{
    [SerializeField]
    private Buff myBuff = null;
    [SerializeField]
    private GameObject mySpellOverTime = null;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            SpawnBuff(other.gameObject);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            RemoveBuff(other.gameObject);
        }
    }


    private void SpawnBuff(GameObject aTarget)
    {
        aTarget.GetComponent<Stats>().AddSpellOverTime(mySpellOverTime.GetComponent<SpellOverTime>());
    }

    private void RemoveBuff(GameObject aTarget)
    {
        aTarget.GetComponent<Stats>().RemoveSpellOverTime(mySpellOverTime.GetComponent<SpellOverTime>());
    }

    public override void SetToDestroy()
    {  
        transform.Translate(Vector3.up * 1000);
        myTimerBeforeDestroy = 0.1f;
    }
}