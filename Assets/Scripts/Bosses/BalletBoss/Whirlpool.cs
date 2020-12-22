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

        float launchDuration = 1.0f;
        aPlayer.GetComponent<PlayerMovementComponent>().GiveImpulse(Vector3.up * 100.0f, launchDuration);
        while (launchDuration > 0)
        {
            launchDuration -= Time.deltaTime;
            yield return null;
        }

        aPlayer.transform.position = aWhirlPool.transform.position + Vector3.up * 30.0f;

        float fallDuration = 0.8f;
        aPlayer.GetComponent<PlayerMovementComponent>().GiveImpulse(Vector3.down * 30.0f, fallDuration);
        while (fallDuration > 0)
        {
            fallDuration -= Time.deltaTime;
            yield return null;
        }

        myHandler.RemovePool(aWhirlPool);
        myHandler.RemovePool(this);
    }
}
