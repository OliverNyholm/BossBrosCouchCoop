using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterSelectSpellInfo : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI mySpellName = null;
    [SerializeField]
    private TextMeshProUGUI mySpellInfo = null;

    private CanvasGroup myCanvasGroup;

    private void Awake()
    {
        myCanvasGroup = GetComponent<CanvasGroup>();
    }

    public void ShowSpellInfo(Spell aSpell)
    {
        myCanvasGroup.alpha = 1.0f;
        mySpellName.text = aSpell.myName;
        mySpellInfo.text = aSpell.myTutorialInfo;
    }

    public void HideSpellInfo()
    {
        myCanvasGroup.alpha = 0.0f;
    }
}
