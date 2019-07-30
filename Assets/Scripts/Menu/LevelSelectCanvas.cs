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

    [SerializeField]
    private GameObject myUnavailableText = null;

    public void SetCanvasData(LevelInfo aLevelInfo, bool aIsAvailable)
    {
        GetComponent<RectTransform>().localPosition = aLevelInfo.myCanvasPosition.position;
        myLevelText.text = aLevelInfo.myLevelName;
        myBossText.text = aLevelInfo.myBossName;

        myUnavailableText.SetActive(!aIsAvailable);
    }
}
