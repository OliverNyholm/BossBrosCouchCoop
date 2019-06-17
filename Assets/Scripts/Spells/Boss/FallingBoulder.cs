using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingBoulder : Spell
{
    [Header("The Model to rotate")]
    [SerializeField]
    private Transform myMeshTransform;

    private Vector3 myLandPosition;
    private Vector3 myRandomRotation;
    private float myRotationSpeed;

    List<int> myTargetsHit = new List<int>();

    void Start()
    {
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
    }

    protected override void Update()
    {
        myMeshTransform.Rotate(myRandomRotation * myRotationSpeed * Time.deltaTime);

        transform.position += Vector3.down * mySpeed * Time.deltaTime;
        if (transform.position.y <= myLandPosition.y)
            Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player")
            return;

        int instanceID = other.gameObject.GetInstanceID();
        if (myTargetsHit.Contains(instanceID))
            return;

        myTargetsHit.Add(instanceID);

        other.gameObject.GetComponent<Health>().TakeDamage(myDamage);
    }
}
