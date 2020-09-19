using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightAreaLogic : PoolableObject
{
    private MeshRenderer myDecalProjector;

    private float myDuration;
    private float myLifeTime;

    private void Awake()
    {
        myDecalProjector = GetComponentInChildren<MeshRenderer>();
        myDecalProjector.material = new Material(myDecalProjector.material);
    }

    public void SetData(float aDuration, float aRadius)
    {
        myDuration = aDuration;

        Transform decalTransform = transform.GetChild(0);
        Vector3 decalScale = decalTransform.lossyScale;
        decalScale.x = aRadius * 2.0f;
        decalScale.z = aRadius * 2.0f;
        decalTransform.localScale = decalScale;
    }

    private void OnEnable()
    {
        myLifeTime = 0.0f;
        myDecalProjector.material.SetFloat("_LifetimePercentage", 1.0f);
    }

    void Update()
    {
        myLifeTime += Time.deltaTime;

        myDecalProjector.material.SetFloat("_LifetimePercentage", 1.0f - myLifeTime / myDuration);
    }

    public override void Reset()
    {
        myLifeTime = 0;
        myDecalProjector.material.SetFloat("_LifetimePercentage", 1.0f);
    }
}
