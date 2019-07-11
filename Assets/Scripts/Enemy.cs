﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using BehaviorDesigner.Runtime;

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

    public int PhaseIndex { get; set; }
    public float AutoAttackTimer { get { return myAutoAttackCooldown; } }

    public enum CombatState
    {
        Idle,
        Combat,
        Disengage
    };

    public CombatState State { get; set; }

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

        State = CombatState.Idle;
        PhaseIndex = 1;

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

        switch (State)
        {
            case CombatState.Idle:
                //if (IsTargetCloseBy())
                //{
                //    SetState(CombatState.Combat);
                //    AddThreat(10, myPlayers[myTargetIndex].GetInstanceID());
                //}
                break;
            case CombatState.Combat:
                //Behaviour();
                DetermineTarget();
                if (myAutoAttackCooldown > 0.0f)
                {
                    myAutoAttackCooldown -= Time.deltaTime * myStats.myAttackSpeed;
                }
                break;
            case CombatState.Disengage:
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
            Debug.Log(myName + " failed to cast spell due to already casting spell");
            return false;
        }

        if (GetComponent<Resource>().myCurrentResource < aSpellScript.myResourceCost)
        {
            Debug.Log(myName + " failed to cast spell due to no resources");
            return false;
        }

        if (!aSpellScript.IsCastableWhileMoving() && IsMoving())
        {
            Debug.Log(myName + " failed to cast spell while moving");
            return false;
        }

        if (aSpellScript.myIsOnlySelfCast)
            return true;

        if (!Target)
        {
            if ((aSpellScript.GetSpellTarget() & SpellTarget.Friend) != 0)
            {
                Target = gameObject;
            }
            else
            {
                Debug.Log(myName + " failed to cast spell due to no target");
                return false;
            }
        }

        if (Target.GetComponent<Health>().IsDead())
        {
            Debug.Log(myName + " failed to cast spell due to target being dead");
            return false;
        }

        if (!CanRaycastToTarget())
        {
            Debug.Log(myName + " failed to cast spell due to no vision of target");
            return false;
        }

        float distance = Vector3.Distance(transform.position, Target.transform.position);
        if (distance > aSpellScript.myRange)
        {
            Debug.Log(myName + " failed to cast spell due to target being without of range");
            return false;
        }

        if ((aSpellScript.GetSpellTarget() & SpellTarget.Enemy) == 0 && Target.tag == "Enemy")
        {
            Debug.Log(myName + " failed to cast spell due to target being of type Enemy");
            return false;
        }

        if ((aSpellScript.GetSpellTarget() & SpellTarget.Friend) == 0 && Target.tag == "Player")
        {
            Debug.Log(myName + " failed to cast spell due to target being of type Player");
            return false;
        }

        if (!aSpellScript.myCanCastOnSelf && Target == transform.gameObject)
        {
            Debug.Log(myName + " failed to cast spell due to spell not castable on self");
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

    private void DetermineTarget()
    {
        int target = GetHighestAggro();

        if (myTargetIndex != target)
        {
            SetTarget(target);
        }
    }

    private void Behaviour()
    {
        int target = GetHighestAggro();

        if (myTargetIndex != target)
        {
            SetTarget(target);
        }


        const float attackRangeOffset = 1.0f;
        float distanceSqr = (Target.transform.position - transform.position).sqrMagnitude;
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
            AutoAttack();
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

    public void AutoAttack()
    {
        if (!Target)
            return;

        if (myAutoAttackCooldown > 0.0f)
            return;

        transform.LookAt(new Vector3(Target.transform.position.x, transform.position.y, Target.transform.position.z));

        myAnimator.SetTrigger("Attack");
        myAutoAttackCooldown = 1.5f;

        SpawnSpell(-1, GetSpellSpawnPosition(myClass.GetAutoAttack().GetComponent<Spell>()));
    }

    public bool CastSpell(GameObject aSpell, GameObject aTarget, Transform aSpawnTransform, bool myShouldIgnoreCastability = false)
    {
        Spell spellScript = aSpell.GetComponent<Spell>();

        if (!myShouldIgnoreCastability && !IsAbleToCastSpell(spellScript))
            return false;

        if (spellScript.myCastTime <= 0.0f)
        {
            SpawnSpell(aSpell, aTarget, aSpawnTransform);
            GetComponent<Resource>().LoseResource(spellScript.myResourceCost);
            myAnimator.SetTrigger("CastingDone");
            return true;
        }

        myAnimator.SetBool("IsCasting", true);

        myCastbar.ShowCastbar();
        myCastbar.SetCastbarFillAmount(0.0f);
        myCastbar.SetSpellName(spellScript.myName);
        myCastbar.SetCastbarColor(spellScript.myCastbarColor);
        myCastbar.SetSpellIcon(spellScript.mySpellIcon);
        myCastbar.SetCastTimeText(spellScript.myCastTime.ToString());

        myCastingRoutine = StartCoroutine(CastbarProgress(aSpell, aTarget, aSpawnTransform));

        return true;
    }

    protected IEnumerator CastbarProgress(GameObject aSpell, GameObject aTarget, Transform aSpawnTransform)
    {
        Spell spellScript = aSpell.GetComponent<Spell>();

        GetComponent<AudioSource>().clip = spellScript.GetSpellSFX().myCastSound;
        GetComponent<AudioSource>().Play();


        myIsCasting = true;
        float castSpeed = spellScript.myCastTime / myStats.myAttackSpeed;
        float rate = 1.0f / castSpeed;
        float progress = 0.0f;

        while (progress <= 1.0f)
        {
            myCastbar.SetCastbarFillAmount(Mathf.Lerp(0, 1, progress));
            myCastbar.SetCastTimeText((castSpeed - (progress * castSpeed)).ToString("0.0"));

            progress += rate * Time.deltaTime;

            yield return null;
        }

        StopCasting();

        if (IsAbleToCastSpell(spellScript))
        {
            SpawnSpell(aSpell, aTarget, aSpawnTransform);
            GetComponent<Resource>().LoseResource(spellScript.myResourceCost);
            myAnimator.SetTrigger("CastingDone");
        }
    }

    public void SpawnSpell(GameObject aSpell, GameObject aTarget, Transform aSpawnTransform)
    {
        GameObject instance = Instantiate(aSpell, aSpawnTransform.position, aSpawnTransform.rotation);

        Spell spellScript = instance.GetComponent<Spell>();
        spellScript.SetParent(transform.gameObject);
        spellScript.AddDamageIncrease(myStats.myDamageIncrease);

        spellScript.SetTarget(aTarget);

        if (aTarget && aSpawnTransform.position != aTarget.transform.position)
            GetComponent<AudioSource>().PlayOneShot(spellScript.GetSpellSFX().mySpawnSound);

        GetComponent<BehaviorTree>().SendEvent("SpellSpawned");
    }

    protected override void StopCasting()
    {
        base.StopCasting();
        GetComponent<BehaviorTree>().SendEvent("SpellSpawned");
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
            SetState(CombatState.Disengage);
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
                    GetComponent<BehaviorTree>().SendEvent("PlayerDied");
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
        if (State != CombatState.Combat)
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
        Target = null;
        myTargetIndex = -1;
        myNavmeshAgent.destination = transform.position;

        SetTarget(null);
    }

    public void SetState(CombatState aState)
    {
        State = aState;

        switch (State)
        {
            case CombatState.Idle:
                transform.rotation = mySpawnRotation;
                myAnimator.SetBool("IsRunning", false);
                break;
            case CombatState.Combat:
                break;
            case CombatState.Disengage:
                myNavmeshAgent.destination = mySpawnPosition;
                myAnimator.SetBool("IsRunning", true);
                break;
        }
    }

    private void MoveBackToSpawn()
    {
        if (!myNavmeshAgent.hasPath)
            SetState(CombatState.Idle);
    }

    public void PlayerSpotted(GameObject aGameObject)
    {
        SetState(CombatState.Combat);
        AddThreat(10, aGameObject.GetInstanceID());
        SetTarget(GetHighestAggro());
    }

    protected override void ChangeMyHudHealth(float aHealthPercentage, string aHealthText, int aShieldValue)
    {
        base.ChangeMyHudHealth(aHealthPercentage, aHealthText, aShieldValue);

        if (State != CombatState.Combat && myHealth.GetHealthPercentage() < 1.0f)
        {
            GetComponent<BehaviorTree>().SendEvent("TakeDamage");
            SetState(CombatState.Combat);
        }
    }
}
