using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : Spell
{

    [SerializeField]
    private GameObject myBarrierPrefab = null;

    [SerializeField]
    private float myChannelTime = 0.0f;

    private float myRadius;
    private TargetHandler myTargetHandler;
    private List<SpellOverTime> myActiveBuffs = new List<SpellOverTime>(4);

    private void Awake()
    {
        myRadius = transform.localScale.x / 2.0f;
        myTargetHandler = FindObjectOfType<TargetHandler>();
    }

    protected override void Start()
    {
        AddBuff(myParent);
        StartCoroutine(gameObject);
    }

    private void OnDisable()
    {
        for (int index = 0; index < myActiveBuffs.Count; index++)
        {
            myActiveBuffs[index].RemoveSpellOverTime();
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
            float sqrDistance = Vector3.SqrMagnitude(gameObject.transform.position - players[index].transform.position);
            if (DoesPlayerHaveBuff(players[index], out int buffIndex))
            {
                if (sqrDistance > myRadius * myRadius)
                {
                    myActiveBuffs[buffIndex].RemoveSpellOverTime();
                    myActiveBuffs.RemoveAt(buffIndex);
                }
            }
            else
            {
                if(sqrDistance <= myRadius * myRadius)
                {
                    AddBuff(players[index]);
                }
            }
        }
    }

    private void StartCoroutine(GameObject aChannelSpell)
    {
        myParent.GetComponent<CastingComponent>().StartChannel(myChannelTime, this, aChannelSpell);
    }

    public override void CreatePooledObjects(PoolManager aPoolManager, int aSpellMaxCount)
    {
        base.CreatePooledObjects(aPoolManager, aSpellMaxCount);

        if(myBarrierPrefab)
        aPoolManager.AddPoolableObjects(myBarrierPrefab, myBarrierPrefab.GetComponent<UniqueID>().GetID(), aSpellMaxCount);
    }

    private void AddBuff(GameObject aPlayer)
    {
        GameObject barrierBuff = PoolManager.Instance.GetPooledObject(mySpawnedOnHit.GetComponent<UniqueID>().GetID());
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
