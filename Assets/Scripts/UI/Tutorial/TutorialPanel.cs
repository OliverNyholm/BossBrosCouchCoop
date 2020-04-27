using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialPanel : MonoBehaviour
{
    [Header("Children that show Tutorial Facts")]
    [SerializeField]
    private Text myTutorialText = null;
    [SerializeField]
    private Image myInfoImage = null;

    public void SetData(string aTutorialText, Sprite aTutorialImageSprite)
    {
        GetComponent<CanvasGroup>().alpha = 1.0f;
        myTutorialText.text = aTutorialText;
        myInfoImage.sprite = aTutorialImageSprite;
    }
}
