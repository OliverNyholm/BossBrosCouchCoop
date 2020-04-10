using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using BehaviorDesigner.Runtime;

public class Enemy : Character
{
    public float mySpeed;

    protected Subscriber mySubscriber;

    private NavMeshAgent myNavmeshAgent;
    private BehaviorTree myBehaviorTree;

    public List<GameObject> Players { get; set; } = new List<GameObject>();
    public List<int> myAggroList = new List<int>();

    [Header("Aggro range")]
    [SerializeField]
    private float myAggroRange = 15.0f;
    private int myTargetIndex;

    private Vector3 mySpawnPosition;
    private Quaternion mySpawnRotation;

    private float myTauntDuration;

    private bool myIsTaunted;

    private GameObject mySpellTarget;

    private TargetHandler myTargetHandler;

    public int PhaseIndex { get; set; }

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
        if (myNavmeshAgent)
            myNavmeshAgent.speed = mySpeed;

        myBehaviorTree = GetComponent<BehaviorTree>();

        myTargetIndex = -1;

        mySpawnPosition = transform.position;
        mySpawnRotation = transform.rotation;

        State = CombatState.Idle;
        PhaseIndex = 1;

        myTargetHandler = FindObjectOfType<TargetHandler>();

        Subscribe();
    }

    protected virtual void OnDestroy()
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
        PostMaster.Instance.RegisterSubscriber(ref mySubscriber, MessageCategory.SpellSpawned);
        PostMaster.Instance.RegisterSubscriber(ref mySubscriber, MessageCategory.PlayerDied);
        PostMaster.Instance.RegisterSubscriber(ref mySubscriber, MessageCategory.EnteredCombat);
        PostMaster.Instance.RegisterSubscriber(ref mySubscriber, MessageCategory.EnemyDied);
    }

    void Unsubscribe()
    {
        mySubscriber.EventOnReceivedMessage -= ReceiveMessage;
        PostMaster.Instance.UnregisterSubscriber(ref mySubscriber, MessageCategory.SpellSpawned);
        PostMaster.Instance.UnregisterSubscriber(ref mySubscriber, MessageCategory.PlayerDied);
        PostMaster.Instance.UnregisterSubscriber(ref mySubscriber, MessageCategory.EnteredCombat);
        PostMaster.Instance.UnregisterSubscriber(ref mySubscriber, MessageCategory.EnemyDied);
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




    private int GetHighestAggro()
    {
        if (myIsTaunted)
            return myTargetIndex;

        int highestAggro = 0;
        if (myAggroList.Count == 1)
            return highestAggro;

        for (int index = 1; index < myAggroList.Count; index++)
        {
            if (myAggroList[index] > myAggroList[highestAggro] && !Players[index].GetComponent<Health>().IsDead())
                highestAggro = index;
        }

        return highestAggro;
    }

    private void DetermineTarget()
    {
        if (myIsCasting)
            return;

        int target = GetHighestAggro();

        if (myTargetIndex != target)
        {
            SetTarget(target);
        }
    }

    private void SetTarget(int aTargetIndex)
    {
        SetTarget(Players[aTargetIndex].gameObject);
        myTargetIndex = aTargetIndex;
    }

    public void SetSpellTarget(GameObject aTarget)
    {
        myTargetIndex = -1;
        mySpellTarget = aTarget;
        if (myHUDTarget)
            UnsubscribePreviousTargetHUD();

        if (mySpellTarget)
            SetTargetHUD(mySpellTarget);
        else
            myTargetHUD.Hide();
    }

    public void AutoAttack()
    {
        if (!Target)
            return;

        if (myAutoAttackCooldown > 0.0f)
            return;

        transform.LookAt(new Vector3(Target.transform.position.x, transform.position.y, Target.transform.position.z));

        myAnimator.SetTrigger("AutoAttack");
        myAutoAttackCooldown = 1.5f;

        SpawnSpell(-1, GetSpellSpawnPosition(myClass.GetAutoAttack().GetComponent<Spell>()));
    }


    public void SetTaunt(int aTaunterID, float aDuration)
    {
        myIsTaunted = true;
        myTauntDuration = aDuration;
        for (int index = 0; index < Players.Count; index++)
        {
            if (Players[index].GetInstanceID() == aTaunterID)
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
        Players.Add(aPlayer);
        myAggroList.Add(0);
    }

    private bool AreAllPlayersDead()
    {
        for (int index = 0; index < Players.Count; index++)
        {
            if (!Players[index].GetComponent<Health>().IsDead())
                return false;
        }

        return true;
    }

    public void RemovePlayer(int anIndex)
    {
        myAggroList.RemoveAt(anIndex);
        Players.RemoveAt(anIndex);

        if (Players.Count == 0)
            SetState(CombatState.Disengage);
    }

    protected void ReceiveMessage(Message anAiMessage)
    {
        switch (anAiMessage.Type)
        {
            case MessageCategory.SpellSpawned:
                {
                    int id = (int)anAiMessage.Data.myVector2.x;
                    int value = (int)anAiMessage.Data.myVector2.y;

                    AddThreat(value, id);
                }
                break;
            case MessageCategory.PlayerDied:
                {
                    if (myBehaviorTree)
                        myBehaviorTree.SendEvent("PlayerDied");

                    int id = anAiMessage.Data.myInt;
                    for (int index = 0; index < Players.Count; index++)
                    {
                        if (Players[index].GetInstanceID() == id)
                        {
                            if (index == myTargetIndex)
                                DropTarget();

                            if (AreAllPlayersDead())
                                SetState(CombatState.Disengage);

                            //RemovePlayer(index);
                            break;
                        }
                    }
                }
                break;
            case MessageCategory.EnteredCombat:
                if (State == CombatState.Idle)
                    SetState(CombatState.Combat);
                break;
            case MessageCategory.EnemyDied:
                if (myBehaviorTree)
                    myBehaviorTree.SendEvent(myTargetHandler.GetEnemyName(anAiMessage.Data.myInt) + "Died");
                break;
            default:
                break;
        }
    }

    private void AddThreat(int aThreatValue, int anID)
    {
        if (State != CombatState.Combat)
            return;

        for (int index = 0; index < Players.Count; index++)
        {
            if (Players[index].GetInstanceID() == anID)
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
        if (myNavmeshAgent)
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
                PostMaster.Instance.PostMessage(new Message(MessageCategory.EnteredCombat));
                break;
            case CombatState.Disengage:
                if (myNavmeshAgent)
                    myNavmeshAgent.destination = mySpawnPosition;
                myAnimator.SetBool("IsRunning", true);

                if (myBehaviorTree)
                {
                    myBehaviorTree.DisableBehavior();
                    myBehaviorTree.EnableBehavior();
                }
                break;
        }
    }

    private void MoveBackToSpawn()
    {
        if (myNavmeshAgent && !myNavmeshAgent.hasPath)
            SetState(CombatState.Idle);
    }

    public void PlayerSpotted(GameObject aGameObject)
    {
        SetState(CombatState.Combat);
        AddThreat(10, aGameObject.GetInstanceID());
        SetTarget(GetHighestAggro());
    }

    public void SetBossHud(GameObject aBossHudGameObject)
    {
        GetComponent<UIComponent>().SetupHud(aBossHudGameObject.transform);
    }
}
