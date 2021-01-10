using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;

public class NPCThreatComponent : MonoBehaviour
{
    private TargetingComponent myTargetingComponent;
    private NPCCastingComponent myCastingComponent;
    private BehaviorTree myBehaviorTree;
    private NPCComponent myNPCComponent;
    private Health myHealth;
    public List<GameObject> Players { get; set; } = new List<GameObject>();
    public List<ThreatHolder> myThreatValues = new List<ThreatHolder>();

    private int myTargetIndex;

    private float myTauntDuration;
    private bool myIsTaunted;

    private Subscriber mySubscriber;

    public delegate void OnTaunted();
    public event OnTaunted EventOnTaunted;

    private void Awake()
    {
        myTargetingComponent = GetComponent<TargetingComponent>();
        myCastingComponent = GetComponent<NPCCastingComponent>();
        myBehaviorTree = GetComponent<BehaviorTree>();
        myNPCComponent = GetComponent<NPCComponent>();
        myHealth = GetComponent<Health>();
        myTargetIndex = -1;
    }

    void Start()
    {
        myHealth.EventOnThreatGenerated += AddThreat;
        myHealth.EventOnHealthZero += ClearAllPlayersThreat;

        Subscribe();
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    void Subscribe()
    {
        mySubscriber = new Subscriber();
        mySubscriber.EventOnReceivedMessage += ReceiveMessage;
        PostMaster.Instance.RegisterSubscriber(ref mySubscriber, MessageCategory.SpellSpawned);
        PostMaster.Instance.RegisterSubscriber(ref mySubscriber, MessageCategory.PlayerDied);
    }

    void Unsubscribe()
    {
        if(mySubscriber == null)
            return;

        mySubscriber.EventOnReceivedMessage -= ReceiveMessage;
        PostMaster.Instance.UnregisterSubscriber(ref mySubscriber, MessageCategory.SpellSpawned);
        PostMaster.Instance.UnregisterSubscriber(ref mySubscriber, MessageCategory.PlayerDied);
    }

    void Update()
    {
        if (myHealth.IsDead())
            return;

        if (myNPCComponent.State != NPCComponent.CombatState.Combat)
            return;

        if (myIsTaunted)
        {
            myTauntDuration -= Time.deltaTime;
            if (myTauntDuration <= 0.0f)
                myIsTaunted = false;
        }

        if (myCastingComponent.IsCasting())
            return;

        DetermineTarget();
    }

    public void DetermineTarget()
    {
        int target = GetHighestAggro();

        if (myTargetIndex != target)
        {
            SetTarget(target);
        }
    }

    private int GetHighestAggro()
    {
        if (myIsTaunted)
            return myTargetIndex;

        int highestThreatIndex = -1;        
        if (myThreatValues.Count == 1)
            return ShouldIgnoreTarget(Players[0]) ? -1 : 0;

        float timeNow = Time.time;
        float highestThreatValue = float.MinValue;
        if (myTargetIndex != -1) //Do this so if everyone is on 0 threat for some reason, the boss will chose the latest target.
        {
            if (ShouldIgnoreTarget(Players[myTargetIndex]))
            {
                myTargetIndex = -1;
            }
            else
            {
                highestThreatValue = myThreatValues[myTargetIndex].CalculateAndTrimThreatAmount(timeNow);
                highestThreatIndex = myTargetIndex;
            }
        }

        for (int index = 0; index < myThreatValues.Count; index++)
        {
            if (myTargetIndex == index)
                continue;

            if (ShouldIgnoreTarget(Players[index]))
                continue;
                
            float threatAmount = myThreatValues[index].CalculateAndTrimThreatAmount(timeNow);
            if (threatAmount > highestThreatValue)
            {
                highestThreatValue = threatAmount;
                highestThreatIndex = index;
            }
        }

        return highestThreatIndex;
    }

    public void PlayerSpotted(GameObject aGameObject)
    {
        GetComponent<NPCComponent>().SetState(NPCComponent.CombatState.Combat);
        AddThreat(10, aGameObject.GetInstanceID(), true);

        SetTarget(GetHighestAggro());
    }

    public void SetTaunt(int aTaunterID, float aDuration)
    {
        myIsTaunted = true;
        myTauntDuration = aDuration;
        for (int index = 0; index < Players.Count; index++)
        {
            if (Players[index].GetInstanceID() == aTaunterID)
            {
                ClearAllPlayersThreat();
                AddThreat(1000, aTaunterID, true);
                EventOnTaunted?.Invoke();

                if (!ShouldIgnoreTarget(Players[myTargetIndex]))
                {
                    myTargetIndex = index;
                    SetTarget(myTargetIndex);
                }

                break;
            }
        }
    }

    private void SetTarget(int aTargetIndex)
    {
        myTargetingComponent.SetTarget(aTargetIndex == -1 ? null : Players[aTargetIndex]);
        myTargetIndex = aTargetIndex;
    }

    public void AddPlayer(GameObject aPlayer)
    {
        Players.Add(aPlayer);
        myThreatValues.Add(new ThreatHolder());
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
        myThreatValues.RemoveAt(anIndex);
        Players.RemoveAt(anIndex);

        if (Players.Count == 0)
            myNPCComponent.SetState(NPCComponent.CombatState.Disengage);
    }

    private void DropTarget()
    {
        myTargetIndex = -1;
        GetComponent<NPCMovementComponent>().Stop();

        myTargetingComponent.SetTarget(null);
    }

    private void AddThreat(int aThreatValue, int anID, bool anIgnoreCombatState)
    {
        if (!anIgnoreCombatState && myNPCComponent.State != NPCComponent.CombatState.Combat)
            return;

        for (int index = 0; index < Players.Count; index++)
        {
            if (Players[index].GetInstanceID() == anID)
            {
                myThreatValues[index].AddThreat(aThreatValue);
                break;
            }
        }
    }

    private void ClearAllPlayersThreat()
    {
        myThreatValues.ForEach(item => { item.ClearAllThreat(); });
    }

    private void ReceiveMessage(Message anAiMessage)
    {
        switch (anAiMessage.Type)
        {
            case MessageCategory.SpellSpawned:
                {
                    int id = (int)anAiMessage.Data.myVector2.x;
                    int value = (int)anAiMessage.Data.myVector2.y;

                    AddThreat(value, id, false);
                }
                break;
            case MessageCategory.PlayerDied:
                {
                    BehaviorTree behaviorTree = GetComponent<BehaviorTree>();
                    if (behaviorTree)
                        behaviorTree.SendEvent("PlayerDied");

                    int id = anAiMessage.Data.myInt;
                    for (int index = 0; index < Players.Count; index++)
                    {
                        if (Players[index].GetInstanceID() == id)
                        {
                            myThreatValues[index].ClearAllThreat();

                            if (index == myTargetIndex)
                                DropTarget();

                            if (AreAllPlayersDead())
                            {
                                ClearAllPlayersThreat();
                                myNPCComponent.SetState(NPCComponent.CombatState.Disengage);
                            }

                            break;
                        }
                    }
                }
                break;
            default:
                break;
        }
    }

    public virtual bool ShouldIgnoreTarget(GameObject aTarget)
    {
        if (aTarget.GetComponent<Health>().IsDead())
            return true;

        return false;
    }
}
