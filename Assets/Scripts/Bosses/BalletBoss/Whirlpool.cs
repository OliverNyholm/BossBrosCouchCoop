using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Whirlpool : PoolableObject
{
    [SerializeField]
    private LayerMask myIntersectLayer = 0;

    private WhirlpoolHandler myHandler;
    private Collider myCollider;


    private void Awake()
    {
        myCollider = GetComponent<Collider>();
    }

    public void SetHandler(WhirlpoolHandler aHandler)
    {
        myHandler = aHandler;
    }

    public override void Reset()
    {
        myCollider.enabled = true;
    }

    private void OnTriggerEnter(Collider aOther)
    {
        if (!UtilityFunctions.Collides(aOther.gameObject.layer, myIntersectLayer))
            return;

        Whirlpool teleportToPool = myHandler.OnPlayerEnterWhirlpool(this);
        if (teleportToPool)
           StartCoroutine(TeleportToOtherPool(teleportToPool, aOther.gameObject));
    }

    public bool IsAvailable()
    {
        return myCollider.enabled;
    }

    public void DisablePool()
    {
        myCollider.enabled = false;
    }

    private IEnumerator TeleportToOtherPool(Whirlpool aWhirlPool, GameObject aPlayer)
    {
        DisablePool();
        aWhirlPool.DisablePool();

        aPlayer.transform.position = aWhirlPool.transform.position;

        float fadeDuration = 1.0f;
        while (fadeDuration > 0)
        {
            fadeDuration -= Time.deltaTime;
            yield return null;
        }

        myHandler.RemovePool(aWhirlPool);
        myHandler.RemovePool(this);
    }
}
