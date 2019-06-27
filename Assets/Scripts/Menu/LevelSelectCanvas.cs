using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectCanvas : MonoBehaviour
{
    [SerializeField]
    private Text myLevelText = null;

    [SerializeField]
    private Text myBossText = null;

    public void SetCanvasData(LevelInfo aLevelInfo)
    {
        GetComponent<RectTransform>().localPosition = aLevelInfo.myCanvasPosition.position;
        myLevelText.text = aLevelInfo.myLevelName;
        myBossText.text = aLevelInfo.myBossName;
    }
}
