using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Barrier : ChannelSpell
{
    [SerializeField]
    private Buff myBuff;

    void OnTriggerEnter(Collider other)
    {
        if (!isServer)
            return;

        if (other.tag == "Player")
        {
            RpcSpawnBuff(other.gameObject);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!isServer)
            return;

        if (other.tag == "Player")
        {
            RpcRemoveBuff(other.gameObject);
        }
    }


    [ClientRpc]
    private void RpcSpawnBuff(GameObject aTarget)
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

    [ClientRpc]
    private void RpcRemoveBuff(GameObject aTarget)
    {
        Debug.Log("Remove Buff: " + myBuff.name);
        aTarget.GetComponent<Player>().RemoveBuffByName(myBuff.name);
    }

    [ClientRpc]
    public override void RpcSetToDestroy()
    {  
        transform.Translate(Vector3.up * 1000);
        myTimerBeforeDestroy = 0.1f;
    }
}