using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu(fileName = "Dialogue", menuName = "Scriptables/Dialogue")]
public class DialogueData : ScriptableObject
{
    [TextArea(2, 10)]
    public string myText = "";
    public string myAudioEvent = "";
    public float myDuration = 3.0f;
}
