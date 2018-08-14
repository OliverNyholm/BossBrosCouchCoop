using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ErrorMessage : MonoBehaviour
{

    [SerializeField]
    private float myLifeTime;

    [SerializeField]
    private float myFadeTime;
    private float myCurrentFadeTime;

    private Text myText;

    void Awake()
    {
        myText = GetComponent<Text>();
        myCurrentFadeTime = myFadeTime;
    }

    void Update()
    {
        myLifeTime -= Time.deltaTime;

        if (myLifeTime <= myFadeTime)
        {
            myCurrentFadeTime -= Time.deltaTime;

            Color newColor = myText.color;
            newColor.a = myCurrentFadeTime / myFadeTime;
            myText.color = newColor;
        }
    }

    public void SetText(string aText)
    {
        myText.text = aText;
    }

    public bool ShouldRemove()
    {
        return myLifeTime <= 0.0f;
    }
}
