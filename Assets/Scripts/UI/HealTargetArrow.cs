using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealTargetArrow : MonoBehaviour
{
    private Material myMaterial;

    void Start()
    {
        myMaterial = new Material(GetComponent<MeshRenderer>().material);
    }

    public void EnableHealTarget(Color aPlayerColor)
    {
        GetComponent<MeshRenderer>().enabled = true;
        myMaterial.SetColor("_Color", aPlayerColor);
    }

    public void DisableHealTarget()
    {
        GetComponent<MeshRenderer>().enabled = false;
    }
}
