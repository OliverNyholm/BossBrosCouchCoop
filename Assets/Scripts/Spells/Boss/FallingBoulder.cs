using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingBoulder : Spell
{
    [Header("The Model to rotate")]
    [SerializeField]
    private Transform myMeshTransform = null;

    [SerializeField]
    private Transform myDecalTransform = null;

    private Vector3 myLandPosition;

    List<int> myTargetsHit = new List<int>();

    public override void Reset()
    {
        base.Reset();
        myTargetsHit.Clear();
    }

    protected override void OnFirstUpdate()
    {
        base.OnFirstUpdate();

        RaycastHit hit;
        Ray ray = new Ray(transform.position, Vector3.down);
        const float maxDistance = 100.0f;
        LayerMask layerMask = LayerMask.GetMask("Terrain");

        if (Physics.Raycast(ray, out hit, maxDistance, layerMask, QueryTriggerInteraction.Collide))
        {
            myLandPosition = hit.point;
        }

        myRotationSpeed = Random.Range(0.3f, 1.5f);
        myRandomRotation = Random.rotation.eulerAngles;
        myDecalTransform.transform.position = myLandPosition;
    }

    protected override void Update()
    {
        if (myIsFirstUpdate)
            OnFirstUpdate();

        myMeshTransform.Rotate(myRandomRotation * myRotationSpeed * Time.deltaTime);

        transform.position += Vector3.down * mySpeed * Time.deltaTime;
        myDecalTransform.transform.position = myLandPosition;
        if (transform.position.y <= myLandPosition.y)
        {
            SpawnVFX(2.5f);
            ReturnToPool();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player")
            return;

        int instanceID = other.gameObject.GetInstanceID();
        if (myTargetsHit.Contains(instanceID))
            return;

        myTargetsHit.Add(instanceID);
        
        DealDamage(myDamage, other.gameObject);
    }
}
