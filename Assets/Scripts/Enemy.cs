﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : Character
{
    public string myName;

    public float mySpeed;

    private AISubscriber myAISubscriber;

    private NavMeshAgent myNavmeshAgent;

    public List<GameObject> myPlayers = new List<GameObject>();
    public List<int> myAggroList = new List<int>();


    private int myTargetIndex;

    private Vector3 mySpawnPosition;
    private Quaternion mySpawnRotation;

    private float myTauntDuration;

    private bool myIsTaunted;

    enum State
    {
        Idle,
        Combat,
        Disengage
    };

    private State myState;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();

        myHealth.EventOnThreatGenerated += AddThreat;

        myNavmeshAgent = GetComponent<NavMeshAgent>();
        myNavmeshAgent.speed = mySpeed;

        myTargetIndex = -1;

        mySpawnPosition = transform.position;
        mySpawnRotation = transform.rotation;

        myState = State.Idle;

        SetupHud(transform.GetComponentInChildren<Canvas>().transform.Find("EnemyUI").transform);

        Subscribe();
    }

    private void OnDestroy()
    {
        Unsubscribe();

        if (myHealth)
        {
            myHealth.EventOnThreatGenerated -= AddThreat;
        }
    }

    void Subscribe()
    {
        myAISubscriber = new AISubscriber();
        myAISubscriber.EventOnReceivedMessage += ReceiveAIMessage;
        AIPostMaster.Instance.RegisterSubscriber(ref myAISubscriber, AIMessageType.SpellSpawned);
        AIPostMaster.Instance.RegisterSubscriber(ref myAISubscriber, AIMessageType.PlayerDied);
    }

    void Unsubscribe()
    {
        myAISubscriber.EventOnReceivedMessage -= ReceiveAIMessage;
        AIPostMaster.Instance.UnregisterSubscriber(ref myAISubscriber, AIMessageType.SpellSpawned);
        AIPostMaster.Instance.UnregisterSubscriber(ref myAISubscriber, AIMessageType.PlayerDied);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (GetComponent<Health>().IsDead())
            return;

        if (myIsTaunted)
        {
            myTauntDuration -= Time.deltaTime;
            if (myTauntDuration <= 0.0f)
                myIsTaunted = false;
        }

        switch (myState)
        {
            case State.Idle:
                if (IsTargetCloseBy())
                {
                    SetState(State.Combat);
                }
                break;
            case State.Combat:
                Behaviour();
                break;
            case State.Disengage:
                MoveBackToSpawn();
                break;
            default:
                break;
        }
    }

    protected override void OnDeath()
    {
        base.OnDeath();

        myNavmeshAgent.isStopped = true;
    }

    protected override bool IsMoving()
    {
        if (myNavmeshAgent.hasPath)
            return true;

        return false;
    }

    protected override bool IsAbleToCastSpell(Spell aSpellScript)
    {
        if (myIsCasting)
        {
            return false;
        }

        if (GetComponent<Resource>().myCurrentResource < aSpellScript.myResourceCost)
        {
            return false;
        }

        if (!aSpellScript.IsCastableWhileMoving() && IsMoving())
        {
            return false;
        }

        if (aSpellScript.myIsOnlySelfCast)
            return true;

        if (!myTarget)
        {
            if ((aSpellScript.GetSpellTarget() & SpellTarget.Friend) != 0)
            {
                myTarget = gameObject;
            }
            else
            {
                return false;
            }
        }

        if (myTarget.GetComponent<Health>().IsDead())
        {
            return false;
        }

        if (!CanRaycastToTarget())
        {
            return false;
        }

        float distance = Vector3.Distance(transform.position, myTarget.transform.position);
        if (distance > aSpellScript.myRange)
        {
            return false;
        }

        if ((aSpellScript.GetSpellTarget() & SpellTarget.Enemy) == 0 && myTarget.tag == "Enemy")
        {
            return false;
        }

        if ((aSpellScript.GetSpellTarget() & SpellTarget.Friend) == 0 && myTarget.tag == "Player")
        {
            return false;
        }

        if (!aSpellScript.myCanCastOnSelf && myTarget == transform.gameObject)
        {
            return false;
        }

        return true;
    }

    private int GetHighestAggro()
    {
        if (myIsTaunted)
            return myTargetIndex;

        int highestAggro = 0;
        if (myAggroList.Count == 1)
            return highestAggro;

        for (int index = 1; index < myAggroList.Count; index++)
        {
            if (myAggroList[index] > myAggroList[highestAggro] && !myPlayers[index].GetComponent<Health>().IsDead())
                highestAggro = index;
        }

        return highestAggro;
    }

    private void Behaviour()
    {
        int target = GetHighestAggro();

        if (myTargetIndex != target)
        {
            SetTarget(target);
        }

        bool isCurrentlyAutoAttacking = myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Attack");
        const float attackRangeOffset = 0.5f;
        if (!isCurrentlyAutoAttacking && Vector3.Distance(myTarget.transform.position, transform.position) > GetComponent<Stats>().myAutoAttackRange - attackRangeOffset)
        {
            Move(myPlayers[myTargetIndex].transform.position);
            myAnimator.SetBool("IsRunning", true);
        }
        else
        {
            myNavmeshAgent.destination = transform.position;
            myAnimator.SetBool("IsRunning", false);
            Attack();
        }

        if (myClass.myCooldownTimers[0] <= 0.0f)
        {
            int randomPlayer = Random.Range(0, myPlayers.Count - 1);

            Vector3 playerPosition = myPlayers[randomPlayer].transform.position;
            Vector3 offsetAbove = Vector3.up * 80.0f;

            SpawnSpell(0, playerPosition + offsetAbove);
            myClass.SetSpellOnCooldown(0);
            GetComponent<Resource>().LoseResource(myClass.mySpells[0].GetComponent<Spell>().myResourceCost);
        }
    }

    private void SetTarget(int aTargetIndex)
    {
        myTargetIndex = aTargetIndex;
        SetTarget(myPlayers[myTargetIndex].gameObject);
    }

    private bool IsTargetCloseBy()
    {
        const float aggroRange = 15.0f;
        for (int index = 0; index < myPlayers.Count; index++)
        {
            Vector3 playerPosition = myPlayers[index].transform.position;
            float distance = Vector3.Distance(playerPosition, transform.position);
            if (distance < aggroRange)
            {
                SetTarget(index);
                AddThreat(10, index);
                return true;
            }
        }

        return false;
    }

    private void Move(Vector3 aTargetPosition)
    {
        myNavmeshAgent.destination = aTargetPosition;
    }

    private void Attack()
    {
        if (!myTarget)
            return;

        transform.LookAt(new Vector3(myTarget.transform.position.x, transform.position.y, myTarget.transform.position.z));
        if (myAutoAttackCooldown > 0.0f)
        {
            myAutoAttackCooldown -= Time.deltaTime * myStats.myAttackSpeed;
            return;
        }

        //------- Flail debugging
        //myNetAnimator.SetTrigger("Attack");
        //myAutoAttackCooldown = 5.2f;

        myAnimator.SetTrigger("Attack");
        myAutoAttackCooldown = 1.5f;

        SpawnSpell(-1, GetSpellSpawnPosition(myClass.GetAutoAttack().GetComponent<Spell>()));
    }

   

    public void SetTaunt(int aTaunterID, float aDuration)
    {
        myIsTaunted = true;
        myTauntDuration = aDuration;
        for (int index = 0; index < myPlayers.Count; index++)
        {
            if (myPlayers[index].GetInstanceID() == aTaunterID)
            {
                myTargetIndex = index;
                Debug.Log(myTargetIndex + " taunted enemy for  " + aDuration);
                AddThreat(5000, aTaunterID);
                break;
            }
        }
    }

    public void AddPlayer(GameObject aPlayer)
    {
        myPlayers.Add(aPlayer);
        myAggroList.Add(0);
    }

    public void RemovePlayer(int anIndex)
    {
        myAggroList.RemoveAt(anIndex);
        myPlayers.RemoveAt(anIndex);

        if (myPlayers.Count == 0)
            SetState(State.Disengage);
    }

    private void ReceiveAIMessage(AIMessage anAiMessage)
    {
        switch (anAiMessage.Type)
        {
            case AIMessageType.SpellSpawned:
                {
                    int id = (int)anAiMessage.Data.myVector2.x;
                    int value = (int)anAiMessage.Data.myVector2.y;

                    AddThreat(value, id);
                }
                break;
            case AIMessageType.PlayerDied:
                {
                    int id = anAiMessage.Data.myInt;
                    for (int index = 0; index < myPlayers.Count; index++)
                    {
                        if (myPlayers[index].GetInstanceID() == id)
                        {
                            if (index == myTargetIndex)
                                DropTarget();

                            RemovePlayer(index);
                            break;
                        }
                    }
                }
                break;
            default:
                break;
        }
    }

    private void AddThreat(int aThreatValue, int anID)
    {
        if (myState != State.Combat)
            return;

        for (int index = 0; index < myPlayers.Count; index++)
        {
            if (myPlayers[index].GetInstanceID() == anID)
            {
                myAggroList[index] += aThreatValue;
                break;
            }
        }
    }

    private void DropTarget()
    {
        myTarget = null;
        myTargetIndex = -1;
        myNavmeshAgent.destination = transform.position;

        SetTarget(null);
    }

    private void SetState(State aState)
    {
        myState = aState;

        switch (myState)
        {
            case State.Idle:
                transform.rotation = mySpawnRotation;
                myAnimator.SetBool("IsRunning", false);
                myHealth.GainHealth(100000);
                break;
            case State.Combat:
                break;
            case State.Disengage:
                myNavmeshAgent.destination = mySpawnPosition;
                myAnimator.SetBool("IsRunning", true);
                break;
        }
    }

    private void MoveBackToSpawn()
    {
        if (!myNavmeshAgent.hasPath)
            SetState(State.Idle);
    }

    protected override void ChangeMyHudHealth(float aHealthPercentage, string aHealthText, int aShieldValue)
    {
        base.ChangeMyHudHealth(aHealthPercentage, aHealthText, aShieldValue);

        if (myState != State.Combat && myHealth.GetHealthPercentage() < 1.0f)
            SetState(State.Combat);
    }
}
