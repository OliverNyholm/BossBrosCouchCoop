using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelInfo : MonoBehaviour
{
    public Transform myCanvasPosition;
    public string mySceneNameToLoad;
    public string myLevelName;

    [Header("Information about boss fight")]
    public Sprite myBossSprite;
    public string myBossName;

    [TextArea(6, 15)]
    public string myBossLore;

    public List<PhaseData> myPhaseData;
}

[System.Serializable]
public class PhaseData
{
    public string myPhaseName;
    public List<AbilityData> myAbilities;
}

[System.Serializable]
public class AbilityData
{
    [TextArea(1, 2)]
    public string myDescription;
    public Sprite myAbilityImage = null;
}