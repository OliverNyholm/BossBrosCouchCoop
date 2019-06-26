using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingHealth : MonoBehaviour
{
    private TextMesh myTextMesh;
    private TextMesh myOutLineTextMesh;
    public float myDuration;
    public float mySpeed;

    private void Awake()
    {
        myTextMesh = transform.GetComponent<TextMesh>();
        myOutLineTextMesh = transform.Find("Outline").gameObject.GetComponent<TextMesh>();
    }

    void Update()
    {
        myTextMesh.transform.rotation = Camera.main.transform.rotation;
        myOutLineTextMesh.transform.rotation = Camera.main.transform.rotation;

        myDuration -= Time.deltaTime;
        if (myDuration <= 0.0f)
            Destroy(gameObject);
        else if (myDuration <= 1.0f)
        {
            Color fadeColor = new Color(myTextMesh.color.r, myTextMesh.color.g, myTextMesh.color.b, myDuration);
            myTextMesh.color = fadeColor;
            fadeColor = new Color(myOutLineTextMesh.color.r, myOutLineTextMesh.color.g, myOutLineTextMesh.color.b, myDuration);
            myOutLineTextMesh.color = fadeColor;
        }

        transform.position += Vector3.up * mySpeed * Time.deltaTime;
    }

    public void SetText(string aText, Color aColor, float aSizeModifier)
    {
        myTextMesh.text = aText;
        myOutLineTextMesh.text = aText;
        myTextMesh.color = aColor;

        myTextMesh.characterSize *= aSizeModifier;
    }
}
