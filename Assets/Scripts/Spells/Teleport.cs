using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Teleport : Spell
{

    [SerializeField]
    float myTeleportDistance = 2.0f;

    protected override void DealSpellEffect()
    {
        if (!isServer)
            return;

        RpcTeleportPlayer(CalculateTeleportPosition());
    }

    private Vector3 CalculateTeleportPosition()
    {
        Transform playerTransform = myParent.transform;
        Vector3 playerDirection = playerTransform.forward;
        Vector3 endPosition = playerTransform.position + playerDirection * myTeleportDistance;

        Ray ray = new Ray(playerTransform.position, playerDirection);
        float distance = Vector3.Distance(myParent.transform.position, endPosition);
        LayerMask layerMask = LayerMask.GetMask("Terrain");

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, distance, layerMask))
        {
            endPosition = hit.point;
        }

        return endPosition;
    }

    [ClientRpc]
    private void RpcTeleportPlayer(Vector3 aPosition)
    {
        myParent.GetComponent<PlayerCharacter>().SetPosition(aPosition);
    }

    protected override string GetSpellDetail()
    {
        string detail = "to teleport forward";

        return detail;
    }
}
