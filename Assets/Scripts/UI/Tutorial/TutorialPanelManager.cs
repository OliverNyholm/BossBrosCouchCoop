using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPanelManager : MonoBehaviour
{
    [SerializeField]
    private TutorialPanel OneImageTutorialPanel = null;

    [SerializeField]
    private TutorialPanel TwoImageTutorialPanel = null;

    public void SetTutorialPanel(List<TutorialPanelImage> somePanelImages, string aTutorialText)
    {
        if (somePanelImages.Count == 1)
        {
            TwoImageTutorialPanel.gameObject.SetActive(false);

            OneImageTutorialPanel.gameObject.SetActive(true);
            OneImageTutorialPanel.SetData(somePanelImages, aTutorialText);
        }
        else if(somePanelImages.Count == 2)
        {
            OneImageTutorialPanel.gameObject.SetActive(false);

            TwoImageTutorialPanel.gameObject.SetActive(true);
            TwoImageTutorialPanel.SetData(somePanelImages, aTutorialText);
        }
    }
}
