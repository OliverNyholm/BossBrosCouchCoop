using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class TargetProjector : MonoBehaviour
{
    private Material myMaterial;
    private DecalProjector myDecalProjector;

    private void Awake()
    {
        myDecalProjector = GetComponent<DecalProjector>();
        myMaterial = new Material(myDecalProjector.material);

        myDecalProjector.material = myMaterial;
    }
    public void AddTargetProjection(Color aColor, int aPlayerIndex)
    {
        switch (aPlayerIndex)
        {
            case 1:
                myMaterial.SetColor("_PlayerOneColor", aColor);
                break;
            case 2:
                myMaterial.SetColor("_PlayerTwoColor", aColor);
                break;
            case 3:
                myMaterial.SetColor("_PlayerThreeColor", aColor);
                break;
            case 4:
                myMaterial.SetColor("_PlayerFourColor", aColor);
                break;
        }
    }

    public void DropTargetProjection(int aPlayerIndex)
    {
        Color off = new Color(0.0f, 0.0f, 0.0f, 0.0f);
        switch (aPlayerIndex)
        {
            case 1:
                myMaterial.SetColor("_PlayerOneColor", off);
                break;
            case 2:
                myMaterial.SetColor("_PlayerTwoColor", off);
                break;
            case 3:
                myMaterial.SetColor("_PlayerThreeColor", off);
                break;
            case 4:
                myMaterial.SetColor("_PlayerFourColor", off);
                break;
        }
    }

    public void SetPlayerColor(Color aColor)
    {
        if(myMaterial)
            myMaterial.SetColor("_PlayerColor", aColor);
    }
}
