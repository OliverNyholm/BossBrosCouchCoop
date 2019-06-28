using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : Character
{
    public float mySpeed;

    protected Subscriber mySubscriber;

    private NavMeshAgent myNavmeshAgent;

    public List<GameObject> myPlayers = new List<GameObject>();
    public List<int> myAggroList = new List<int>();

    [Header("Aggro range")]
    [SerializeField]
    private float myAggroRange = 15.0f;
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
        mySubscriber = new Subscriber();
        mySubscriber.EventOnReceivedMessage += ReceiveMessage;
        PostMaster.Instance.RegisterSubscriber(ref mySubscriber, MessageType.SpellSpawned);
        PostMaster.Instance.RegisterSubscriber(ref mySubscriber, MessageType.PlayerDied);
    }

    void Unsubscribe()
    {
        mySubscriber.EventOnReceivedMessage -= ReceiveMessage;
        PostMaster.Instance.UnregisterSubscriber(ref mySubscriber, MessageType.SpellSpawned);
        PostMaster.Instance.UnregisterSubscriber(ref mySubscriber, MessageType.PlayerDied);
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
                    AddThreat(10, myPlayers[myTargetIndex].GetInstanceID());
                }
                break;
            case State.Combat:
                Behaviour();
                if (myAutoAttackCooldown > 0.0f)
                {
                    myAutoAttackCooldown -= Time.deltaTime * myStats.myAttackSpeed;
                }
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

        transform.GetComponentInChildren<Canvas>().transform.Find("EnemyUI").gameObject.SetActive(false);
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


        const float attackRangeOffset = 1.0f;
        float distanceSqr = (myTarget.transform.position - transform.position).sqrMagnitude;
        float autoAttackRange = GetComponent<Stats>().myAutoAttackRange;
        float moveMinDistance = autoAttackRange - attackRangeOffset;
        if (distanceSqr > moveMinDistance * moveMinDistance)
        {
            Move(myPlayers[myTargetIndex].transform.position);
            myAnimator.SetBool("IsRunning", true);
        }
        else
        {
            myNavmeshAgent.destination = transform.position;
            myAnimator.SetBool("IsRunning", false);
        }

        if (distanceSqr < autoAttackRange * autoAttackRange)
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
        for (int index = 0; index < myPlayers.Count; index++)
        {
            Vector3 playerPosition = myPlayers[index].transform.position;
            float distanceSqr = (playerPosition - transform.position).sqrMagnitude;
            if (distanceSqr < myAggroRange * myAggroRange)
            {
                SetTarget(index);
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

        if (myAutoAttackCooldown > 0.0f)
            return;

        transform.LookAt(new Vector3(myTarget.transform.position.x, transform.position.y, myTarget.transform.position.z));

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
                SetTarget(myTargetIndex);
                AddThreat(2000, aTaunterID);
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

    protected void ReceiveMessage(Message anAiMessage)
    {
        switch (anAiMessage.Type)
        {
            case MessageType.SpellSpawned:
                {
                    int id = (int)anAiMessage.Data.myVector2.x;
                    int value = (int)anAiMessage.Data.myVector2.y;

                    AddThreat(value, id);
                }
                break;
            case MessageType.PlayerDied:
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
