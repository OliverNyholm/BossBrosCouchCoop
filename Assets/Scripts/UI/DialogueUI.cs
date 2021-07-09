using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueUI : MonoBehaviour
{
    private TextMeshProUGUI myText = null;
    private float myDuration = 0.0f;


    private void Awake()
    {
        myText = GetComponent<TextMeshProUGUI>();
        this.enabled = false;
        myText.enabled = false;
    }

    private void Update()
    {
        if (myDuration > 0.0f)
        {
            myDuration -= Time.deltaTime;
            if (myDuration <= 0.0f)
            {
                this.enabled = false;
                myText.enabled = false;
            }    
        }
    }

    public void SetDialogue(DialogueData aDialogueData, Color aColor)
    {
        this.enabled = true;
        myText.enabled = true;

        myText.text = aDialogueData.myText;
        myText.color = aColor;
        myDuration = aDialogueData.myDuration;
    }
}
