using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetProjector : MonoBehaviour
{
    private Material myMaterial;

    private int myProjectionCounter;

    private void Start()
    {
        myMaterial = new Material(GetComponent<Projector>().material);

        GetComponent<Projector>().material = myMaterial;

        myProjectionCounter = 0;
    }
    public void AddTargetProjection(Color aColor, int aPlayerIndex)
    {
        if (myProjectionCounter++ == 0)
            GetComponent<Projector>().enabled = true;

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

        if (myProjectionCounter-- == 0)
            GetComponent<Projector>().enabled = false;
    }
}
