using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TutorialGuideData", menuName = "Scriptables/TutorialGuideData")]
public class TutorialGuideData : ScriptableObject
{
    public DialogueData myOnActivate = null;
    public DialogueData myOnFailed = null;
    public DialogueData myOnCompleteData = null;
}
