using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.HDPipeline;

public class TargetProjector : MonoBehaviour
{
    private Material myMaterial;
    private DecalProjectorComponent myDecalProjector;

    private void Awake()
    {
        myDecalProjector = GetComponent<DecalProjectorComponent>();
        myMaterial = new Material(myDecalProjector.m_Material);

        myDecalProjector.m_Material = myMaterial;
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
        myDecalProjector.OnValidate();
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
        myDecalProjector.OnValidate();
    }

    public void SetPlayerColor(Color aColor)
    {
        myMaterial.SetColor("_PlayerColor", aColor);
    }
}
