using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : ChannelSpell
{
    private float myRadius;
    private List<SpellOverTime> myActiveBuffs = new List<SpellOverTime>(4);

    protected override void Awake()
    {
        base.Awake();
        myRadius = transform.localScale.x / 2.0f;
    }

    public override void Restart()
    {
        AddBuff(myParent);
        StartCoroutine(gameObject);

        transform.parent = myParent.transform;
    }

    private void OnDisable()
    {
        for (int index = 0; index < myActiveBuffs.Count; index++)
        {
            if(myActiveBuffs[index])
                myActiveBuffs[index].RemoveSpellOverTime(true);
        }

        myActiveBuffs.Clear();
    }

    protected override void Update()
    {
        if (!myTargetHandler)
            return;

       List<GameObject> players = myTargetHandler.GetAllPlayers();
        for (int index = 0; index < players.Count; index++)
        {
            GameObject player = players[index];
            if (!player)
                continue;

            Health health = player.GetComponent<Health>();
            if (health && health.IsDead())
                continue;

            float sqrDistance = Vector3.SqrMagnitude(gameObject.transform.position - player.transform.position);
            if (DoesPlayerHaveBuff(player, out int buffIndex))
            {
                if (sqrDistance > myRadius * myRadius)
                {
                    myActiveBuffs[buffIndex].RemoveSpellOverTime(true);
                    myActiveBuffs.RemoveAt(buffIndex);
                }
            }
            else
            {
                if(sqrDistance <= myRadius * myRadius)
                {
                    AddBuff(player);
                }
            }
        }
    }

    private void StartCoroutine(GameObject aChannelSpell)
    {
        myParent.GetComponent<CastingComponent>().StartChannel(myChannelTime, this, aChannelSpell, 0.0f);
    }

    private void AddBuff(GameObject aPlayer)
    {
        GameObject barrierBuff = PoolManager.Instance.GetPooledObject(mySpawnedOnHit.GetComponent<UniqueID>().GetID());
        if (!barrierBuff)
            return;

        SpellOverTime spellOverTime = barrierBuff.GetComponent<SpellOverTime>();
        spellOverTime.SetParent(myParent);
        spellOverTime.SetTarget(aPlayer);
        spellOverTime.transform.parent = aPlayer.transform;

        myActiveBuffs.Add(spellOverTime);
    }

    private bool DoesPlayerHaveBuff(GameObject aPlayer, out int aBuffIndex)
    {
        for (int index = 0; index < myActiveBuffs.Count; index++)
        {
            if (myActiveBuffs[index].GetTarget() == aPlayer)
            {
                aBuffIndex = index;
                return true;

            }
        }

        aBuffIndex = -1;
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(transform.position, myRadius);
    }
}
