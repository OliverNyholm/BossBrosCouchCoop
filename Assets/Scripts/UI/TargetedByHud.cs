using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetedByHud : MonoBehaviour
{
    [SerializeField]
    List<Image> myTargetIcons = new List<Image>(4);

    public void SetTargetedBy(Color aColor, int aPlayerIndex)
    {
        myTargetIcons[aPlayerIndex -1].color = aColor;
        myTargetIcons[aPlayerIndex -1].enabled = true;
    }

    public void RemoveTargetedBy(int aPlayerIndex)
    {
        myTargetIcons[aPlayerIndex - 1].enabled = false;
    }
}
