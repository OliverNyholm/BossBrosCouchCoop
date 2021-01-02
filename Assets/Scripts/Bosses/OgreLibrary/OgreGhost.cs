using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OgreGhost : NPCComponent
{
    private GameObject myPlayer = null;

    public override void Reset()
    {
        myPlayer = null;
        GetComponent<Collider>().enabled = true;
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

        myPlayer = aOther.gameObject;
        myPlayer.GetComponent<PlayerMovementComponent>().SetEnabledMovement(false);
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
}
