using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : ChannelSpell
{
    [SerializeField]
    private Buff myBuff = null;

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
        if (myBuff.mySpellType == SpellType.DOT || myBuff.mySpellType == SpellType.HOT)
        {
            BuffTickSpell buffSpell;
            buffSpell = (myBuff as TickBuff).InitializeBuff(transform.parent.gameObject, aTarget);
            aTarget.GetComponent<Player>().AddBuff(buffSpell, mySpellIcon);
        }
        else
        {
            BuffSpell buffSpell;
            buffSpell = myBuff.InitializeBuff(transform.parent.gameObject);
            aTarget.GetComponent<Player>().AddBuff(buffSpell, mySpellIcon);
        }
    }

    private void RemoveBuff(GameObject aTarget)
    {
        Debug.Log("Remove Buff: " + myBuff.name);
        aTarget.GetComponent<Player>().RemoveBuffByName(myBuff.name);
    }

    public override void SetToDestroy()
    {  
        transform.Translate(Vector3.up * 1000);
        myTimerBeforeDestroy = 0.1f;
    }
}