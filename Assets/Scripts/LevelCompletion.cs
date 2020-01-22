using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LevelCompletion : MonoBehaviour
{
    [System.Serializable]
    public class Goal
    {
        public enum GoalType
        {
            Death,
            PickUp
        }

        public GoalType myType;
        public GameObject myGameObject;
    }

    [Header("Goals to complete scene")]
    [SerializeField]
    private List<Goal> myGoals = new List<Goal>();
    private List<bool> myCompletedGoals;

    private Subscriber mySubscriber;

    void Start()
    {
        myCompletedGoals = new List<bool>(myGoals.Count);
        for (int index = 0; index < myCompletedGoals.Capacity; index++)
            myCompletedGoals.Add(false);

        mySubscriber = new Subscriber();
        mySubscriber.EventOnReceivedMessage += ReceiveMessage;
        Subscribe();
    }

    private void OnDestroy()
    {
        mySubscriber.EventOnReceivedMessage -= ReceiveMessage;
        Unsubscribe();
    }

    private void Subscribe()
    {
        PostMaster.Instance.RegisterSubscriber(ref mySubscriber, MessageCategory.EnemyDied);
    }

    private void Unsubscribe()
    {
        PostMaster.Instance.UnregisterSubscriber(ref mySubscriber, MessageCategory.EnemyDied);
    }

    private void ReceiveMessage(Message aMessage)
    {
        bool goalCompleted = false;

        switch (aMessage.Type)
        {
            case MessageCategory.EnemyDied:
                for (int index = 0; index < myGoals.Count; index++)
                {
                    if (myGoals[index].myType == Goal.GoalType.Death && myGoals[index].myGameObject.GetInstanceID() == aMessage.Data.myInt)
                    {
                        goalCompleted = true;
                        myCompletedGoals[index] = true;
                    }
                }
                break;
        }

        if (goalCompleted && AreAllGoalsCompleted())
        {
            Debug.Log("Next Level Unlocked!");
            FindObjectOfType<LevelProgression>().UnlockLevels();
        }
    }

    private bool AreAllGoalsCompleted()
    {
        for (int index = 0; index < myCompletedGoals.Count; index++)
        {
            if (!myCompletedGoals[index])
                return false;
        }

        return true;
    }
}
