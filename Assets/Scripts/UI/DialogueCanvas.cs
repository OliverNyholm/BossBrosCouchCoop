using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueCanvas : MonoBehaviour
{
    [SerializeField]
    private DialogueUI myDialogue = null;

    public static DialogueCanvas Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void AddDialogue(DialogueData aDialogueData, Color aColor)
    {
        myDialogue.SetDialogue(aDialogueData, aColor);
    }
}
