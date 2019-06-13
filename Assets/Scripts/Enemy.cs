using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{

    public string myName;
    public float myDamage;

    public GameObject[] mySpells;

    public float mySpeed;
    private float myAutoAttackCooldown;
    private float myAutoAttackCooldownReset = 1.0f;
    private float myTauntDuration;

    private Health myHealth;
    private Stats myStats;
    private List<BuffSpell> myBuffs;

    private Animator myAnimator;

    private NavMeshAgent myNavmeshAgent;

    public List<PlayerCharacter> myPlayerCharacters;
    public List<int> myAggroList;

    private bool myIsCasting;
    private Coroutine myCastingRoutine;

    public GameObject myTarget;
    private int myTargetIndex;

    private bool myIsTaunted;

    AISubscriber myAISubscriber;

    // Use this for initialization
    void Start()
    {
        myHealth = GetComponent<Health>();
        myHealth.EventOnThreatGenerated += AddThreat;
        myHealth.EventOnHealthZero += OnDeath;

        myStats = GetComponent<Stats>();
        myBuffs = new List<BuffSpell>();

        myAnimator = GetComponent<Animator>();

        myNavmeshAgent = GetComponent<NavMeshAgent>();

        myPlayerCharacters = new List<PlayerCharacter>();
        myAggroList = new List<int>();
        myTargetIndex = -1;

        myAISubscriber = new AISubscriber();
        myAISubscriber.EventOnReceivedMessage += ReceiveAIMessage;
        AIPostMaster.Instance.RegisterSubscriber(ref myAISubscriber, AIMessageType.SpellSpawned);
    }

    private void OnDestroy()
    {
        myAISubscriber.EventOnReceivedMessage -= ReceiveAIMessage;
        AIPostMaster.Instance.UnregisterSubscriber(ref myAISubscriber, AIMessageType.SpellSpawned);
    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<Health>().IsDead())
            return;

        HandleBuffs();

        if(myIsTaunted)
        {
            myTauntDuration -= Time.deltaTime;
            if (myTauntDuration <= 0.0f)
                myIsTaunted = false;
        }

        if (myPlayerCharacters.Count <= 0)
            return;

        if (!IsTargetCloseBy())
        {
            myAnimator.SetBool("IsRunning", false);
            myTarget = null;
            myTargetIndex = -1;
            myNavmeshAgent.destination = transform.position;
            return;
        }

        Behaviour();
    }

    private void OnDeath()
    {
        myAnimator.SetTrigger("Death");
        myHealth.EventOnHealthZero -= OnDeath;
    }

    private int GetHighestAggro()
    {
        if (myIsTaunted)
            return myTargetIndex;

        int highestAggro = 0;
        for (int index = 1; index < myAggroList.Count; index++)
        {
            if (myAggroList[index] > myAggroList[highestAggro] && !myPlayerCharacters[index].GetComponent<Health>().IsDead())
                highestAggro = index;
        }

        return highestAggro;
    }

    private void Behaviour()
    {
        int target = GetHighestAggro();

        if (myTargetIndex != target)
        {
            myTargetIndex = target;
            myTarget = myPlayerCharacters[myTargetIndex].gameObject;
        }

        float attackRange = 2.0f;
        if (Vector3.Distance(myTarget.transform.position, transform.position) > attackRange)
        {
            Move(myPlayerCharacters[myTargetIndex].transform.position);
            myAnimator.SetBool("IsRunning", true);
        }
        else
        {
            myNavmeshAgent.destination = transform.position;
            myAnimator.SetBool("IsRunning", false);
            Attack();
        }
    }

    private bool IsTargetCloseBy()
    {
        const float aggroRange = 15.0f;
        for (int index = 0; index < myPlayerCharacters.Count; index++)
        {
            Vector3 playerPosition = myPlayerCharacters[index].transform.position;
            float distance = Vector3.Distance(playerPosition, transform.position);
            if (distance < aggroRange)
            {
                return true;
            }
        }

        return false;
    }

    private void Move(Vector3 aTargetPosition)
    {
        //Vector3 direction = aTargetPosition - transform.position;
        //direction.y = 0.0f;
        //transform.position += direction.normalized * mySpeed * Time.deltaTime;
        myNavmeshAgent.destination = aTargetPosition;
        transform.LookAt(new Vector3(aTargetPosition.x, transform.position.y, aTargetPosition.z));
    }

    private void Attack()
    {
        if (myAutoAttackCooldown > 0.0f)
        {
            myAutoAttackCooldown -= Time.deltaTime * myStats.myAttackSpeed;
            return;
        }

        if (!myTarget)
            return;

        //------- Flail debugging
        //myNetAnimator.SetTrigger("Attack");
        //myAutoAttackCooldown = 5.2f;

        myAnimator.SetTrigger("Attack");
        myAutoAttackCooldown = 1.5f;

        SpawnSpell(0, myTarget.transform.position);
    }

    private void SpawnSpell(int aSpellIndex, Vector3 aPosition)
    {
        GameObject spell = mySpells[aSpellIndex];

        GameObject instance = Instantiate(spell, aPosition + new Vector3(0.0f, 0.5f, 0.0f), transform.rotation);

        Spell spellScript = instance.GetComponent<Spell>();
        spellScript.AddDamageIncrease(myStats.myDamageIncrease);
        spellScript.SetParent(transform.gameObject);

        if (spellScript.myIsOnlySelfCast)
            spellScript.SetTarget(transform.gameObject);
        else if (myTarget)
            spellScript.SetTarget(myTarget);
        else
            spellScript.SetTarget(transform.gameObject);
    }

    private void HandleBuffs()
    {
        for (int index = 0; index < myBuffs.Count; index++)
        {
            myBuffs[index].Tick();
            if (myBuffs[index].IsFinished())
            {
                RemoveBuff(index);
            }
            else if (myBuffs[index].GetBuff().mySpellType == SpellType.HOT)
            {
                BuffTickSpell hot = myBuffs[index] as BuffTickSpell;
                if (hot.ShouldDealTickSpellEffect)
                {
                    myHealth.TakeDamage(hot.GetTickValue());
                    hot.ShouldDealTickSpellEffect = false;
                }
            }
            else if (myBuffs[index].GetBuff().mySpellType == SpellType.DOT)
            {
                BuffTickSpell dot = myBuffs[index] as BuffTickSpell;
                if (dot.ShouldDealTickSpellEffect)
                {
                    myHealth.TakeDamage(dot.GetTickValue());
                    dot.ShouldDealTickSpellEffect = false;
                }
            }
        }
    }

    public void AddBuff(BuffSpell aBuffSpell, Sprite aSpellIcon)
    {
        for (int index = 0; index < myBuffs.Count; index++)
        {
            if (myBuffs[index].GetParent() == aBuffSpell.GetParent() &&
                myBuffs[index].GetBuff() == aBuffSpell.GetBuff())
            {
                RemoveBuff(index);
                break;
            }
        }

        myBuffs.Add(aBuffSpell);
        aBuffSpell.GetBuff().ApplyBuff(ref myStats);

        if (aBuffSpell.GetBuff().myAttackSpeed != 0.0f)
        {
            float attackspeed = myAutoAttackCooldownReset / myStats.myAttackSpeed;

            float currentAnimationSpeed = 1.0f;
            if (currentAnimationSpeed > attackspeed)
            {
                myAnimator.SetFloat("AutoAttackSpeed", myAnimator.GetFloat("AutoAttackSpeed") + attackspeed / currentAnimationSpeed);
            }
        }
        else if (aBuffSpell.GetBuff().mySpeedMultiplier != 0.0f)
        {
            myAnimator.SetFloat("RunSpeed", myStats.mySpeedMultiplier);
        }
    }

    public void SetTaunt(int aTaunterID, float aDuration)
    {
        myIsTaunted = true;
        myTauntDuration = aDuration;
        for (int index = 0; index < myPlayerCharacters.Count; index++)
        {
            if(myPlayerCharacters[index].GetInstanceID() == aTaunterID)
            {
                myTargetIndex = index;
                Debug.Log(myTargetIndex + " taunted enemy for  " + aDuration);
                break;
            }
        }
    }

    private void RemoveBuff(int anIndex)
    {
        myBuffs[anIndex].GetBuff().EndBuff(ref myStats);
        if (myBuffs[anIndex].GetBuff().mySpellType == SpellType.Shield)
        {
            myBuffs[anIndex].GetBuff().myDuration = 0.0f;
            myHealth.RemoveShield();
        }

        if (myBuffs[anIndex].GetBuff().myAttackSpeed != 0.0f)
        {
            float attackspeed = myAutoAttackCooldownReset / myStats.myAttackSpeed;

            float currentAnimationSpeed = 1.0f;
            if (currentAnimationSpeed > attackspeed)
            {
                myAnimator.SetFloat("AutoAttackSpeed", myAnimator.GetFloat("AutoAttackSpeed") + attackspeed / currentAnimationSpeed);
            }
            else
            {
                myAnimator.SetFloat("AutoAttackSpeed", 1.0f);
            }
        }
        else if (myBuffs[anIndex].GetBuff().mySpeedMultiplier != 0.0f)
        {
            myAnimator.SetFloat("RunSpeed", myStats.mySpeedMultiplier);
        }

        myBuffs.RemoveAt(anIndex);
    }

    public void RemoveBuffByName(string aName)
    {
        for (int index = 0; index < myBuffs.Count; index++)
        {
            if (myBuffs[index].GetName() == aName)
            {
                RemoveBuff(index);
                return;
            }
        }
    }

    public void InterruptSpellCast()
    {
        if (myIsCasting)
        {
            StopCasting();
        }
    }

    private void StopCasting()
    {
        if (myCastingRoutine != null)
            StopCoroutine(myCastingRoutine);
        myIsCasting = false;
    }

    public void AddPlayerCharacter(GameObject aPlayerCharacter)
    {
        myPlayerCharacters.Add(aPlayerCharacter.GetComponent<PlayerCharacter>());
        myAggroList.Add(0);
    }

    public void RemovePlayerCharacter(GameObject aPlayerCharacter)
    {
        myAggroList.Remove(myPlayerCharacters.IndexOf(aPlayerCharacter.GetComponent<PlayerCharacter>()));
        myPlayerCharacters.Remove(aPlayerCharacter.GetComponent<PlayerCharacter>());
    }

    private void ReceiveAIMessage(AIMessage anAiMessage)
    {
        switch (anAiMessage.Type)
        {
            case AIMessageType.SpellSpawned:
                {
                    int id = anAiMessage.Data.myObjectID;
                    int value = anAiMessage.Data.myInt;

                    AddThreat(value, id);
                }
                break;
            default:
                break;
        }
    }

    private void AddThreat(int aThreatValue, int anID)
    {
        for (int index = 0; index < myPlayerCharacters.Count; index++)
        {
            if (myPlayerCharacters[index].GetInstanceID() == anID)
            {
                myAggroList[index] += aThreatValue;
                break;
            }
        }
    }
}
