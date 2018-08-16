using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Tug : Spell
{

    [SerializeField]
    float myInitialAngle = 30.0f;

    protected override void DealSpellEffect()
    {
        if (!isServer)
            return;

        RpcTugPlayer(CalculateTugImpact());
    }

    private Vector3 CalculateTugImpact()
    {
        float gravity = myParent.GetComponent<PlayerCharacter>().myGravity;
        // Selected angle in radians
        float angle = myInitialAngle * Mathf.Deg2Rad;

        // Positions of this object and the target on the same plane
        Vector3 planarTarget = new Vector3(myTarget.transform.position.x, 0, myTarget.transform.position.z);
        Vector3 planarPostion = new Vector3(myParent.transform.position.x, 0, myParent.transform.position.z);

        // Planar distance between objects
        float distance = Vector3.Distance(planarTarget, planarPostion);
        // Distance along the y axis between objects
        float yOffset = transform.position.y - myTarget.transform.position.y;

        float initialVelocity = (1 / Mathf.Cos(angle)) * Mathf.Sqrt((0.5f * gravity * Mathf.Pow(distance, 2)) / (distance * Mathf.Tan(angle) + yOffset));

        Vector3 velocity = new Vector3(0, initialVelocity * Mathf.Sin(angle), initialVelocity * Mathf.Cos(angle));

        // Rotate our velocity to match the direction between the two objects
        float angleBetweenObjects = Vector3.Angle(Vector3.forward, planarTarget - planarPostion);
        Vector3 finalVelocity = Quaternion.AngleAxis(angleBetweenObjects, Vector3.up) * velocity;

        //Have to inverse in x, since angleBetweenObjects seems to ignore that fact //Oliver
        if (myTarget.transform.position.x < myParent.transform.position.x)
            finalVelocity.x *= -1;

        finalVelocity.x *= -1;
        finalVelocity.z *= -1;

        // Fire!
        return finalVelocity;
    }

    [ClientRpc]
    private void RpcTugPlayer(Vector3 aVelocity)
    {
        myTarget.GetComponent<PlayerCharacter>().GiveImpulse(aVelocity, false);
    }
}
