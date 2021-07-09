using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TutorialOldGnome : MonoBehaviour
{
    private NPCNavAgent myNavAgent = null;
    private DialogueCanvas myDialogueCanvas = null;

    [System.Serializable]
    private struct TutorialWaypoints
    {
        public Transform myTargetTransform;
        public Transform myActivateTargetTransform;
        public TutorialCompletion myTutorial;
        public TutorialTrigger myActivateHitbox;
        public TutorialGuideData myGuideData;
    }

    [SerializeField]
    private List<TutorialWaypoints> myWaypoints = new List<TutorialWaypoints>(10);
    private int myWaypointIndex = -1;
    private bool myHasActivatedDialogueShown = false;

    private void Awake()
    {
        myNavAgent = GetComponent<NPCNavAgent>();
        myDialogueCanvas = FindObjectOfType<DialogueCanvas>();
    }

    private void Start()
    {
        myNavAgent.OnStoppedMoving += OnStoppedMoving;
        MoveToNextWaypoint();
    }


    private void MoveToNextWaypoint()
    {
        myWaypointIndex++;
        myHasActivatedDialogueShown = false;
        TutorialWaypoints waypointData = myWaypoints[myWaypointIndex];

        myNavAgent.Move(waypointData.myTargetTransform.position);

        if (waypointData.myActivateHitbox)
            waypointData.myActivateHitbox.OnTriggerHit += OnActivateTriggerHit;

        if (waypointData.myTutorial)
            waypointData.myTutorial.OnTutorialCompleted += OnTutorialCompleted;
    }

    private void OnStoppedMoving()
    {
        TutorialWaypoints waypointData = myWaypoints[myWaypointIndex];
        transform.rotation = waypointData.myTargetTransform.rotation;

        if (!waypointData.myActivateHitbox && waypointData.myGuideData.myOnActivate && !myHasActivatedDialogueShown)
        {
            myDialogueCanvas.AddDialogue(waypointData.myGuideData.myOnActivate, Color.white);
            myHasActivatedDialogueShown = true;
        }
    }

    private void OnActivateTriggerHit()
    {
        TutorialWaypoints waypointData = myWaypoints[myWaypointIndex];
        waypointData.myActivateHitbox.OnTriggerHit -= OnActivateTriggerHit;

        if (waypointData.myGuideData.myOnActivate)
            myDialogueCanvas.AddDialogue(waypointData.myGuideData.myOnActivate, Color.white);

        if (waypointData.myActivateTargetTransform)
            myNavAgent.Move(waypointData.myActivateTargetTransform.position);
    }

    private void OnTutorialCompleted()
    {
        TutorialWaypoints waypointData = myWaypoints[myWaypointIndex];
        waypointData.myTutorial.OnTutorialCompleted -= OnTutorialCompleted;

        if (waypointData.myGuideData.myOnCompleteData)
            myDialogueCanvas.AddDialogue(waypointData.myGuideData.myOnCompleteData, Color.white);

        MoveToNextWaypoint();
    }
}
