using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class HighlightObject : Action
{
    public GameObject myHighlightObject = null;
    public SharedGameObject myObjectToHighlight = null;
    public Vector3 myOffsetFromObject = Vector3.zero;

    [BehaviorDesigner.Runtime.Tasks.Tooltip("If this is set, the lifetime is based on the cast time.")]
    public GameObject myCastingSpell = null;

    public int myMaxHighlightsCount = 4;
    public float myLifetime = 0;

    public override void OnAwake()
    {
        base.OnAwake();

        PoolManager poolManager = PoolManager.Instance;
        UniqueID uniqueID = myHighlightObject.GetComponent<UniqueID>();
        if (uniqueID == null)
            Debug.LogError(myHighlightObject.name + " is missing unique ID!");

        poolManager.AddPoolableObjects(myHighlightObject, uniqueID.GetID(), myMaxHighlightsCount);

        if (myCastingSpell)
            myLifetime = myCastingSpell.GetComponent<Spell>().myCastTime;
    }

    public override void OnStart()
    {
        PoolManager poolManager = PoolManager.Instance;
        GameObject highlightObject = poolManager.GetPooledObject(myHighlightObject.GetComponent<UniqueID>().GetID());
        if (!highlightObject)
            return;

        Quaternion originalRotation = highlightObject.transform.rotation;
        highlightObject.transform.parent = myObjectToHighlight.Value.transform;
        highlightObject.transform.localPosition = Vector3.zero + myOffsetFromObject;
        highlightObject.transform.localRotation = originalRotation;

        if (myLifetime > 0.0f)
            poolManager.AddTemporaryObject(highlightObject, myLifetime);
    }

    public override TaskStatus OnUpdate()
    {
        return TaskStatus.Success;
    }
}
