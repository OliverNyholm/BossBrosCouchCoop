using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelInfo : MonoBehaviour
{
    public Transform myCanvasPosition = null;
    public string mySceneNameToLoad = "";
    public string myLevelName = "";

    public List<GameObject> myLevelsToUnlock = new List<GameObject>();

    [Header("Information about boss fight")]
    public Sprite myBossSprite = null;
    public string myBossName = "NO BOSS";

    [TextArea(6, 15)]
    public string myBossLore = "Shit lore";

    public List<PhaseData> myPhaseData = new List<PhaseData>();
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