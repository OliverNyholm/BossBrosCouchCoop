using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossDetailsPanel : MonoBehaviour
{
    [Header("The UI elements that are changed for each boss")]
    [SerializeField]
    private Text myBossName = null;
    [SerializeField]
    private Text myLoreText = null;
    [SerializeField]
    private Image myBossImage = null;
    [SerializeField]
    private GameObject myPhasePanel = null;

    [Header("The Prefabs for all phase information")]
    [Space(10)]
    [SerializeField]
    private GameObject myPhasePrefab = null;
    [SerializeField]
    private GameObject myDescriptionPrefab = null;
    [SerializeField]
    private GameObject myAbilityPrefab = null;

    public void ShowBossDetails(LevelInfo aLevelInfo)
    {
        myBossImage.sprite = aLevelInfo.myBossSprite;
        myBossName.text = aLevelInfo.myBossName;
        myLoreText.text = aLevelInfo.myBossLore;

        for (int index = 0; index < aLevelInfo.myPhaseData.Count; index++)
        {
            GameObject phase = Instantiate(myPhasePrefab, myPhasePanel.transform);
            phase.GetComponent<Text>().text = aLevelInfo.myPhaseData[index].myPhaseName;

            List<AbilityData> abilityData = aLevelInfo.myPhaseData[index].myAbilities;
            for (int abilityIndex = 0; abilityIndex < abilityData.Count; abilityIndex++)
            {
                GameObject ability = null;
                if (abilityData[abilityIndex].myAbilityImage == null)
                {
                    ability = Instantiate(myDescriptionPrefab, phase.transform);
                    ability.GetComponent<Text>().text = abilityData[abilityIndex].myDescription;
                }
                else
                {
                    ability = Instantiate(myAbilityPrefab, phase.transform);
                    ability.GetComponent<Image>().sprite = abilityData[abilityIndex].myAbilityImage;
                    ability.GetComponentInChildren<Text>().text = abilityData[abilityIndex].myDescription;
                }
            }
        }

        GetComponent<CanvasGroup>().alpha = 1.0f;
    }

    public void HideBossDetails()
    {
        foreach (Transform child in myPhasePanel.transform)
            Destroy(child.gameObject);

        GetComponent<CanvasGroup>().alpha = 0.0f;
    }

    public void ToggleBossDetails(LevelInfo aLevelInfo)
    {
        if (AreDetailsShowing())
            HideBossDetails();
        else
            ShowBossDetails(aLevelInfo);
    }

    private bool AreDetailsShowing()
    {
        if (GetComponent<CanvasGroup>().alpha > 0.0f)
            return true;

        return false;
    }
}
