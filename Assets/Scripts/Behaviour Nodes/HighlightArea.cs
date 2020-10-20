using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class HighlightArea : Action
{
    public GameObject myHighlightPrefab = null;
    public int mySpellMaxCount = 8;

    private GameObject myHightlightObject = null;
    private uint myUniqueID = uint.MaxValue;

    public SharedVector3 mySpawnPosition = null;

    public float myHighlightDuration;
    private float myTimer;

    public override void OnAwake()
    {
        base.OnAwake();

        myUniqueID = myHighlightPrefab.GetComponent<UniqueID>().GetID();
        PoolManager.Instance.AddPoolableObjects(myHighlightPrefab, myUniqueID, mySpellMaxCount);
    }

    public override void OnStart()
    {
        myTimer = myHighlightDuration;

        myHightlightObject = PoolManager.Instance.GetPooledObject(myUniqueID);
        if (myHightlightObject)
        {
            myTimer = 0.0f;
            return;
        }

        myHightlightObject.transform.position = mySpawnPosition.Value;

        HighlightAreaLogic logic = myHightlightObject.GetComponent<HighlightAreaLogic>();
        if (logic)
            logic.SetData(myHighlightDuration);
    }

    public override TaskStatus OnUpdate()
    {
        myTimer -= Time.deltaTime;
        if(myTimer <= 0.0f)
            return TaskStatus.Success;

        return TaskStatus.Running;
    }

    public override void OnEnd()
    {
        PoolManager.Instance.ReturnObject(myHightlightObject, myUniqueID);
        myHightlightObject = null;
    }
}
