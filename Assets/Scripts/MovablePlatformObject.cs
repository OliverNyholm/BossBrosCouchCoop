using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovablePlatformObject : MonoBehaviour
{
    private MovablePlatform myMovablePlatform = null;

    public void CheckForMovablePlatform()
    {
        float distance = 2.0f;
        Ray ray = new Ray(transform.position + Vector3.up, Vector3.down);
        LayerMask layerMask = LayerMask.GetMask("Terrain");

        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, distance, layerMask))
        {
            MovablePlatform movablePlatform = hitInfo.collider.gameObject.GetComponent<MovablePlatform>();
            if (movablePlatform)
            {
                AddSelfToPlatform(movablePlatform);
            }
        }
    }

    private void OnDisable()
    {
        if (myMovablePlatform)
            RemoveSelfFromPlatform();
    }

    public void AddSelfToPlatform(MovablePlatform aMovablePlatform)
    {
        if (myMovablePlatform)
            RemoveSelfFromPlatform();

        myMovablePlatform = aMovablePlatform;
        myMovablePlatform.AddToPlatform(gameObject);
    }

    public void RemoveSelfFromPlatform()
    {
        myMovablePlatform.RemoveFromPlatform(gameObject);
        myMovablePlatform = null;
    }
}
