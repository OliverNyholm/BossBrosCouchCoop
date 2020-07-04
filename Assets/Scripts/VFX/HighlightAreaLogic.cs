using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightAreaLogic : PoolableObject
{
    private Material myMaterial;
    private MeshRenderer myDecalProjector;

    private float myDuration;
    private float myLifeTime;

    private void Awake()
    {
        myDecalProjector = GetComponentInChildren<MeshRenderer>();
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
