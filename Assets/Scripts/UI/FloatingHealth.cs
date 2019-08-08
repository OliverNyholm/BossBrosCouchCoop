using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingHealth : PoolableObject
{
    private TextMesh myTextMesh;
    private TextMesh myOutLineTextMesh;
    public float myDuration;
    public float mySpeed;

    private Camera myMainCamera;

    private float myLifeTime;

    private void Awake()
    {
        myTextMesh = transform.GetComponent<TextMesh>();
        myOutLineTextMesh = transform.Find("Outline").gameObject.GetComponent<TextMesh>();

        myMainCamera = Camera.main;
    }

    void Update()
    {
        myTextMesh.transform.rotation = myMainCamera.transform.rotation;
        myOutLineTextMesh.transform.rotation = myMainCamera.transform.rotation;

        myLifeTime -= Time.deltaTime;
        if (myLifeTime <= 0.0f)
            ReturnToPool();

        else if (myLifeTime <= 1.0f)
        {
            Color fadeColor = new Color(myTextMesh.color.r, myTextMesh.color.g, myTextMesh.color.b, myLifeTime);
            myTextMesh.color = fadeColor;
            fadeColor = new Color(myOutLineTextMesh.color.r, myOutLineTextMesh.color.g, myOutLineTextMesh.color.b, myLifeTime);
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

    public override void Reset()
    {
        myLifeTime = myDuration;
    }
}
