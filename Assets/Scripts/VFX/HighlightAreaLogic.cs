using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class HighlightAreaLogic : PoolableObject
{
    private Material myMaterial;
    private DecalProjector myDecalProjector;

    private float myDuration;
    private float myLifeTime;

    private void Awake()
    {
        myDecalProjector = GetComponentInChildren<DecalProjector>();
        myMaterial = new Material(myDecalProjector.material);

        myDecalProjector.material = myMaterial;
    }

    public void SetData(float aDuration)
    {
        myDuration = aDuration;
    }

    private void OnEnable()
    {
        myLifeTime = 0.0f;
        myMaterial.SetFloat("_Percentage", 1.0f);
    }

    void Update()
    {
        myLifeTime += Time.deltaTime;

        myMaterial.SetFloat("_Percentage", 1.0f - myLifeTime / myDuration);
    }

    public override void Reset()
    {
        
    }
}
