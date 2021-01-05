using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OgreGhost : NPCComponent
{
    private GameObject myPlayer = null;

    private List<GameObject> myCheckpoints = new List<GameObject>(5);
    private List<GameObject> myAvailableCheckpoints = new List<GameObject>(5);

    public bool HasReachedTop { get; set; }

    protected override void Awake()
    {
        base.Awake();

        OgreGhostCheckpoint[] checkpoints = FindObjectsOfType<OgreGhostCheckpoint>();
        foreach (OgreGhostCheckpoint checkpoint in checkpoints)
        {
            myCheckpoints.Add(checkpoint.gameObject);
            myAvailableCheckpoints.Add(checkpoint.gameObject);
        }

        myAvailableCheckpoints.Shuffle();
    }

    public override void Reset()
    {
        myPlayer = null;
        GetComponent<Collider>().enabled = true;
        HasReachedTop = false;
    }

    protected override void Update()
    {
        base.Update();

        if (myPlayer)
            myPlayer.transform.position = transform.position;
    }

    public void OnTriggerEnter(Collider aOther)
    {
        if (myPlayer)
            return;

        if (!aOther.GetComponent<Player>())
            return;

        PlayerMovementComponent movementComponent = aOther.GetComponent<PlayerMovementComponent>();
        if (movementComponent.IsMovementDisabled())
            return;

        myPlayer = aOther.gameObject;
        movementComponent.SetEnabledMovement(false);
    }

    protected override void OnDeath()
    {
        base.OnDeath();
        GetComponent<Collider>().enabled = false;

        if (myPlayer)
        {
            myPlayer.GetComponent<PlayerMovementComponent>().SetEnabledMovement(true);
            myPlayer = null;
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
            {
                myAvailableCheckpoints.Add(checkpoint);
            }

            myAvailableCheckpoints.Shuffle();
        }
    }
}
