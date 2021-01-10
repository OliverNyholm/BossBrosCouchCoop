using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetProjector : MonoBehaviour
{
    private Material myMaterial;
    private MeshRenderer myDecalProjector;

    [SerializeField]
    private bool myRaycastPosition = false;

    private Vector3 myOffsetFromParent = Vector3.zero;

    private void Awake()
    {
        myDecalProjector = GetComponent<MeshRenderer>();
        myMaterial = new Material(myDecalProjector.material);

        myDecalProjector.material = myMaterial;

        myOffsetFromParent = transform.localPosition;
    }

    private void Update()
    {
        if (!myRaycastPosition)
            return;

        if (UtilityFunctions.FindGroundFromLocation(transform.parent.position, out Vector3 hitLocation, out _, 5.0f))
            transform.position = hitLocation + Vector3.up * 0.01f;
        else
            transform.localPosition = myOffsetFromParent;
    }

    public void AddTargetProjection(Color aColor, int aPlayerIndex)
    {
        switch (aPlayerIndex)
        {
            case 1:
                myMaterial.SetColor("_TopLeftColor", aColor);
                break;
            case 2:
                myMaterial.SetColor("_TopRightColor", aColor);
                break;
            case 3:
                myMaterial.SetColor("_BottomLeftColor", aColor);
                break;
            case 4:
                myMaterial.SetColor("_BottomRightColor", aColor);
                break;
        }
    }

    public void DropTargetProjection(int aPlayerIndex)
    {
        Color off = new Color(0.0f, 0.0f, 0.0f, 0.0f);
        switch (aPlayerIndex)
        {
            case 1:
                myMaterial.SetColor("_TopLeftColor", off);
                break;
            case 2:
                myMaterial.SetColor("_TopRightColor", off);
                break;
            case 3:
                myMaterial.SetColor("_BottomLeftColor", off);
                break;
            case 4:
                myMaterial.SetColor("_BottomRightColor", off);
                break;
        }
    }

    public void SetPlayerColor(Color aColor)
    {
        if(myMaterial)
            myMaterial.SetColor("_PlayerColor", aColor);
    }
}
