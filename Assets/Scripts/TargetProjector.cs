using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetProjector : MonoBehaviour
{
    private Material myMaterial;

    private void Awake()
    {
        myMaterial = new Material(GetComponent<Projector>().material);

        GetComponent<Projector>().material = myMaterial;
    }
    public void AddTargetProjection(Color aColor, int aPlayerIndex)
    {
        switch (aPlayerIndex)
        {
            case 1:
                myMaterial.SetColor("_TopLeft", aColor);
                break;
            case 2:
                myMaterial.SetColor("_TopRight", aColor);
                break;
            case 3:
                myMaterial.SetColor("_BottomLeft", aColor);
                break;
            case 4:
                myMaterial.SetColor("_BottomRight", aColor);
                break;
        }
    }

    public void DropTargetProjection(int aPlayerIndex)
    {
        switch (aPlayerIndex)
        {
            case 1:
                myMaterial.SetColor("_TopLeft", Color.black);
                break;
            case 2:
                myMaterial.SetColor("_TopRight", Color.black);
                break;
            case 3:
                myMaterial.SetColor("_BottomLeft", Color.black);
                break;
            case 4:
                myMaterial.SetColor("_BottomRight", Color.black);
                break;
        }
    }

    public void SetPlayerColor(Color aColor)
    {
        myMaterial.SetColor("_PlayerColor", aColor);
    }
}
