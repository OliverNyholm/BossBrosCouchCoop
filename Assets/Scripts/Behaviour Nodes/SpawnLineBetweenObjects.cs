using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class SpawnLineBetweenObjects : Action
{
    public GameObject myLine = null;
    public SharedGameObject myFirstObject = null;
    public SharedGameObject mySecondObject = null;

    [BehaviorDesigner.Runtime.Tasks.Tooltip("If this is set, the lifetime is based on the cast time.")]
    public GameObject myCastingSpell = null;

    public int myMaxLinesCount = 4;
    public float myLifetime = 0;

    public override void OnAwake()
    {
        base.OnAwake();

        PoolManager poolManager = PoolManager.Instance;
        if (myLine.GetComponent<UniqueID>() == null)
            Debug.LogError(myLine.name + " is missing unique ID!");

        poolManager.AddPoolableObjects(myLine, myLine.GetComponent<UniqueID>().GetID(), myMaxLinesCount);

        if (myCastingSpell)
            myLifetime = myCastingSpell.GetComponent<Spell>().myCastTime;
    }

    public override void OnStart()
    {
        PoolManager poolManager = PoolManager.Instance;
        GameObject lineGO = poolManager.GetPooledObject(myLine.GetComponent<UniqueID>().GetID());
        if (!lineGO)
            return;

        LineBetweenTwoPoints line = lineGO.GetComponent<LineBetweenTwoPoints>();
        line.SetPoints(myFirstObject.Value.transform, mySecondObject.Value.transform);

        if (myLifetime > 0.0f)
            poolManager.AddTemporaryObject(lineGO, myLifetime);
    }

    public override TaskStatus OnUpdate()
    {
        return TaskStatus.Success;
    }
}
