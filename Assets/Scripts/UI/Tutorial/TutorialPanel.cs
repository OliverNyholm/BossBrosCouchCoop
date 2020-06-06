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
    private List<Image> myInfoImages = new List<Image>();

    public void SetData(List<TutorialPanelImage> somePanelImages, string aTutorialText)
    {
        myTutorialText.text = aTutorialText;

        for (int index = 0; index < myInfoImages.Count; index++)
        {
            myInfoImages[index].sprite = somePanelImages[index].myImageSprite;
            myInfoImages[index].material = somePanelImages[index].myImageGif;

            if (myInfoImages[index].material)
                myInfoImages[index].material.SetFloat("_EnableTime", Time.time);
        }
    }
}
