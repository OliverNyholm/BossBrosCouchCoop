using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OgreGhost : NPCComponent
{
    private PlayerMovementComponent myPlayerMovementComponent = null;

    private List<GameObject> myCheckpoints = new List<GameObject>(5);

    [SerializeField]
    private List<GameObject> myAvailableCheckpoints = new List<GameObject>(5);

    private bool myShuffleCheckpoints = false;
    public List<GameObject> Players { get; set; }

    private float myDroppedPlayerTimestamp = 0;

    public bool HasPlayer { get { return myPlayerMovementComponent != null; } }
    public bool HasReachedTop { get; set; }

    protected override void Awake()
    {
        base.Awake();

        bool hasCheckpointsManuallySet = myAvailableCheckpoints.Count > 0;

        OgreGhostCheckpoint[] checkpoints = FindObjectsOfType<OgreGhostCheckpoint>();
        foreach (OgreGhostCheckpoint checkpoint in checkpoints)
        {
            myCheckpoints.Add(checkpoint.gameObject);
        }

        myCheckpoints.Sort((first, second) => second.GetComponent<OgreGhostCheckpoint>().myOrder.CompareTo(first.GetComponent<OgreGhostCheckpoint>().myOrder));

        if (!hasCheckpointsManuallySet)
        {
            foreach (GameObject checkpoint in myCheckpoints)
                myAvailableCheckpoints.Add(checkpoint);
        }

        if (myShuffleCheckpoints)
            myAvailableCheckpoints.Shuffle();

        Players = FindObjectOfType<TargetHandler>().GetAllPlayers();

        HasReachedTop = hasCheckpointsManuallySet;
    }

    public override void Reset()
    {
        myPlayerMovementComponent = null;
        GetComponent<Collider>().enabled = true;
        HasReachedTop = false;
    }

    protected override void Update()
    {
        base.Update();

        if (!myPlayerMovementComponent)
            return;

        if (!myPlayerMovementComponent.IsMovementDisabled())
        {
            myPlayerMovementComponent = null;
            return;
        }

        myPlayerMovementComponent.transform.position = transform.position;
    }

    public void OnTriggerEnter(Collider aOther)
    {
        if (myPlayerMovementComponent)
            return;

        if (Time.time - myDroppedPlayerTimestamp < 1.0f)
            return;

        if (!aOther.GetComponent<Player>())
            return;

        if (aOther.GetComponent<Health>().IsDead())
            return;

        PlayerMovementComponent movementComponent = aOther.GetComponent<PlayerMovementComponent>();
        if (movementComponent.IsMovementDisabled())
            return;

        myPlayerMovementComponent = movementComponent;
        myPlayerMovementComponent.SetEnabledMovement(false);
    }

    protected override void OnDeath()
    {
        base.OnDeath();
        GetComponent<Collider>().enabled = false;

        if (myPlayerMovementComponent)
        {
            myPlayerMovementComponent.SetEnabledMovement(true);
            myPlayerMovementComponent = null;
        }
    }

    public List<GameObject> Checkpoints
    {
        get { return myAvailableCheckpoints; }
    }

    public void RemoveLastCheckpoint()
    {
        myAvailableCheckpoints.RemoveAt(myAvailableCheckpoints.Count - 1);
        if(myAvailableCheckpoints.Count == 0)
        {
            foreach (GameObject checkpoint in myCheckpoints)
                myAvailableCheckpoints.Add(checkpoint);

            if (myShuffleCheckpoints)
                myAvailableCheckpoints.Shuffle();
        }
    }

    public void DropPlayer()
    {
        myPlayerMovementComponent.SetEnabledMovement(true);
        myPlayerMovementComponent = null;

        myDroppedPlayerTimestamp = Time.time;
    }
}
