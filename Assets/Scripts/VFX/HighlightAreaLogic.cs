using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.HDPipeline;

public class HighlightAreaLogic : PoolableObject
{
    private Material myMaterial;
    private DecalProjectorComponent myDecalProjector;

    private float myDuration;
    private float myLifeTime;

    private void Awake()
    {
        myDecalProjector = GetComponentInChildren<DecalProjectorComponent>();
        myMaterial = new Material(myDecalProjector.m_Material);

        myDecalProjector.m_Material = myMaterial;
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
