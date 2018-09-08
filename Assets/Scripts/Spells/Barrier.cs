using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Barrier : ChannelSpell
{
    [SerializeField]
    private Buff myBuff;

    private List<GameObject> myCollidedWith = new List<GameObject>();

    void OnTriggerEnter(Collider other)
    {
        if (!isServer)
            return;

        if (other.tag == "Player")
        {
            RpcSpawnBuff(other.gameObject);
            myCollidedWith.Add(other.gameObject);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!isServer)
            return;

        if (other.tag == "Player")
        {
            RpcRemoveBuff(other.gameObject);
            myCollidedWith.Remove(other.gameObject);
        }
    }

    public void OnDisable()
    {
        if (!isServer)
            return;

        Debug.Log("ON DESTROY! : " + myCollidedWith.Count);
        for (int index = 0; index < myCollidedWith.Count; index++)
        {
            RpcRemoveBuff(myCollidedWith[index]);
        }
    }


    [ClientRpc]
    private void RpcSpawnBuff(GameObject aTarget)
    {
        if (myBuff.mySpellType == SpellType.DOT || myBuff.mySpellType == SpellType.HOT)
        {
            BuffTickSpell buffSpell;
            buffSpell = (myBuff as TickBuff).InitializeBuff(transform.parent.gameObject, aTarget);
            aTarget.GetComponent<PlayerCharacter>().AddBuff(buffSpell, mySpellIcon);
        }
        else
        {
            BuffSpell buffSpell;
            buffSpell = myBuff.InitializeBuff(transform.parent.gameObject);
            aTarget.GetComponent<PlayerCharacter>().AddBuff(buffSpell, mySpellIcon);
        }
    }

    [ClientRpc]
    private void RpcRemoveBuff(GameObject aTarget)
    {
        Debug.Log("Remove Buff: " + myBuff.name);
        aTarget.GetComponent<PlayerCharacter>().RemoveBuffByName(myBuff.name);
    }
}